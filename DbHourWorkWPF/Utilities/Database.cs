using MySqlConnector;

namespace DBHourWorkWPF.Utilities
{
    public class Database
    {
        public MySqlConnection connection;
        public MySqlConnectionStringBuilder stringConnection;
        public Database(MySqlConnection connection, MySqlConnectionStringBuilder stringConnection)
        {
            this.connection = connection;
            this.stringConnection = stringConnection;
        }

        public Database(MySqlConnection connection, string server, int port, string nameDb, string userId, string pass)
        {
            this.connection = connection;
            stringConnection = new MySqlConnectionStringBuilder($"Server={server};Port={port};User ID={userId};Database={nameDb};pass={pass};Allow Zero DateTime=True");
        }
    }
}
