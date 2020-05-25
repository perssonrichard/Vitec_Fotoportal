using System.Collections.Generic;
using System.Data.SqlClient;
using Photobox.Models;
using Photobox.Models.Config;

namespace Photobox.DB {
    /// <summary>
    /// Handles all calls to the database
    /// </summary>
    public class PhotoboxDB {
        private readonly DatabaseSettings _databaseDetails;

        private readonly HandlePhotographerTable photographerTable;
        private readonly HandleImageTable imageTable;
        private readonly HandleOrderTable orderTable;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseDetails">Connection settings</param>
        public PhotoboxDB (DatabaseSettings databaseDetails) {
            _databaseDetails = databaseDetails;

            photographerTable = new HandlePhotographerTable (GetConnectionString());
            imageTable = new HandleImageTable (GetConnectionString());
            orderTable = new HandleOrderTable (GetConnectionString());
        }

        /// <summary>
        /// Save photographer to DB
        /// </summary>
        /// <param name="photographer"></param>
        public void RegisterPhotographer (Photographer photographer) => photographerTable.Register (photographer);

        /// <summary>
        /// Get photographer by email from DB
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Photographer or null</returns>
        public Photographer GetPhotographerByEmail (string email) => photographerTable.GetByEmail (email);

        /// <summary>
        /// Get all photographers from DB
        /// </summary>
        /// <returns>List of photographers</returns>
        public List<Photographer> GetAllPhotographers () => photographerTable.GetAll ();

        /// <summary>
        /// Update photographer information
        /// </summary>
        /// <param name="photographer"></param>
        public void UpdatePhotographer (Photographer photographer) => photographerTable.UpdatePhotographer (photographer);

        /// <summary>
        /// Update photographer password
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public void UpdatePhotographerPassword (string email, string hashedPassword) => photographerTable.UpdatePassword (email, hashedPassword);

        /// <summary>
        /// Update photographer Email
        /// </summary>
        /// <param name="oldEmail"></param>
        /// <param name="newEmail"></param>
        public void UpdatePhotographerEmail (string oldEmail, string newEmail) => photographerTable.UpdateEmail (oldEmail, newEmail);

        /// <summary>
        /// Delete photographer from db
        /// </summary>
        /// <param name="email"></param>
        public void DeletePhotographerByEmail (string email) => photographerTable.DeletePhotographer (email);

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order"></param>
        public void CreateOrder (Order order) => orderTable.CreateOrder (order);

        /// <summary>
        /// Get all orders on a photographer
        /// </summary>
        /// <param name="photographerEmail"></param>
        /// <returns></returns>
        public List<Order> GetPhotographerOrders (string photographerEmail) => orderTable.GetPhotographerOrders (photographerEmail);

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Order GetOrderById (string orderId) => orderTable.GetById (orderId);

        /// <summary>
        /// Update an order
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrder (Order order) => orderTable.UpdateOrder (order);

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrderStatus (string orderId, StatusType status) => orderTable.UpdateStatus (orderId, status);

        /// <summary>
        /// Update photographer on an order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="photographerEmail">Can be empty</param>
        public void UpdateOrderPhotographer (string orderId, string photographerEmail) => orderTable.UpdateEmail (orderId, photographerEmail);

        /// <summary>
        /// Delete order
        /// </summary>
        /// <param name="orderId"></param>
        public void DeleteOrder (string orderId) => orderTable.DeleteOrder (orderId);

        /// <summary>
        /// Add images to a specific order
        /// </summary>
        /// <param name="images"></param>
        public void AddImagesToOrder (Image image) => imageTable.AddToOrder (image);

        /// <summary>
        /// Get image by externalImageReference
        /// </summary>
        /// <param name="externalImageReference"></param>
        /// <returns></returns>
        // public Image GetImageById (string externalImageReference) => imageTable.GetById (externalImageReference);

        /// <summary>
        /// Get all images on a specific order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>A list of image objects</returns>
        public List<Image> GetAllImagesOnOrder (string orderId, bool fullQuality = false, bool getThumbnail = true) => imageTable.GetImagesOnOrder (orderId, fullQuality, getThumbnail);

        /// <summary>
        /// Update an image
        /// </summary>
        /// <param name="updatedImage"></param>
        public void UpdateImage (Image updatedImage) => imageTable.UpdateImage (updatedImage);

        /// <summary>
        /// Delete an image by id
        /// </summary>
        /// <param name="externalImageReference"></param>
        public void DeleteImageById (string externalImageReference) => imageTable.DeleteById (externalImageReference);

        /// <summary>
        /// Returns a connection string for an SQL Database
        /// </summary>
        private string GetConnectionString () {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder ();
            builder.DataSource = _databaseDetails.serverName;
            builder.UserID = _databaseDetails.username;
            builder.Password = _databaseDetails.password;
            builder.InitialCatalog = _databaseDetails.databaseName;

            return builder.ConnectionString;
        }

        /// <summary>
        /// Connect to the database
        /// </summary>
        private SqlConnection Connect () => new SqlConnection (GetConnectionString ());
    }
}