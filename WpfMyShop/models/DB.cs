using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMyShop.model
{
    public class DB
    {
        private static DB? _instance = null;
        private SqlConnection _connection = null;

        public SqlConnection? Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(ConnectionString); ;
                    _connection.Open();
                }

                return _connection;
            }
        }
        public string ConnectionString { get; set; } = "";

        public static DB Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DB();
                }

                return _instance;
            }
        }
    }
}
