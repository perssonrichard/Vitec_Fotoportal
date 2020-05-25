using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Photobox.CustomExceptions;
using Photobox.Models;

namespace Photobox.DB
{
    /// <summary>
    /// Does queries against the photographer table
    /// </summary>
    public class HandlePhotographerTable
    {
        // Table constants
        private const string PHOTOGRAPHER_TABLE = "Photographer";
        private const string EMAIL = "email";
        private const string FIRST_NAME = "firstName";
        private const string LAST_NAME = "lastName";
        private const string HASHED_PASSWORD = "hashedPassword";
        private const string CELL_PHONE_NUMBER = "cellPhoneNumber";
        private const string ADDRESS = "address";
        private const string COMPANY = "company";
        private const string ORGNR = "orgNr";
        private const string CITY = "city";
        private const string POSTAL_CODE = "postalCode";
        private const string POSTAL_CODE_AREA = "postalCodeArea";
        private const string ORDER_ID_LIST = "orderIdList";
        private const string AVAILABLE = "available";

        private string connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="con">An SQL connection</param>
        public HandlePhotographerTable (string conString) => connectionString = conString;

        /// <summary>
        /// Save photographer to DB
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="photographer"></param>
        public void Register (Photographer photographer)
        {
            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (PhotographerExist (photographer.email, connection))
                {
                    connection.Close ();
                    throw new UserAlreadyExistException ();
                };

                string sql = CreateSqlStringForRegister ();

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    cmd.Parameters.Add ($"@{EMAIL}", SqlDbType.NVarChar).Value = photographer.email;
                    cmd.Parameters.Add ($"@{FIRST_NAME}", SqlDbType.NVarChar).Value = photographer.firstName;
                    cmd.Parameters.Add ($"@{LAST_NAME}", SqlDbType.NVarChar).Value = photographer.lastName;
                    cmd.Parameters.Add ($"@{HASHED_PASSWORD}", SqlDbType.NVarChar).Value = photographer.hashedPassword;
                    cmd.Parameters.Add ($"@{CELL_PHONE_NUMBER}", SqlDbType.NVarChar).Value = photographer.cellPhoneNumber;
                    cmd.Parameters.Add ($"@{ADDRESS}", SqlDbType.NVarChar).Value = photographer.address;
                    cmd.Parameters.Add ($"@{COMPANY}", SqlDbType.NVarChar).Value = photographer.company;
                    cmd.Parameters.Add ($"@{ORGNR}", SqlDbType.NVarChar).Value = photographer.orgNr;
                    cmd.Parameters.Add ($"@{CITY}", SqlDbType.NVarChar).Value = photographer.city;
                    cmd.Parameters.Add ($"@{POSTAL_CODE}", SqlDbType.NVarChar).Value = photographer.postalCode;
                    cmd.Parameters.Add ($"@{POSTAL_CODE_AREA}", SqlDbType.NVarChar).Value = photographer.postalCodeArea;
                    cmd.Parameters.Add ($"@{AVAILABLE}", SqlDbType.Bit).Value = photographer.available;

                    // String separated by commas. "1,2,3"
                    string joinedList = String.Join (",", photographer.orderIdList);

                    cmd.Parameters.Add ($"@{ORDER_ID_LIST}", SqlDbType.NVarChar).Value = joinedList;

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery ();
                }
                connection.Close ();
            }
        }

        /// <summary>
        /// Get photographer by email from DB
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Photographer GetByEmail (string email)
        {
            if (String.IsNullOrEmpty (email) || String.IsNullOrWhiteSpace (email))
                throw new ArgumentNullException ();

            Photographer photographer = new Photographer ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!PhotographerExist (email, connection))
                {
                    connection.Close ();
                    return null;
                }

                string sql = $"SELECT * FROM {PHOTOGRAPHER_TABLE} WHERE {EMAIL}=@{EMAIL}";

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    cmd.Parameters.Add ($"@{EMAIL}", SqlDbType.NVarChar).Value = email;

                    using (SqlDataReader reader = cmd.ExecuteReader ())
                    {
                        while (reader.Read ())
                        {
                            photographer.email = reader[EMAIL].ToString ();
                            photographer.firstName = reader[FIRST_NAME].ToString ();
                            photographer.lastName = reader[LAST_NAME].ToString ();
                            photographer.hashedPassword = reader[HASHED_PASSWORD].ToString ();
                            photographer.cellPhoneNumber = reader[CELL_PHONE_NUMBER].ToString ();
                            photographer.address = reader[ADDRESS].ToString ();
                            photographer.company = reader[COMPANY].ToString ();
                            photographer.orgNr = reader[ORGNR].ToString ();
                            photographer.city = reader[CITY].ToString ();
                            photographer.postalCode = reader.GetInt32 (POSTAL_CODE);
                            photographer.postalCodeArea = reader[POSTAL_CODE_AREA].ToString ();
                            photographer.available = reader.GetBoolean (AVAILABLE);
                            // Commma separated string to List. "1,2,3"
                            List<string> orderIdList = reader[ORDER_ID_LIST].ToString ().Split (",").ToList ();
                            photographer.orderIdList = orderIdList;
                        }
                    }
                }
                connection.Close ();
                return photographer;
            }
        }

        /// <summary>
        /// Get all photographers from DB
        /// </summary>
        /// <returns>List of photographers</returns>
        public List<Photographer> GetAll ()
        {
            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();
                List<Photographer> photographers = new List<Photographer> ();

                string sql = $"SELECT * FROM {PHOTOGRAPHER_TABLE}";

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader ())
                    {
                        while (reader.Read ())
                        {
                            var photographer = new Photographer ();

                            photographer.email = reader[EMAIL].ToString ();
                            photographer.firstName = reader[FIRST_NAME].ToString ();
                            photographer.lastName = reader[LAST_NAME].ToString ();
                            photographer.hashedPassword = reader[HASHED_PASSWORD].ToString ();
                            photographer.cellPhoneNumber = reader[CELL_PHONE_NUMBER].ToString ();
                            photographer.address = reader[ADDRESS].ToString ();
                            photographer.company = reader[COMPANY].ToString ();
                            photographer.orgNr = reader[ORGNR].ToString ();
                            photographer.city = reader[CITY].ToString ();
                            photographer.postalCode = reader.GetInt32 (POSTAL_CODE);
                            photographer.postalCodeArea = reader[POSTAL_CODE_AREA].ToString ();
                            photographer.available = reader.GetBoolean (AVAILABLE);
                            // Commma separated string to List. "1,2,3"
                            List<string> orderIdList = reader[ORDER_ID_LIST].ToString ().Split (",").ToList ();
                            photographer.orderIdList = orderIdList;

                            photographers.Add (photographer);
                        }
                        connection.Close ();

                        return photographers;
                    }
                }
            }
        }

        /// <summary>
        /// Update photographer
        /// </summary>
        /// <param name="updatedPhotographer"></param>
        public void UpdatePhotographer (Photographer updatedPhotographer)
        {
            if (String.IsNullOrEmpty (updatedPhotographer.email) || String.IsNullOrWhiteSpace (updatedPhotographer.email))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!PhotographerExist (updatedPhotographer.email, connection))
                {
                    connection.Close ();
                    throw new UserDoesNotExistException ();
                }

                string sql = CreateSqlStringForUpdate ();

                using (SqlCommand cmd = new SqlCommand (sql, connection))
                {
                    cmd.Parameters.Add ($"@{FIRST_NAME}", SqlDbType.NVarChar).Value = updatedPhotographer.firstName;
                    cmd.Parameters.Add ($"@{LAST_NAME}", SqlDbType.NVarChar).Value = updatedPhotographer.lastName;
                    cmd.Parameters.Add ($"@{CELL_PHONE_NUMBER}", SqlDbType.NVarChar).Value = updatedPhotographer.cellPhoneNumber;
                    cmd.Parameters.Add ($"@{ORGNR}", SqlDbType.NVarChar).Value = updatedPhotographer.orgNr;
                    cmd.Parameters.Add ($"@{ADDRESS}", SqlDbType.NVarChar).Value = updatedPhotographer.address;
                    cmd.Parameters.Add ($"@{COMPANY}", SqlDbType.NVarChar).Value = updatedPhotographer.company;
                    cmd.Parameters.Add ($"@{CITY}", SqlDbType.NVarChar).Value = updatedPhotographer.city;
                    cmd.Parameters.Add ($"@{POSTAL_CODE}", SqlDbType.NVarChar).Value = updatedPhotographer.postalCode;
                    cmd.Parameters.Add ($"@{POSTAL_CODE_AREA}", SqlDbType.NVarChar).Value = updatedPhotographer.postalCodeArea;
                    cmd.Parameters.Add ($"@{AVAILABLE}", SqlDbType.NVarChar).Value = updatedPhotographer.available;
                    cmd.Parameters.Add ($"@{EMAIL}", SqlDbType.NVarChar).Value = updatedPhotographer.email;

                    // String separated by commas. "1,2,3"
                    string joinedList = String.Join (",", updatedPhotographer.orderIdList);

                    cmd.Parameters.Add ($"@{ORDER_ID_LIST}", SqlDbType.NVarChar).Value = joinedList;

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery ();
                }
                connection.Close ();
            }
        }

        /// <summary>
        /// Update photographer password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="hashedPw"></param>
        public void UpdatePassword (string email, string hashedPassword)
        {
            if (String.IsNullOrEmpty (email) || String.IsNullOrWhiteSpace (hashedPassword))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!PhotographerExist (email, connection))
                {
                    connection.Close ();
                    throw new UserAlreadyExistException ();
                }

                string sql = $"UPDATE {PHOTOGRAPHER_TABLE} SET {HASHED_PASSWORD} = @{HASHED_PASSWORD} WHERE {EMAIL} = @{EMAIL}";

                SqlCommand cmd = new SqlCommand (sql, connection);
                cmd.Parameters.Add ($"@{HASHED_PASSWORD}", SqlDbType.NVarChar).Value = hashedPassword;
                cmd.Parameters.Add ($"@{EMAIL}", SqlDbType.NVarChar).Value = email;
                cmd.ExecuteNonQuery ();

                connection.Close ();
            }
        }

        /// <summary>
        /// Update photographer email
        /// </summary>
        /// <param name="oldEmail"></param>
        /// <param name="newEmail"></param>
        public void UpdateEmail (string oldEmail, string newEmail)
        {
            if (String.IsNullOrEmpty (oldEmail) || String.IsNullOrWhiteSpace (newEmail))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (!PhotographerExist (oldEmail, connection))
                {
                    connection.Close ();
                    throw new UserDoesNotExistException();
                }

                string pNew = "new";
                string pOld = "old";
                string sql = $"UPDATE {PHOTOGRAPHER_TABLE} SET {EMAIL} = @{pNew} WHERE {EMAIL} = @{pOld}";

                SqlCommand cmd = new SqlCommand (sql, connection);
                cmd.Parameters.Add ($"@{pNew}", SqlDbType.NVarChar).Value = newEmail;
                cmd.Parameters.Add ($"@{pOld}", SqlDbType.NVarChar).Value = oldEmail;
                cmd.ExecuteNonQuery ();

                connection.Close ();
            }
        }

        /// <summary>
        /// Delete photographer
        /// </summary>
        /// <param name="email"></param>
        public void DeletePhotographer (string email)
        {
            if (String.IsNullOrEmpty (email) || String.IsNullOrWhiteSpace (email))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();

                if (PhotographerExist (email, connection))
                {
                    SqlCommand cmd = new SqlCommand ($"DELETE FROM {PHOTOGRAPHER_TABLE} WHERE {EMAIL} = @{EMAIL};", connection);
                    cmd.Parameters.Add ($"@{EMAIL}", SqlDbType.NVarChar).Value = email;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery ();
                }
                connection.Close ();
            }
        }

        /// <summary>
        /// Check if a photographer exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Bool</returns>
        private bool PhotographerExist (string email, SqlConnection connection)
        {
            SqlCommand checkForUser = new SqlCommand ($"SELECT COUNT(*) FROM {PHOTOGRAPHER_TABLE} WHERE {EMAIL} = @{EMAIL};", connection);
            checkForUser.Parameters.Add ($"@{EMAIL}", SqlDbType.NVarChar).Value = email;
            int userExist = (int) checkForUser.ExecuteScalar ();

            return userExist > 0 ? true : false;
        }

        private string CreateSqlStringForRegister ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append ($"INSERT INTO {PHOTOGRAPHER_TABLE}(");
            sb.Append ($"{EMAIL},{FIRST_NAME},{LAST_NAME},{HASHED_PASSWORD},");
            sb.Append ($"{CELL_PHONE_NUMBER},{ADDRESS},{COMPANY},{ORGNR},");
            sb.Append ($"{CITY},{POSTAL_CODE},{POSTAL_CODE_AREA},{ORDER_ID_LIST},{AVAILABLE}) ");

            sb.Append ($"VALUES (@{EMAIL},@{FIRST_NAME},@{LAST_NAME},@{HASHED_PASSWORD},");
            sb.Append ($"@{CELL_PHONE_NUMBER},@{ADDRESS},@{COMPANY},@{ORGNR},");
            sb.Append ($"@{CITY},@{POSTAL_CODE},@{POSTAL_CODE_AREA},@{ORDER_ID_LIST},@{AVAILABLE});");

            return sb.ToString ();
        }

        private string CreateSqlStringForUpdate ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append ($"UPDATE {PHOTOGRAPHER_TABLE} SET ");
            sb.Append ($"{FIRST_NAME} = @{FIRST_NAME},");
            sb.Append ($"{LAST_NAME} = @{LAST_NAME},");
            sb.Append ($"{CELL_PHONE_NUMBER} = @{CELL_PHONE_NUMBER},");
            sb.Append ($"{ORGNR} = @{ORGNR},");
            sb.Append ($"{ADDRESS} = @{ADDRESS},");
            sb.Append ($"{COMPANY} = @{COMPANY},");
            sb.Append ($"{CITY} = @{CITY},");
            sb.Append ($"{POSTAL_CODE} = @{POSTAL_CODE},");
            sb.Append ($"{POSTAL_CODE_AREA} = @{POSTAL_CODE_AREA},");
            sb.Append ($"{ORDER_ID_LIST} = @{ORDER_ID_LIST},");
            sb.Append ($"{AVAILABLE} = @{AVAILABLE} ");

            sb.Append ($"WHERE {EMAIL} = @{EMAIL}");

            return sb.ToString ();
        }
    }
}