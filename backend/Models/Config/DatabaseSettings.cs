namespace Photobox.Models.Config {
    /// <summary>
    /// Connection settings for SQL DB
    /// </summary>
    public class DatabaseSettings {
        private string _serverName;
        private string _username;
        private string _password;
        private string _databaseName;
        public string serverName { get => _serverName; set => _serverName = value; }
        public string username { get => _username; set => _username = value; }
        public string password { get => _password; set => _password = value; }
        public string databaseName { get => _databaseName; set => _databaseName = value; }
    }
}