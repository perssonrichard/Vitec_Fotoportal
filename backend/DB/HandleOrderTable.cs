using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Photobox.CustomExceptions;
using Photobox.Models;

namespace Photobox.DB {
    /// <summary>
    /// Does queries against the order table
    /// </summary>
    public class HandleOrderTable {
        // Table constants
        private const string ORDER_TABLE = "Order_Table";
        private const string ORDER_ID = "orderId";
        private const string INSTALLATION_ID = "installationId";
        private const string ESTATE_ID = "estateId";
        private const string DEPT_ID = "deptId";
        private const string USER_ID = "userId";
        private const string USER_EMAIL = "userEmail";
        private const string USERNAME = "username";
        private const string DESCRIPTION = "description";
        private const string ADDRESS = "address";
        private const string CITY = "city";
        private const string POSTAL_CODE = "postalCode";
        private const string STATUS = "status";
        private const string REG_DATE = "regDate";
        private const string ARCHIVE_DATE = "archiveDate";
        private const string PHOTOGRAPHER_EMAIL = "photographerEmail";

        private string connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="con">SQL connection</param>
        public HandleOrderTable (string conString) => connectionString = conString;

        /// <summary>
        /// Create order
        /// </summary>
        /// <param name="order"></param>
        public void CreateOrder (Order order) {
            using (SqlConnection connection = new SqlConnection (connectionString)) {
                connection.Open ();

                if (OrderExist (order.orderId, connection)) {
                    connection.Close ();
                    throw new OrderAlreadyExistException ();
                }

                string sql = CreateSqlStringForCreate ();

                using (SqlCommand cmd = new SqlCommand (sql, connection)) {
                    cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = order.orderId;
                    cmd.Parameters.Add ($"@{INSTALLATION_ID}", SqlDbType.NVarChar).Value = order.installationId;
                    cmd.Parameters.Add ($"@{ESTATE_ID}", SqlDbType.NVarChar).Value = order.estateId;
                    cmd.Parameters.Add ($"@{DEPT_ID}", SqlDbType.NVarChar).Value = order.deptId;
                    cmd.Parameters.Add ($"@{USER_ID}", SqlDbType.NVarChar).Value = order.userId;
                    cmd.Parameters.Add ($"@{USER_EMAIL}", SqlDbType.NVarChar).Value = order.userEmail;
                    cmd.Parameters.Add ($"@{USERNAME}", SqlDbType.NVarChar).Value = order.username;
                    cmd.Parameters.Add ($"@{ADDRESS}", SqlDbType.NVarChar).Value = order.address;
                    cmd.Parameters.Add ($"@{CITY}", SqlDbType.NVarChar).Value = order.city;
                    cmd.Parameters.Add ($"@{POSTAL_CODE}", SqlDbType.NVarChar).Value = order.postalCode;
                    cmd.Parameters.Add ($"@{STATUS}", SqlDbType.NVarChar).Value = order.status;
                    cmd.Parameters.Add ($"@{REG_DATE}", SqlDbType.DateTime).Value = order.regDate;

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery ();
                }
                connection.Close ();
            }
        }

