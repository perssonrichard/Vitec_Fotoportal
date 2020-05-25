using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Photobox.CustomExceptions;

namespace Photobox.Models
{
    /// <summary>
    /// Image object
    /// </summary>
    public class Image
    {
        private const int THUMBNAIL_WIDTH = 700;

        private string _orderId;
        private string _installationId;
        private string _estateId;
        private string _externalImageReference;
        private string _imageDescription;
        private string _deliveryPackageId;
        private int _imageSequence;
        private ExtensionType _extension;
        private ImageQualityType _imageQuality;
        private string _imageCategoryName = " ";
        private byte[] _imageFile;
        private byte[] _thumbnail;
        private bool _createThumbnail;
        private string _fileName;
        private bool _submittedToNext;

        public Image (bool thumbnail = true)
        {
            _createThumbnail = thumbnail;
        }

        /// <summary>
        /// Used internally to identify order id
        /// </summary>
        public string orderId { get => _orderId; set => _orderId = value; }

        /// <summary>
        /// Reference to the client id
        /// Obtained when recieving an order
        /// </summary>
        public string installationId { get => _installationId; set => _installationId = value; }

        /// <summary>
        /// Reference to the estate id
        /// Obtained when recieving an order
        /// </summary>
        public string estateId { get => _estateId; set => _estateId = value; }

        /// <summary>
        /// Unique id (guid) for image file
        /// </summary>
        public string externalImageReference { get => _externalImageReference; set => _externalImageReference = value; }

        /// <summary>
        /// Description of this specific image
        /// </summary>
        public string imageDescription { get => _imageDescription; set => _imageDescription = value; }

        /// <summary>
        /// Unique id for package of images
        /// </summary>
        public string deliveryPackageId { get => _deliveryPackageId; set => _deliveryPackageId = value; }

        /// <summary>
        /// Sorting number
        /// </summary>
        public int imageSequence { get => _imageSequence; set => _imageSequence = value; }

        /// <summary>
        /// File extension
        /// </summary>
        public ExtensionType extension { get => _extension; set => _extension = value; }

        /// <summary>
        /// Image quality
        /// </summary>
        public ImageQualityType imageQuality { get => _imageQuality; set => _imageQuality = value; }

        /// <summary>
        /// Image type name
        /// "Floor plans" for example
        /// </summary>
        public string imageCategoryName { get => _imageCategoryName; set => _imageCategoryName = value; }

        /// <summary>
        /// The raw image file
        /// </summary>
        public byte[] imageFile
        {
            get => _imageFile;
            set
            {
                _imageFile = value;
                _thumbnail = Resize (value, THUMBNAIL_WIDTH);
            }
        }

        /// <summary>
        /// Image thumbnail
        /// </summary>
        public byte[] thumbnail { get => _thumbnail; set => _thumbnail = value; }

        /// <summary>
        /// Image file name
        /// </summary>
        public string fileName { get => _fileName; set => _fileName = value; }

        /// <summary>
        /// If the image is submitted to Next
        /// </summary>
        public bool submittedToNext { get => _submittedToNext; set => _submittedToNext = value; }

        /// <summary>
        /// Possible extension types of image
        /// </summary>
        public enum ExtensionType
        {
            JPG
        }

        /// <summary>
        /// Possible image quality types of image
        /// </summary>
        public enum ImageQualityType
        {
            standard,
            print
        }

        /// <summary>
        /// Resize an image to a thumbnail
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private byte[] Resize (byte[] data, int width)
        {
            using (var stream = new MemoryStream (data))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream (stream);

                if (img.Width < THUMBNAIL_WIDTH)
                    throw new ImageTooSmallException ();

                var height = (width * img.Height) / img.Width;

                var resized = new Bitmap (width, height, PixelFormat.Format24bppRgb);

                // Draw image with high quality
                using (var graphics = Graphics.FromImage (resized))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage (img, 0, 0, width, height);
                }

                // Get an ImageCodecInfo object that represents the JPEG codec.
                ImageCodecInfo imageCodecInfo = this.GetEncoderInfo (ImageFormat.Jpeg);

                // Create an Encoder object for the Quality parameter.
                Encoder encoder = Encoder.Quality;

                // Create an EncoderParameters object. 
                EncoderParameters encoderParameters = new EncoderParameters (1);

                EncoderParameter encoderParameter = new EncoderParameter (encoder, 100L);
                encoderParameters.Param[0] = encoderParameter;

                using (var thumbnailStream = new MemoryStream ())
                {
                    resized.Save (thumbnailStream, imageCodecInfo, encoderParameters);
                    return thumbnailStream.ToArray ();
                }
            }
        }

        /// <summary>
        /// Method to get encoder infor for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private ImageCodecInfo GetEncoderInfo (ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders ().SingleOrDefault (c => c.FormatID == format.Guid);
        }
    }
}