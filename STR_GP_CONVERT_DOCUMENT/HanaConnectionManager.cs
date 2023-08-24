using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STR_GP_CONVERT_DOCUMENT
{
    public class HanaConnectionManager
    {
        private HanaConnection hanaConnection = null;

        public HanaConnection GetConnection()
        {
            hanaConnection = new HanaConnection(ConfigurationManager.ConnectionStrings["hana"].ConnectionString);
            return hanaConnection;
        }

        public void OpenConnection()
        {
            hanaConnection.Open();
        }

        public void CloseConnection()
        {
            hanaConnection.Close();
            hanaConnection = null;
        }
    }
}
