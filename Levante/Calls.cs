using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Levante
{
    /// <summary>
    /// Class for the RESTful calls
    /// All the methods are statics
    /// </summary>
    class Calls
    {
        // Name or IP Address of the Server to call
        private static String _ServerName;
        public String ServerName
        {
            get
            {
                return _ServerName;
            }
            set
            {
                _ServerName = value;
            }
        }

        // Port Number of the Server to call
        private static Int32? _ServerPort;
        public Int32? ServerPort
        {
            get
            {
                return _ServerPort;
            }
            set
            {
                _ServerPort = value;
            }
        }

        /// <summary>
        /// Create a new connection to the database
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <returns>Feedback of the operation</returns>
        public static GenericResult ConnectToODB(parmConnection Connection)
        {
            try
            {
                GenericResult res = new GenericResult();
                res.Code = ResultCode.OK;

                try
                {
                    String resStr = Utility.CallWS<String>(Connection
                                                    , null
                                                    , "connect/" + Connection.DBName
                                                    , "GET") as String;
                }
                catch (Exception exc)
                {
                    if (exc.Message.Contains("401"))
                    {
                        res.Code = ResultCode.AuthError;
                        res.Message = "Authentication Error: " + exc.Message;
                    }
                    else
                    {
                        res.Code = ResultCode.GenericError;
                        res.Message = exc.Message;
                    }
                }

                return res;
            }
            catch (Exception exc)
            {
                throw new Exception("ConnectToODB - " + exc.Message);
            }
        }

        /// <summary>
        /// Force disconnection from the database
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <returns>Feedback of the operation</returns>
        public static GenericResult DisconnectFromODB(parmConnection Connection)
        {
            try
            {
                GenericResult res = new GenericResult();
                res.Code = ResultCode.OK;

                // Disconnection doesn't need authentication
                Connection.Authentication = null;

                try
                {
                    String resStr = Utility.CallWS<String>(Connection
                                                    , null
                                                    , "disconnect"
                                                    , "GET") as String;
                }
                catch (Exception exc)
                {
                    if (exc.Message.Contains("401"))
                    {
                        // I can expect a 401 error from certain browsers, I can ignore it
                    }
                    else
                    {
                        res.Code = ResultCode.GenericError;
                        res.Message = exc.Message;
                    }
                }

                return res;
            }
            catch (Exception exc)
            {
                throw new Exception("DisconnectFromODB - " + exc.Message);
            }
        }

        /// <summary>
        /// Get a document given its RID
        /// </summary>
        /// <typeparam name="T">Expected class/type of the document</typeparam>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="RID">Record Id</param>
        /// <returns>Document found</returns>
        public static Object GetDocumentById<T>(parmConnection Connection, String RID)
        {
            return Utility.CallWS<T>(Connection
                                , null
                                , "document/" + Connection.DBName + "/" + RID
                                , "GET");
        }

        /// <summary>
        /// Check if a document exists in the database
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="RID">Record Id</param>
        /// <returns>True if the document exists</returns>
        public static Boolean DocumentExists(parmConnection Connection, String RID)
        {
            // L'eccezione di record non trovato viene gestita ad un livello superiore
            Utility.CallWS<String>(Connection
                                , null
                                , "document/" + Connection.DBName + "/" + RID
                                , "HEAD");

            return true;
        }

        /// <summary>
        /// Given a Query (e.g.: SELECT) returns the result
        /// </summary>
        /// <typeparam name="T">Expected class/type of the documents returned</typeparam>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="SqlQuery">SQL command for the query</param>
        /// <param name="Limit">Limit number of the returned documents</param>
        /// <param name="FetchPlan">Fetching level for the nested classes; N=maximum level specified, 0=current document, -1=all the levels, -2=excluded</param>
        /// <returns>List<T> of the documents founded with the query</returns>
        public static List<T> Query<T>(parmConnection Connection, String SqlQuery, Int32 Limit, Int32 FetchPlan)
        {
            QueryResult res = Utility.CallWS<QueryResult>(Connection
                                , null
                                , "query/" + Connection.DBName + "/sql/" + SqlQuery + "/" + Limit + "/*:" + FetchPlan
                                , "GET") as QueryResult;

            // I've to cast all the documents of the list
            List<T> listres = new List<T>();
            foreach (Object elem in res.Result)
            {
                T elem_casted = JsonHelper.DeserializeObjectFromString<T>(elem.ToString());
                listres.Add(elem_casted);
            }

            return listres;
        }

        /// <summary>
        /// Execute a SQL Command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <typeparam name="T">Expected class/type of the documents returned</typeparam>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="SqlCommand">SQL command to execute</param>
        /// <returns>List<T> of the documents affected by the command, or GenericResult of the exception</returns>
        public static Object Command<T>(parmConnection Connection, String SqlCommand)
        {
            GenericResult res = new GenericResult();
            res.Code = ResultCode.OK;

            try
            {
                QueryResult cmdres = Utility.CallWS<QueryResult>(Connection
                                        , SqlCommand
                                        , "command/" + Connection.DBName + "/sql"
                                        , "POST") as QueryResult;

                // I've to cast all the documents of the list
                List<T> listres = new List<T>();
                foreach (Object elem in cmdres.Result)
                {
                    T elem_casted = JsonHelper.DeserializeObjectFromString<T>(elem.ToString());
                    listres.Add(elem_casted);
                }

                return listres;
            }
            catch (Exception exc)
            {
                res.Code = ResultCode.GenericError;
                res.Message = exc.Message;
            }

            return res;
        }

        /// <summary>
        /// Delete a document given its RID
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="RID">Record Id</param>
        /// <returns>Feedback of the operation</returns>
        public static GenericResult DeleteDocumentById(parmConnection Connection, String RID)
        {
            GenericResult res = new GenericResult();
            res.Code = ResultCode.OK;

            try
            {
                Utility.CallWS<String>(Connection
                                    , null
                                    , "document/" + Connection.DBName + "/" + RID
                                    , "DELETE");
            }
            catch (Exception exc)
            {
                res.Code = ResultCode.GenericError;
                res.Message = exc.Message;
            }

            return res;
        }

        /// <summary>
        /// Create a new document in the database for the specified class
        /// </summary>
        /// <typeparam name="T">Expected class/type of the document to insert</typeparam>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="document">Document to insert in the database</param>
        /// <param name="document_res">Document created with its RID</param>
        /// <returns>Feedback of the operation</returns>
        public static GenericResult InsertDocument<T>(parmConnection Connection, T document, out T document_res)
        {
            GenericResult res = new GenericResult();
            res.Code = ResultCode.OK;

            document_res = document;

            try
            {
                document_res = (T)Utility.CallWS<T>(Connection
                                    , JsonHelper.SerializeObjectToString(document)
                                    , "document/" + Connection.DBName
                                    , "POST");
            }
            catch (Exception exc)
            {
                res.Code = ResultCode.GenericError;
                res.Message = exc.Message;
            }

            return res;
        }
        public static GenericResult InsertDocument<T>(parmConnection Connection, T document)
        {
            T document_res = document;
            return InsertDocument<T>(Connection, document, out document_res);
        }
        
        /// <summary>
        /// Update a specific document of the database
        /// </summary>
        /// <typeparam name="T">Expected class/type of the document</typeparam>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="document">Document to update in the database with its RID</param>
        /// <param name="document_res">Document updated and returned</param>
        /// <param name="updatemode">Could be a Full update or a Partial one</param>
        /// <returns>Feedback of the operation</returns>
        public static GenericResult UpdateDocument<T>(parmConnection Connection, T document, out T document_res, UpdateMode updatemode = UpdateMode.Full)
        {
            GenericResult res = new GenericResult();
            res.Code = ResultCode.OK;

            document_res = document;

            try
            {
                // Reperisco l'Id dal parametro
                PropertyInfo prop = document.GetType().GetProperty("Id");
                if (prop == null)
                {
                    res.Code = ResultCode.ParametersError;
                    res.Message = "'Id' property is missing";
                    return res;
                }
                String Id = prop.GetValue(document).ToString();
                if (Id.StartsWith("#"))
                    Id = Id.Substring(1);

                String updatemode_str = "full";
                switch (updatemode)
                {
                    case UpdateMode.Partial:
                        updatemode_str = "partial";
                        break;
                }

                document_res = (T)Utility.CallWS<T>(Connection
                                    , JsonHelper.SerializeObjectToString(document)
                                    , "document/" + Connection.DBName + "/" + Id + "?updateMode=" + updatemode_str
                                    , "PUT");
            }
            catch (Exception exc)
            {
                res.Code = ResultCode.GenericError;
                res.Message = exc.Message;
            }

            return res;
        }
        public static GenericResult UpdateDocument<T>(parmConnection Connection, T document, UpdateMode updatemode = UpdateMode.Full)
        {
            T document_res = document;
            return UpdateDocument<T>(Connection, document, out document_res, updatemode);
        }

        /// <summary>
        /// Returns the list of the existing databases on the server specified
        /// </summary>
        /// <param name="Connection">Datas for the connection to the server</param>
        /// <returns>List of the databases names, or an error feedback</returns>
        public static ListStringResult DatabasesList(parmConnection Connection)
        {
            ListStringResult res = new ListStringResult();
            res.Code = ResultCode.OK;

            try
            {
                DatabasesListResult dlres = Utility.CallWS<DatabasesListResult>(Connection
                                                        , null
                                                        , "listDatabases"
                                                        , "GET") as DatabasesListResult;
                res.Result = dlres.Databases.ToList<String>();
                return res;
            }
            catch (Exception exc)
            {
                res.Code = ResultCode.GenericError;
                res.Message = exc.Message;
            }

            return res;
        }

        /// <summary>
        /// Returns the structure of the specified class
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="Name">Name of the class</param>
        /// <returns>Structure of the class found, or an error feedback</returns>
        public static ClassStructResult GetClass(parmConnection Connection, String Name)
        {
            ClassStructResult res = new ClassStructResult();
            res.Code = ResultCode.OK;

            try
            {
                res.Result = Utility.CallWS<ClassStruct>(Connection
                                        , null
                                        , "class/" + Connection.DBName + "/" + Name
                                        , "GET") as ClassStruct;
            }
            catch (Exception exc)
            {
                res.Code = ResultCode.GenericError;
                res.Message = exc.Message;
            }

            return res;
        }

        /// <summary>
        /// SELECT statement of a DotQuery object
        /// </summary>
        /// <typeparam name="T">Expected class/type of the documents returned</typeparam>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="ClassesStruct">Structures of the classes involved</param>
        /// <param name="Qry_From">SQL query for the FROM statement</param>
        /// <param name="Qry_Where">SQL query for the WHERE statement</param>
        /// <param name="SelectAll">if 'true' returns all the field using the "*" char</param>
        /// <returns>List of documents returned from the query</returns>
        public static List<T> DotQuery_Select<T>(Connection Connection, List<ClassStruct> ClassesStruct, String Qry_From, String Qry_Where = "", Boolean SelectAll = false)
        {
            String qry_select = "";

            if (!SelectAll)
            {
                // Add to the SELECT query the RID of each class
                foreach (ClassStruct classes_struct_elem in ClassesStruct)
                {
                    if (qry_select != "")
                        qry_select += ", ";
                    qry_select += String.Format("@rid as {0}_rid", classes_struct_elem.Name);
                }

                // Get from the Type all the Properties of the Classes that are involved
                foreach (PropertyInfo pi in (typeof(T)).GetProperties())
                {
                    String qry_attrname = pi.Name;
                    if (pi.CustomAttributes.Any(ca => ca.AttributeType.Name == "JsonPropertyAttribute"))
                    {
                        CustomAttributeData attr = pi.CustomAttributes.First(ca => ca.AttributeType.Name == "JsonPropertyAttribute");
                        qry_attrname = attr.ConstructorArguments[0].Value.ToString();

                        // Check if the Class is involved and the Attribute exists, so I can add it to the SELECT statement
                        if (qry_attrname.Contains("."))
                        {
                            if (ClassesStruct.Any(cs => cs.Name == qry_attrname.Split('.')[0])
                                && ClassesStruct.Find(cs => cs.Name == qry_attrname.Split('.')[0])
                                                 .Properties.Any(p => p.Name == qry_attrname.Split('.')[1]))
                            {
                                if (qry_select != "")
                                    qry_select += ", ";
                                qry_select += qry_attrname;
                            }
                        }
                        else
                        {
                            if (ClassesStruct.SelectMany(cs => cs.Properties).Any(p => p.Name == qry_attrname))
                            {
                                if (qry_select != "")
                                    qry_select += ", ";
                                qry_select += qry_attrname;
                            }
                        }
                    }
                }
            }
            else
                qry_select = "*";
            qry_select = "SELECT " + qry_select;

            String qry_complete = qry_select + " " + Qry_From;
            if (!String.IsNullOrEmpty(Qry_Where))
                qry_complete += " " + Qry_Where;

            return Connection.Query<T>(qry_complete) as List<T>;
        }

        /// <summary>
        /// COUNT statement of a DotQuery object
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="Qry_From">SQL query for the FROM statement</param>
        /// <param name="Qry_Where">SQL query for the WHERE statement</param>
        /// <returns>Total number of records returned from the complete query</returns>
        public static Int32 DotQuery_Count(Connection Connection, String Qry_From, String Qry_Where = "")
        {
            String qry_complete = "SELECT COUNT(*) as Result " + Qry_From;
            if (!String.IsNullOrEmpty(Qry_Where))
                qry_complete += " " + Qry_Where;

            List<Int32Result> res = Connection.Query<Int32Result>(qry_complete) as List<Int32Result>;
            return res[0].Result;
        }

        /// <summary>
        /// Execute a transaction made of one or more operations
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="ParmBatch">List of all the operations to execute with the transaction</param>
        public static void Transaction_Execute(Connection Connection, parmBatch ParmBatch)
        {
            if (ParmBatch != null && ParmBatch.Operations.Count > 0)
            {
                Utility.CallWS<Object>(Connection.parmConnection
                                    , JsonHelper.SerializeObjectToString(ParmBatch)
                                    , "batch/" + Connection.parmConnection.DBName
                                    , "POST");
            }
        }
    }
}
