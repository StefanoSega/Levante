using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levante
{
    public class LevanteSync
    {
        public GenericResult UpdateRecord<T>(T document)
        {
            try
            {
                // Verifico se sono connesso
                if (!_Connected)
                {
                    GenericResult res = new GenericResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.UpdateRecord<T>(_parmConnection, document);
            }
            catch (Exception exc)
            {
                throw new Exception("UpdateRecord - " + exc.Message);
            }
        }
    }
}
