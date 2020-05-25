using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photobox.DB;
using Photobox.Models;

namespace Photobox.Controllers {
    [ApiController]
    [Route ("api/[controller]")]
    public class ImageController : ControllerBase {
        public ImageController (PhotoboxDB DB) {
            this.DB = DB;
        }
        private PhotoboxDB DB;
        private HttpClient client = new HttpClient ();

        // GET /api/image/{orderId} Hämta alla bilder på ett uppdrag
        [HttpGet ("{orderId}"), Authorize]
        public IActionResult GetImagesFromOrder (string orderId) {
            try {
                IActionResult response;

                List<Image> imageArray = this.DB.GetAllImagesOnOrder (orderId);

                if (imageArray == null) {
                    response = NotFound (new { message = "No images was found on this order." });

                } else {
                    response = Ok (imageArray);
                }

                return response;

            } catch (Exception err) {
                IActionResult response = StatusCode (StatusCodes.Status500InternalServerError, err);
                return response;
            }
        }

        // POST /api/image Lägg till bild(er) på ett uppdrag
        [HttpPost, Authorize]
        public IActionResult SaveImagesToOrder ([FromBody] Image image) {
            try {
                IActionResult response;

                if (image == null) return response = BadRequest (new { message = "No images was found in request body" });

                Order order = DB.GetOrderById (image.orderId);

                image.installationId = order.installationId;
                image.estateId = order.estateId;
                image.externalImageReference = Guid.NewGuid ().ToString ();
                image.deliveryPackageId = "tilldelas vid sändning till next";
                image.extension = Image.ExtensionType.JPG;
                image.imageQuality = Image.ImageQualityType.print;
                image.submittedToNext = image.submittedToNext;

                this.DB.AddImagesToOrder (image);

                response = StatusCode (StatusCodes.Status201Created, new { message = image.externalImageReference });
                return response;

            } catch (Exception err) {
                Console.WriteLine (err.ToString ());
                IActionResult response = StatusCode (StatusCodes.Status500InternalServerError, err.ToString ());
                return response;
            }
        }

        /// <summary>
        /// Update all images on an order
        /// api/image/{orderId}/sort/
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        [HttpPut ("{orderId}/sort"), Authorize]
        public IActionResult UpdateImagesOnOrder ([FromBody] List<Image> images) {
            try {
                IActionResult response;

                string orderId = images[0].orderId;

                images.ForEach (img => {
                    this.DB.UpdateImage (img);
                });

                response = Ok (new { message = "Images updated" });
                return response;

            } catch (Exception err) {
                IActionResult response = StatusCode (StatusCodes.Status500InternalServerError, err.ToString ());
                return response;
            }
        }

        // PUT /api/image/{externalImageReference} Ändra bildtext, taggar på en bild
        [HttpPut ("{externalImageReference}"), Authorize]
        public IActionResult UpdateImageDetails ([FromBody] Image updatedImage) {
            try {
                IActionResult response;

                this.DB.UpdateImage (updatedImage);

                response = Ok (updatedImage);
                return response;

            } catch (Exception err) {
                IActionResult response = StatusCode (StatusCodes.Status500InternalServerError, err.ToString ());
                return response;
            }
        }

        // DELETE /api/image/{externalImageReference} Ta bort enskild bild
        [HttpDelete ("{externalImageReference}"), Authorize]
        public IActionResult DeleteImage (string externalImageReference) {
            try {
                IActionResult response;

                this.DB.DeleteImageById (externalImageReference);

                response = Ok ();
                return response;

            } catch (Exception err) {
                IActionResult response = StatusCode (StatusCodes.Status500InternalServerError, err.ToString ());
                return response;
            }
        }

        // POST /api/image/{orderId}/send Skicka bilder på ett uppdrag till Next
        [HttpPost ("{orderId}/send"), Authorize]
        public IActionResult SendImages (string orderId) {
            try {
                IActionResult response = Ok ("not given a value");
                this.client.BaseAddress = new Uri ("https://hubtest.megler.vitec.net/");
                // this.client.BaseAddress = new Uri ("https://hub.megler.vitec.net/");
                this.client.DefaultRequestHeaders.Accept.Add (
                    new MediaTypeWithQualityHeaderValue ("application/json"));
                this.client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue ("Basic", "Vml0ZWNmb3RvcG9ydGFsOmFtSzlzY0VnSnpNd1Bqckdack03bUNIY1llWEdsbmxDdEIyQ3JweHU0YTlIa25Eektn");
                this.client.DefaultRequestHeaders.ConnectionClose = true;
                this.client.DefaultRequestHeaders.Add ("Connection", "Keep-Alive");
                this.client.DefaultRequestHeaders.Add ("Keep-Alive", "3600");
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                List<Image> images = this.DB.GetAllImagesOnOrder (orderId, true, false);
                if (images == null) return response = BadRequest (new { message = "No images on order" });
                string packageId = Guid.NewGuid ().ToString ();
                foreach (Image image in images) {
                    string uri = image.installationId + "/Estates/" + image.estateId + "/Images";
                    // string uri = "MSVPAR" + "/Estates/" + "0DF7D5B6-A2E0-4A07-BCD4-34CB64E3C4B7" + "/Images";
                    uri += "?externalImageReference=" + image.externalImageReference;
                    uri += "&deliveryPackageId=" + packageId; // "Unik id for denne pakke med bilder (en bilde-bestilling f.eks. tournr)"
                    uri += "&imageDescription=" + image.imageDescription;
                    uri += "&imageSequence=" + image.imageSequence;
                    uri += "&extension=" + image.extension;
                    uri += "&imageQuality=" + image.imageQuality;
                    uri += "&approvedBroker=" + true;
                    uri += "&approvedSeller=" + true;
                    uri += "&imageCategoryName=" + image.imageCategoryName;

                    var imageBinaryContent = new ByteArrayContent (image.imageFile);
                    var result = client.PostAsync (uri, imageBinaryContent).Result;

                    Order order = this.DB.GetOrderById (orderId);
                    order.archiveDate = DateTime.Now;
                    order.status = StatusType.Delivered;

                    this.DB.UpdateOrder (order);
                    if (result.IsSuccessStatusCode) {
                        image.submittedToNext = true;
                        this.DB.UpdateImage (image);
                        Console.WriteLine ("Success");
                        response = Ok (true);
                    } else {
                        Console.WriteLine ("Error");
                        response = Ok (false);
                    }
                }
                return response;
            } catch (Exception err) {
                IActionResult response = StatusCode (StatusCodes.Status500InternalServerError, err.ToString ());
                return response;
            }
        }
    }
}