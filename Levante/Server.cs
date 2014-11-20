using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levante
{
    public class Server
    {
        public String Name { get; set; }
        public Int32? Port { get; set; }

        /// <summary>
        /// Restituisce l'elenco dei Database presenti sul Server
        /// </summary>
        /// <returns></returns>
        public ListStringResult DatabasesList()
        {
            try
            {
                ListStringResult res = new ListStringResult();
                res.Code = ResultCode.OK;

                parmConnection parmConnection = new parmConnection();
                parmConnection.Server = new Server();
                parmConnection.Server.Name = Name;
                parmConnection.Server.Port = Port;

                return Calls.DatabasesList(parmConnection);
            }
            catch (Exception exc)
            {
                throw new Exception("Query - " + exc.Message);
            }
        }
    }
}
