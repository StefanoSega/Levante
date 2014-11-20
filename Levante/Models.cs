using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Levante
{
    #region PARAMETERS

    public enum UpdateMode
    {
        Full,
        Partial
    }

    public enum CommandLanguage
    {
        Javascript,
        SQL
    }

    public class parmAuthentication
    {
        public String Name { get; set; }
        public String Password { get; set; }
    }

    public class parmConnection
    {
        public Server Server { get; set; }
        public parmAuthentication Authentication { get; set; }
        public String DBName { get; set; }
    }

    public class parmCommand
    {
        [JsonProperty("command-text")]
        public String CommandText { get; set; }
    }

    public class parmBatch
    {
        [JsonProperty("transaction")]
        public Boolean Transaction { get; set; }
        [JsonProperty("operations")]
        public List<parmBatchOperation> Operations { get; set; }

        public parmBatch()
        {
            Operations = new List<parmBatchOperation>();
        }
    }
    public class parmBatchOperation
    {
        [JsonProperty("type")]
        public String Type { get; set; }
        [JsonProperty("record")]
        public Object Record { get; set; }
        [JsonProperty("language")]
        public String Language { get; set; }
        [JsonProperty("command")]
        public String Command { get; set; }
        [JsonProperty("script")]
        public String Script { get; set; }
    }

    public class parmRecordRID
    {
        [JsonProperty("@rid")]
        public String Id { get; set; }
    }

    #endregion PARAMETERS

    #region RESULTS

    public enum ResultCode
    {
        OK,
        AuthError,
        GenericError,
        ParametersError,
        NotConnected
    }

    /// <summary>
    /// Risultato generico
    /// </summary>
    public class GenericResult
    {
        public ResultCode Code { get; set; }
        public String Message { get; set; }
    }
    /// <summary>
    /// Risultato di tipo Boolean
    /// </summary>
    public class BooleanResult : GenericResult
    {
        public Boolean Result { get; set; }
    }

    /// <summary>
    /// Risultato di tipo List(String)
    /// </summary>
    public class ListStringResult : GenericResult
    {
        public List<String> Result { get; set; }
    }

    /// <summary>
    /// Risultato di tipo Int32
    /// </summary>
    public class Int32Result : GenericResult
    {
        public Int32 Result { get; set; }
    }

    /// <summary>
    /// Risultato di una Query
    /// </summary>
    public class QueryResult
    {
        [JsonProperty("result")]
        public List<Object> Result { get; set; }
    }

    public class DatabasesListResult
    {
        [JsonProperty("@type")]
        public String Type { get; set; }
        [JsonProperty("@version")]
        public Int32 Version { get; set; }
        [JsonProperty("databases")]
        public String[] Databases { get; set; }
    }

    public class ClassStructResult : GenericResult
    {
        public ClassStruct Result { get; set; }
    }

    public class ClassStruct
    {
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("superClass")]
        public String SuperClass { get; set; }
        [JsonProperty("alias")]
        public String Alias { get; set; }
        [JsonProperty("abstract")]
        public Boolean Abstract { get; set; }
        [JsonProperty("strictmode")]
        public Boolean StrictMode { get; set; }
        [JsonProperty("clusters")]
        public List<Int32> Clusters { get; set; }
        [JsonProperty("defaultCluster")]
        public Int32 DefaultCluster { get; set; }
        [JsonProperty("records")]
        public Int32 Records { get; set; }
        [JsonProperty("properties")]
        public List<PropertyStruct> Properties { get; set; }
    }

    public class PropertyStruct
    {
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("linkedType")]
        public String LinkedType { get; set; }
        [JsonProperty("type")]
        public String Type { get; set; }
        [JsonProperty("mandatory")]
        public Boolean Mandatory { get; set; }
        [JsonProperty("readonly")]
        public Boolean Readonly { get; set; }
        [JsonProperty("notNull")]
        public Boolean NotNull { get; set; }
        [JsonProperty("min")]
        public Int32? Min { get; set; }
        [JsonProperty("max")]
        public Int32? Max { get; set; }
        [JsonProperty("collate")]
        public String Collate { get; set; }
    }

    #endregion RESULTS
}
