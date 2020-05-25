using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Photobox.CustomExceptions;
using Photobox.Models;

namespace Photobox.DB
{
    /// <summary>
    /// Does queries against the image table
    /// </summary>
    public class HandleImageTable
    {
        // Table constants
        private const string IMAGE_TABLE = "Image";
        private const string ORDER_ID = "orderId";
        private const string INSTALLATION_ID = "installationId";
        private const string ESTATE_ID = "estateId";
        private const string EXTERNAL_IMAGE_REFERENCE = "externalImageReference";
        private const string IMAGE_DESCRIPTION = "imageDescription";
        private const string DELIVERY_PACKAGE_ID = "deliveryPackageId";
        private const string IMAGE_SEQUENCE = "imageSequence";
        private const string EXTENSION = "extension";
        private const string IMAGE_QUALITY = "imageQuality";
        private const string IMAGE_CATEGORY_NAME = "imageCategoryName";
        private const string IMAGE_FILE = "imageFile";
        private const string THUMBNAIL = "thumbnail";
        private const string FILE_NAME = "fileName";
        private const string SUBMITTED_TO_NEXT = "submittedToNext";

        private string connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="con"></param>
        public HandleImageTable (string conString) => connectionString = conString;

        /// <summary>
        /// Add one image to an order
        /// </summary>
        /// <param name="img"></param>
        public void AddToOrder (Image img)
        {
            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                string sql = CreateSqlStringForAdd ();

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = img.orderId;
                    cmd.Parameters.Add ($"@{INSTALLATION_ID}", SqlDbType.NVarChar).Value = img.installationId;
                    cmd.Parameters.Add ($"@{ESTATE_ID}", SqlDbType.NVarChar).Value = img.estateId;
                    cmd.Parameters.Add ($"@{EXTERNAL_IMAGE_REFERENCE}", SqlDbType.NVarChar).Value = img.externalImageReference;
                    cmd.Parameters.Add ($"@{IMAGE_DESCRIPTION}", SqlDbType.NVarChar).Value = img.imageDescription;
                    cmd.Parameters.Add ($"@{DELIVERY_PACKAGE_ID}", SqlDbType.NVarChar).Value = img.deliveryPackageId;
                    cmd.Parameters.Add ($"@{IMAGE_SEQUENCE}", SqlDbType.Int).Value = img.imageSequence;
                    cmd.Parameters.Add ($"@{EXTENSION}", SqlDbType.NVarChar).Value = img.extension;
                    cmd.Parameters.Add ($"@{IMAGE_QUALITY}", SqlDbType.NVarChar).Value = img.imageQuality;
                    cmd.Parameters.Add ($"@{IMAGE_CATEGORY_NAME}", SqlDbType.NVarChar).Value = img.imageCategoryName;
                    cmd.Parameters.Add ($"@{IMAGE_FILE}", SqlDbType.Image).Value = img.imageFile;
                    cmd.Parameters.Add ($"@{THUMBNAIL}", SqlDbType.Image).Value = img.thumbnail;
                    cmd.Parameters.Add ($"@{FILE_NAME}", SqlDbType.NVarChar).Value = img.fileName;
                    cmd.Parameters.Add ($"@{SUBMITTED_TO_NEXT}", SqlDbType.Bit).Value = img.submittedToNext;

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery ();
                }
                connection.Close ();
            }
        }

        /// <summary>
        /// Get image by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Image GetById (string id)
        {
            if (String.IsNullOrEmpty (id) || String.IsNullOrWhiteSpace (id))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!IdExist (id, EXTERNAL_IMAGE_REFERENCE, connection))
                {
                    connection.Close ();
                    return null;
                }

                var sql = $"SELECT * FROM {IMAGE_TABLE} WHERE {EXTERNAL_IMAGE_REFERENCE} = @{EXTERNAL_IMAGE_REFERENCE};";

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    cmd.Parameters.Add ($"@{EXTERNAL_IMAGE_REFERENCE}", SqlDbType.NVarChar).Value = id;
                    Image img = new Image ();

                    using (SqlDataReader reader = cmd.ExecuteReader ())
                    {
                        while (reader.Read ())
                        {
                            img.orderId = reader[ORDER_ID].ToString ();
                            img.installationId = reader[INSTALLATION_ID].ToString ();
                            img.estateId = reader[ESTATE_ID].ToString ();
                            img.externalImageReference = reader[EXTERNAL_IMAGE_REFERENCE].ToString ();
                            img.imageDescription = reader[IMAGE_DESCRIPTION].ToString ();
                            img.deliveryPackageId = reader[DELIVERY_PACKAGE_ID].ToString ();
                            img.imageSequence = reader.GetInt32 (IMAGE_SEQUENCE);
                            img.extension = (Image.ExtensionType) Enum.Parse (typeof (Image.ExtensionType), reader[EXTENSION].ToString ());
                            img.imageQuality = (Image.ImageQualityType) Enum.Parse (typeof (Image.ImageQualityType), reader[IMAGE_QUALITY].ToString ());
                            img.imageFile = (byte[]) reader[IMAGE_FILE];
                            img.fileName = reader[FILE_NAME].ToString ();
                            img.submittedToNext = reader.GetBoolean (SUBMITTED_TO_NEXT);
                        }
                        connection.Close ();

                        return img;
                    }
                }
            }
        }

        /// <summary>
        /// Get all images on a specific order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="fullQuality"></param>
        /// <returns></returns>
        public List<Image> GetImagesOnOrder (string orderId, bool fullQuality = false, bool getThumbnail = true)
        {
            if (String.IsNullOrEmpty (orderId) || String.IsNullOrWhiteSpace (orderId))
                throw new ArgumentNullException ();

            List<Image> imgs = new List<Image> ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!IdExist (orderId, ORDER_ID, connection))
                {
                    connection.Close ();
                    return null;
                }

                string sql = $"SELECT * FROM {IMAGE_TABLE} WHERE {ORDER_ID}=@{ORDER_ID};";

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = orderId;

                    using (SqlDataReader reader = cmd.ExecuteReader ())
                    {
                        while (reader.Read ())
                        {
                            Image img = new Image (getThumbnail);

                            img.orderId = reader[ORDER_ID].ToString ();
                            img.installationId = reader[INSTALLATION_ID].ToString ();
                            img.estateId = reader[ESTATE_ID].ToString ();
                            img.externalImageReference = reader[EXTERNAL_IMAGE_REFERENCE].ToString ();
                            img.imageDescription = reader[IMAGE_DESCRIPTION].ToString ();
                            img.deliveryPackageId = reader[DELIVERY_PACKAGE_ID].ToString ();
                            img.imageSequence = reader.GetInt32 (IMAGE_SEQUENCE);
                            img.extension = (Image.ExtensionType) Enum.Parse (typeof (Image.ExtensionType), reader[EXTENSION].ToString ());
                            img.imageQuality = (Image.ImageQualityType) Enum.Parse (typeof (Image.ImageQualityType), reader[IMAGE_QUALITY].ToString ());

                            if (fullQuality)
                                img.imageFile = (byte[]) reader[IMAGE_FILE];
                            else
                                img.thumbnail = (byte[]) reader[THUMBNAIL];

                            img.fileName = reader[FILE_NAME].ToString ();
                            img.submittedToNext = reader.GetBoolean (SUBMITTED_TO_NEXT);

                            imgs.Add (img);
                        }
                        connection.Close ();

                        return imgs;
                    }
                }
            }
        }

        /// <summary>
        /// Update image
        /// </summary>
        /// <param name="updatedImage"></param>
        public void UpdateImage (Image updatedImage)
        {
            if (String.IsNullOrEmpty (updatedImage.externalImageReference) || String.IsNullOrWhiteSpace (updatedImage.externalImageReference))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!IdExist (updatedImage.externalImageReference, EXTERNAL_IMAGE_REFERENCE, connection))
                {
                    connection.Close ();
                    throw new ImageDoesNotExistException ();
                }

                string sql = CreateSqlStringForUpdate ();

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    cmd.Parameters.Add ($"@{IMAGE_DESCRIPTION}", SqlDbType.NVarChar).Value = updatedImage.imageDescription;
                    cmd.Parameters.Add ($"@{IMAGE_SEQUENCE}", SqlDbType.NVarChar).Value = updatedImage.imageSequence;
                    cmd.Parameters.Add ($"@{IMAGE_CATEGORY_NAME}", SqlDbType.NVarChar).Value = updatedImage.imageCategoryName;
                    cmd.Parameters.Add ($"@{EXTERNAL_IMAGE_REFERENCE}", SqlDbType.NVarChar).Value = updatedImage.externalImageReference;
                    cmd.Parameters.Add ($"@{SUBMITTED_TO_NEXT}", SqlDbType.Bit).Value = updatedImage.submittedToNext;

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery ();
                }
                connection.Close ();
            }
        }

        /// <summary>
        /// Delete image by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteById (string id)
        {
            if (String.IsNullOrEmpty (id) || String.IsNullOrWhiteSpace (id))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!IdExist (id, EXTERNAL_IMAGE_REFERENCE, connection))
                {
                    connection.Close ();
                    throw new ImageDoesNotExistException ();
                }

                SqlCommand cmd = new SqlCommand ($"DELETE FROM {IMAGE_TABLE} WHERE {EXTERNAL_IMAGE_REFERENCE} = @{EXTERNAL_IMAGE_REFERENCE}", connection);
                cmd.Parameters.Add ($"@{EXTERNAL_IMAGE_REFERENCE}", SqlDbType.NVarChar).Value = id;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery ();

                connection.Close ();
            }
        }

        /// <summary>
        /// Check if order id exist
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private bool IdExist (string id, string typeofId, SqlConnection connection)
        {
            SqlCommand checkForId = new SqlCommand ($"SELECT COUNT(*) FROM {IMAGE_TABLE} WHERE {typeofId} = @{typeofId}", connection);
            checkForId.Parameters.Add ($"@{typeofId}", SqlDbType.NVarChar).Value = id;

            int orderIdExist = (int) checkForId.ExecuteScalar ();

            return orderIdExist > 0 ? true : false;
        }

        private string CreateSqlStringForAdd ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append ($"INSERT INTO {IMAGE_TABLE}(");
            sb.Append ($"{ORDER_ID},{INSTALLATION_ID},{ESTATE_ID},{EXTERNAL_IMAGE_REFERENCE},");
            sb.Append ($"{IMAGE_DESCRIPTION},{DELIVERY_PACKAGE_ID},{IMAGE_SEQUENCE},{EXTENSION},");
            sb.Append ($"{IMAGE_QUALITY},{IMAGE_CATEGORY_NAME},{IMAGE_FILE},{THUMBNAIL},");
            sb.Append ($"{FILE_NAME},{SUBMITTED_TO_NEXT})");

            sb.Append ($"VALUES (");
            sb.Append ($"@{ORDER_ID},@{INSTALLATION_ID},@{ESTATE_ID},@{EXTERNAL_IMAGE_REFERENCE},");
            sb.Append ($"@{IMAGE_DESCRIPTION},@{DELIVERY_PACKAGE_ID},@{IMAGE_SEQUENCE},@{EXTENSION},");
            sb.Append ($"@{IMAGE_QUALITY},@{IMAGE_CATEGORY_NAME},@{IMAGE_FILE},@{THUMBNAIL},");
            sb.Append ($"@{FILE_NAME},@{SUBMITTED_TO_NEXT})");

            return sb.ToString ();
        }

        private string CreateSqlStringForUpdate ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append ($"UPDATE {IMAGE_TABLE} SET ");
            sb.Append ($"{IMAGE_DESCRIPTION} = @{IMAGE_DESCRIPTION},");
            sb.Append ($"{IMAGE_SEQUENCE} = @{IMAGE_SEQUENCE},");
            sb.Append ($"{SUBMITTED_TO_NEXT} = @{SUBMITTED_TO_NEXT},");
            sb.Append ($"{IMAGE_CATEGORY_NAME} = @{IMAGE_CATEGORY_NAME} ");

            sb.Append ($"WHERE {EXTERNAL_IMAGE_REFERENCE} = @{EXTERNAL_IMAGE_REFERENCE}");

            return sb.ToString ();
        }
    }
}