        /// <summary>
        /// Get all orders on a photographer
        /// </summary>
        /// <param name="photographerEmail"></param>
        /// <returns>List of orders</returns>
        public List<Order> GetPhotographerOrders (string photographerEmail) {
            if (String.IsNullOrEmpty (photographerEmail) || String.IsNullOrWhiteSpace (photographerEmail))
                throw new ArgumentNullException ();

            List<Order> orders = new List<Order> ();

            using (SqlConnection connection = new SqlConnection (connectionString)) {
                connection.Open ();

                string sql = $"SELECT * FROM {ORDER_TABLE} WHERE {PHOTOGRAPHER_EMAIL}=@{PHOTOGRAPHER_EMAIL};";

                using (SqlCommand cmd = new SqlCommand (sql, connection)) {
                    cmd.Parameters.Add ($"@{PHOTOGRAPHER_EMAIL}", SqlDbType.NVarChar).Value = photographerEmail;

                    using (SqlDataReader reader = cmd.ExecuteReader ()) {
                        while (reader.Read ()) {
                            Order order = new Order ();

                            order.orderId = reader[ORDER_ID].ToString ();
                            order.installationId = reader[INSTALLATION_ID].ToString ();
                            order.estateId = reader[ESTATE_ID].ToString ();
                            order.deptId = reader[DEPT_ID].ToString ();
                            order.userId = reader[USER_ID].ToString ();
                            order.userEmail = reader[USER_EMAIL].ToString ();
                            order.username = reader[USERNAME].ToString ();
                            order.description = reader[DESCRIPTION].ToString ();
                            order.address = reader[ADDRESS].ToString ();
                            order.city = reader[CITY].ToString ();
                            order.postalCode = (int) reader[POSTAL_CODE];
                            order.status = (StatusType) Enum.Parse (typeof (StatusType), reader[STATUS].ToString ());
                            order.regDate = (DateTime) reader[REG_DATE];

                            if (reader[ARCHIVE_DATE].ToString () != "")
                                order.archiveDate = (DateTime) reader[ARCHIVE_DATE];

                            order.photographerEmail = reader[PHOTOGRAPHER_EMAIL].ToString ();

                            orders.Add (order);
                        }
                        connection.Close ();

                        return orders;
                    }
                }
            }
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order GetById (string id) {
            if (String.IsNullOrEmpty (id) || String.IsNullOrWhiteSpace (id))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString)) {
                connection.Open ();

                if (!OrderExist (id, connection)) {
                    connection.Close ();
                    return null;
                }

                var sql = $"SELECT * FROM {ORDER_TABLE} WHERE {ORDER_ID} = @{ORDER_ID};";

                using (SqlCommand cmd = new SqlCommand (sql, connection)) {
                    cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = id;
                    Order order = new Order ();

                    using (SqlDataReader reader = cmd.ExecuteReader ()) {
                        while (reader.Read ()) {
                            order.orderId = reader[ORDER_ID].ToString ();
                            order.installationId = reader[INSTALLATION_ID].ToString ();
                            order.estateId = reader[ESTATE_ID].ToString ();
                            order.deptId = reader[DEPT_ID].ToString ();
                            order.userId = reader[USER_ID].ToString ();
                            order.userEmail = reader[USER_EMAIL].ToString ();
                            order.username = reader[USERNAME].ToString ();
                            order.description = reader[DESCRIPTION].ToString ();
                            order.address = reader[ADDRESS].ToString ();
                            order.city = reader[CITY].ToString ();
                            order.postalCode = (int) reader[POSTAL_CODE];
                            order.status = (StatusType) Enum.Parse (typeof (StatusType), reader[STATUS].ToString ());
                            order.regDate = (DateTime) reader[REG_DATE];

                            if (reader[ARCHIVE_DATE].ToString () != "")
                                order.archiveDate = (DateTime) reader[ARCHIVE_DATE];

                            order.photographerEmail = reader[PHOTOGRAPHER_EMAIL].ToString ();
                        }
                        connection.Close ();

                        return order;
                    }
                }
            }
        }

        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrder (Order order) {
            if (String.IsNullOrEmpty (order.orderId) || String.IsNullOrWhiteSpace (order.orderId))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString)) {
                connection.Open ();

                if (!OrderExist (order.orderId, connection)) {
                    connection.Close ();
                    throw new OrderDoesNotExistException ();
                }

                string sql = CreateSqlStringForUpdate ();

                using (SqlCommand cmd = new SqlCommand (sql, connection)) {
                    cmd.Parameters.Add ($"@{INSTALLATION_ID}", SqlDbType.NVarChar).Value = order.installationId;
                    cmd.Parameters.Add ($"@{ESTATE_ID}", SqlDbType.NVarChar).Value = order.estateId;
                    cmd.Parameters.Add ($"@{DEPT_ID}", SqlDbType.NVarChar).Value = order.deptId;
                    cmd.Parameters.Add ($"@{USER_ID}", SqlDbType.NVarChar).Value = order.userId;
                    cmd.Parameters.Add ($"@{USER_EMAIL}", SqlDbType.NVarChar).Value = order.userEmail;
                    cmd.Parameters.Add ($"@{USERNAME}", SqlDbType.NVarChar).Value = order.username;
                    cmd.Parameters.Add ($"@{DESCRIPTION}", SqlDbType.NVarChar).Value = order.description;
                    cmd.Parameters.Add ($"@{ADDRESS}", SqlDbType.NVarChar).Value = order.address;
                    cmd.Parameters.Add ($"@{CITY}", SqlDbType.NVarChar).Value = order.city;
                    cmd.Parameters.Add ($"@{POSTAL_CODE}", SqlDbType.NVarChar).Value = order.postalCode;
                    cmd.Parameters.Add ($"@{STATUS}", SqlDbType.NVarChar).Value = order.status;
                    cmd.Parameters.Add ($"@{REG_DATE}", SqlDbType.DateTime).Value = order.regDate;
                    if (order.archiveDate.ToString () == "0001-01-01 00:00:00") {
                        cmd.Parameters.Add ($"@{ARCHIVE_DATE}", SqlDbType.DateTime).Value = new DateTime(1970, 1, 1);
                    } else {
                        cmd.Parameters.Add ($"@{ARCHIVE_DATE}", SqlDbType.DateTime).Value = order.archiveDate;
                     }
                    cmd.Parameters.Add ($"@{PHOTOGRAPHER_EMAIL}", SqlDbType.NVarChar).Value = order.photographerEmail;
                    cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = order.orderId;

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery ();
                }
                connection.Close ();
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        public void UpdateStatus (string orderId, StatusType status) {
            if (String.IsNullOrEmpty (orderId) || String.IsNullOrWhiteSpace (orderId))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString)) {
                connection.Open ();

                if (!OrderExist (orderId, connection)) {
                    connection.Close ();
                    throw new OrderDoesNotExistException ();
                }

                string sql = $"UPDATE {ORDER_TABLE} SET {STATUS} = @{STATUS} WHERE {ORDER_ID} = @{ORDER_ID}";

                SqlCommand cmd = new SqlCommand (sql, connection);
                cmd.Parameters.Add ($"@{STATUS}", SqlDbType.NVarChar).Value = status;
                cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = orderId;
                cmd.ExecuteNonQuery ();

                connection.Close ();
            }
        }

        /// <summary>
        /// Update photographer
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="photographerEmail"></param>
        public void UpdateEmail (string orderId, string photographerEmail) {
            if (String.IsNullOrEmpty (orderId) || String.IsNullOrWhiteSpace (orderId))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString)) {
                connection.Open ();

                if (!OrderExist (orderId, connection)) {
                    connection.Close ();
                    throw new OrderDoesNotExistException ();
                }

                string sql = $"UPDATE {ORDER_TABLE} SET {PHOTOGRAPHER_EMAIL} = @{PHOTOGRAPHER_EMAIL} WHERE {ORDER_ID} = @{ORDER_ID}";

                SqlCommand cmd = new SqlCommand (sql, connection);
                cmd.Parameters.Add ($"@{PHOTOGRAPHER_EMAIL}", SqlDbType.NVarChar).Value = photographerEmail;
                cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = orderId;
                cmd.ExecuteNonQuery ();

                connection.Close ();
            }
        }

        /// <summary>
        /// Delete an order from db
        /// </summary>
        /// <param name="id"></param>
        public void DeleteOrder (string id) {
            if (String.IsNullOrEmpty (id) || String.IsNullOrWhiteSpace (id))
                throw new ArgumentNullException ();

            using (SqlConnection connection = new SqlConnection (connectionString)) {
                connection.Open ();

                if (!OrderExist (id, connection)) {
                    connection.Close ();
                    throw new OrderDoesNotExistException ();
                }

                SqlCommand cmd = new SqlCommand ($"DELETE FROM {ORDER_TABLE} WHERE {ORDER_ID} = @{ORDER_ID}", connection);
                cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = id;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery ();
                connection.Close ();
            }
        }

        /// <summary>
        /// Check if order exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool OrderExist (string id, SqlConnection connection) {
            SqlCommand cmd = new SqlCommand ($"SELECT COUNT(*) FROM {ORDER_TABLE} WHERE {ORDER_ID} = @{ORDER_ID}", connection);
            cmd.Parameters.Add ($"@{ORDER_ID}", SqlDbType.NVarChar).Value = id;
            int orderExist = (int) cmd.ExecuteScalar ();

            return orderExist > 0 ? true : false;
        }

        private string CreateSqlStringForCreate () {
            StringBuilder sb = new StringBuilder ();

            sb.Append ($"INSERT INTO {ORDER_TABLE}(");
            sb.Append ($"{ORDER_ID},{INSTALLATION_ID},{ESTATE_ID},{DEPT_ID},");
            sb.Append ($"{USER_ID},{USER_EMAIL},{USERNAME},{ADDRESS},{CITY},{POSTAL_CODE},");
            sb.Append ($"{STATUS},{REG_DATE}) ");

            sb.Append ($"VALUES (@{ORDER_ID},@{INSTALLATION_ID},@{ESTATE_ID},@{DEPT_ID},");
            sb.Append ($"@{USER_ID},@{USER_EMAIL},@{USERNAME},@{ADDRESS},@{CITY},@{POSTAL_CODE},");
            sb.Append ($"@{STATUS},@{REG_DATE});");

            return sb.ToString ();
        }

        private string CreateSqlStringForUpdate () {
            StringBuilder sb = new StringBuilder ();

            sb.Append ($"UPDATE {ORDER_TABLE} SET ");

            sb.Append ($"{INSTALLATION_ID} = @{INSTALLATION_ID},");
            sb.Append ($"{ESTATE_ID} = @{ESTATE_ID},");
            sb.Append ($"{DEPT_ID} = @{DEPT_ID},");
            sb.Append ($"{USER_ID} = @{USER_ID},");
            sb.Append ($"{USER_EMAIL} = @{USER_EMAIL},");
            sb.Append ($"{USERNAME} = @{USERNAME},");
            sb.Append ($"{DESCRIPTION} = @{DESCRIPTION},");
            sb.Append ($"{ADDRESS} = @{ADDRESS},");
            sb.Append ($"{CITY} = @{CITY},");
            sb.Append ($"{POSTAL_CODE} = @{POSTAL_CODE},");
            sb.Append ($"{STATUS} = @{STATUS},");
            sb.Append ($"{REG_DATE} = @{REG_DATE},");
            sb.Append ($"{ARCHIVE_DATE} = @{ARCHIVE_DATE},");
            sb.Append ($"{PHOTOGRAPHER_EMAIL} = @{PHOTOGRAPHER_EMAIL} ");

            sb.Append ($"WHERE {ORDER_ID} = @{ORDER_ID}");

            return sb.ToString ();
        }
    }
}