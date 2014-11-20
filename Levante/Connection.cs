using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Levante
{
    /// <summary>
    /// Class to create a new OrientDB Session
    /// </summary>
    public class Connection:IDisposable
    {
        // Datas for the connection to the database
        private parmConnection _parmConnection;
        public parmConnection parmConnection
        {
            get
            {
                return _parmConnection;
            }
            set
            {
                _parmConnection = value;
            }
        }

        // Identify if the connection is open
        private Boolean _Connected = false;
        // List of the structures of all the classes of the database
        private List<ClassStruct> StructClasses = new List<ClassStruct>();

        /// <summary>
        /// Constructor to create a new Session
        /// </summary>
        /// <param name="Connection">Datas for the connection to the database</param>
        public Connection(parmConnection Connection)
        {
            try
            {
                _parmConnection = Connection;
                StructClasses = new List<ClassStruct>();
            }
            catch (Exception exc)
            {
                throw new Exception("Connection - " + exc.Message);
            }
        }

        /// <summary>
        /// Try to connect to the specified database
        /// </summary>
        /// <returns>Feedback of the operation</returns>
        public GenericResult Connect()
        {
            try
            {
                GenericResult res = new GenericResult();
                res.Code = ResultCode.OK;

                // If I'm already connected I quit the method
                if (!_Connected)
                {
                    res = Calls.ConnectToODB(_parmConnection);
                    if (res.Code == ResultCode.OK)
                        _Connected = true;
                } 

                return res;
            }
            catch (Exception exc)
            {
                throw new Exception("Connect - " + exc.Message);
            }
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        /// <returns>Feedback of the operation</returns>
        public GenericResult Disconnect()
        {
            try
            {
                GenericResult res = new GenericResult();
                res.Code = ResultCode.OK;

                // Try the disconnection only if the connection is on
                if (_Connected)
                {
                    res = Calls.DisconnectFromODB(_parmConnection);
                    if (res.Code == ResultCode.OK)
                        _Connected = false;
                }

                return res;
            }
            catch (Exception exc)
            {
                throw new Exception("Disconnect - " + exc.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                // Disconnect from the server
                GenericResult res = Disconnect();
                if (res.Code != ResultCode.OK)
                    throw new Exception(res.Message);
            }
            catch (Exception exc)
            {
                throw new Exception("Dispose - " + exc.Message);
            }
        }

        /// <summary>
        /// Get a document given its RID
        /// </summary>
        /// <typeparam name="T">Expected class/type of the document</typeparam>
        /// <param name="RID">Record Id</param>
        /// <returns>Document found, or GenericResult of the exception</returns>
        public Object GetDocumentById<T>(String RID)
        {
            try
            {
                // Check if the connection is on
                if (!_Connected)
                {
                    GenericResult res = new GenericResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.GetDocumentById<T>(_parmConnection, RID);
            }
            catch (Exception exc)
            {
                throw new Exception("GetDocumentById - " + exc.Message);
            }
        }

        /// <summary>
        /// Check if a document exists in the database
        /// </summary>
        /// <param name="RID">Record Id</param>
        /// <returns>True if the document exists</returns>
        public BooleanResult DocumentExists(String RID)
        {
            try
            {
                BooleanResult res = new BooleanResult();
                res.Result = false;

                // Check if the connection is on
                if (!_Connected)
                {
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                try
                {
                    res.Result = Calls.DocumentExists(_parmConnection, RID);
                }
                catch (Exception exc)
                {
                    if (exc.Message.Contains("500"))
                    {
                        // Error 500 - Document not found
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
                throw new Exception("DocumentExists - " + exc.Message);
            }
        }

        /// <summary>
        /// Given a Query (e.g.: SELECT) returns the result
        /// </summary>
        /// <typeparam name="T">Expected class/type of the documents returned</typeparam>
        /// <param name="SqlQuery">SQL command for the query</param>
        /// <param name="Limit">Limit number of the returned documents</param>
        /// <param name="FetchPlan">Fetching level for the nested classes; N=maximum level specified, 0=current document, -1=all the levels, -2=excluded</param>
        /// <returns>List<T> of the documents founded with the query</returns>
        public Object Query<T>(String SqlQuery, Int32 Limit = 100000, Int32 FetchPlan = -1)
        {
            try
            {
                // Check if the connection is on
                if (!_Connected)
                {
                    GenericResult res = new GenericResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.Query<T>(_parmConnection, SqlQuery, Limit, FetchPlan);
            }
            catch (Exception exc)
            {
                throw new Exception("Query - " + exc.Message);
            }
        }

        /// <summary>
        /// Execute a SQL Command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <typeparam name="T">Expected class/type of the documents returned</typeparam>
        /// <param name="SqlCommand">SQL command to execute</param>
        /// <returns>List<T> of the documents affected by the command, or GenericResult of the exception</returns>
        public Object Command<T>(String SqlCommand)
        {
            try
            {
                // Check if the connection is on
                if (!_Connected)
                {
                    GenericResult res = new GenericResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.Command<T>(_parmConnection, SqlCommand);
            }
            catch (Exception exc)
            {
                throw new Exception("Command - " + exc.Message);
            }
        }

        /// <summary>
        /// Delete a document given its RID
        /// </summary>
        /// <param name="RID">Record Id</param>
        /// <returns>Feedback of the operation</returns>
        public GenericResult DeleteDocumentById(String RID)
        {
            try
            {
                // Check if the connection is on
                if (!_Connected)
                {
                    GenericResult res = new GenericResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.DeleteDocumentById(_parmConnection, RID);
            }
            catch (Exception exc)
            {
                throw new Exception("DeleteDocumentById - " + exc.Message);
            }
        }

        /// <summary>
        /// Create a new document in the database for the specified class
        /// </summary>
        /// <typeparam name="T">Expected class/type of the document to insert</typeparam>
        /// <param name="Connection">Datas for the connection to the database</param>
        /// <param name="document">Document to insert in the database</param>
        /// <param name="document_res">Document created with its RID</param>
        /// <returns>Feedback of the operation</returns>
        public GenericResult InsertDocument<T>(T document, out T document_res)
        {
            try
            {
                document_res = document;

                // Check if the connection is on
                if (!_Connected)
                {
                    GenericResult res = new GenericResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.InsertDocument<T>(_parmConnection, document, out document_res);
            }
            catch (Exception exc)
            {
                throw new Exception("InsertDocument - " + exc.Message);
            }
        }
        public GenericResult InsertDocument<T>(T document)
        {
            T document_res = document;

            return InsertDocument<T>(document, out document_res);
        }

        /// <summary>
        /// Update a specific document of the database
        /// </summary>
        /// <typeparam name="T">Expected class/type of the document</typeparam>
        /// <param name="document">Document to update in the database with its RID</param>
        /// <param name="document_res">Document updated and returned</param>
        /// <param name="updatemode">Could be a Full update or a Partial one</param>
        /// <returns>Feedback of the operation</returns>
        public GenericResult UpdateDocument<T>(T document, out T document_res, UpdateMode updatemode = UpdateMode.Full)
        {
            try
            {
                document_res = document;

                // Check if the connection is on
                if (!_Connected)
                {
                    GenericResult res = new GenericResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.UpdateDocument<T>(_parmConnection, document, out document_res, updatemode);
            }
            catch (Exception exc)
            {
                throw new Exception("UpdateRecord - " + exc.Message);
            }
        }
        public GenericResult UpdateDocument<T>(T document, UpdateMode updatemode = UpdateMode.Full)
        {
            T document_res = document;

            return UpdateDocument<T>(document, out document_res, updatemode);
        }

        /// <summary>
        /// Returns the structure of the specified class
        /// </summary>
        /// <param name="Name">Name of the class</param>
        /// <returns>Structure of the class found, or an error feedback</returns>
        public ClassStructResult GetClass(String Name)
        {
            try
            {
                // Check if the connection is on
                if (!_Connected)
                {
                    ClassStructResult res = new ClassStructResult();
                    res.Code = ResultCode.NotConnected;
                    res.Message = "Current connection is not open";
                    return res;
                }

                return Calls.GetClass(_parmConnection, Name);
            }
            catch (Exception exc)
            {
                throw new Exception("GetClass - " + exc.Message);
            }
        }

        /// <summary>
        /// Check if the connection is on
        /// </summary>
        /// <returns>Returns if the connection is still open or not</returns>
        public Boolean IsConnected()
        {
            try
            {
                return _Connected;
            }
            catch (Exception exc)
            {
                throw new Exception("IsConnected - " + exc.Message);
            }
        }

        #region DOT QUERY

        /// <summary>
        /// Clausola "FROM" dell'interrogazione
        /// </summary>
        /// <param name="Classes">Nomi delle classi coinvolte nell'interrogazione, separati dal carattere ','</param>
        /// <returns></returns>
        public DotQueryFrom From(String Classes)
        {
            return From(Classes.Split(',').ToList<String>());
        }
        /// <summary>
        /// Clausola "FROM" dell'interrogazione
        /// </summary>
        /// <param name="Classes">Lista dei nomi delle classi coinvolte nell'interrogazione</param>
        /// <returns></returns>
        public DotQueryFrom From(List<String> Classes)
        {
            DotQueryFrom res = new DotQueryFrom();
            res.result = new GenericResult();
            res.result.Code = ResultCode.OK;

            // Verifico se sono connesso
            if (!_Connected)
            {
                res.result = new GenericResult();
                res.result.Code = ResultCode.NotConnected;
                res.result.Message = "Current connection is not open";
                return res;
            }

            // Verifico se sono state specificate classi
            if (Classes == null 
                || Classes.Count <= 0
                || Classes.Any(c => c.Trim() == ""))
            {
                res.result = new GenericResult();
                res.result.Code = ResultCode.NotConnected;
                res.result.Message = "At least one class must be specified and all the classes names must not be empty strings";
                return res;
            }

            // Ottengo le strutture di tutte le tabelle coinvolte
            foreach (String Classe in Classes)
            {
                try
                {
                    if (!StructClasses.Any(sc => sc.Name == Classe))
                    { 
                        ClassStructResult csres = GetClass(Classe);
                        if (csres.Code == ResultCode.OK)
                        {
                            StructClasses.Add(csres.Result);
                        }
                    }
                }
                catch
                {

                }
            }

            // Costruisco il FROM
            String qry_from = "";
            foreach (String classe in Classes)
            {
                if (qry_from != "")
                    qry_from += ", ";
                qry_from += classe;
            }
            qry_from = "FROM " + qry_from;

            res.classes_name = Classes;
            res.classes_struct = StructClasses;
            res.connection = this;
            res.qry_from = qry_from;
            return res;
        }

        #region RELATED CLASSES

        public class DotQueryFrom
        {
            public GenericResult result { get; set; }
            public List<String> classes_name { get; set; }
            public List<ClassStruct> classes_struct { get; set; }
            public Connection connection { get; set; }

            public String qry_from { get; set; }

            /// <summary>
            /// Clausola "SELECT" dell'interrogazione
            /// </summary>
            /// <typeparam name="T">Classe dei documenti da restituire</typeparam>
            /// <param name="SelectAll">se "true" applica ilcarattere jolly "*" per estrarre tutti i campi</param>
            /// <returns>Lista dei documenti restituiti dall'interrogazione</returns>
            public List<T> Select<T>(Boolean SelectAll = true)
            {
                return Calls.DotQuery_Select<T>(connection, classes_struct, qry_from, "", SelectAll);
            }

            /// <summary>
            /// Funzione "SELECT COUNT(*)" dell'interrogazione
            /// </summary>
            /// <returns>Restituisce il numero di record restituiti dall'interrogazione</returns>
            public Int32 Count()
            {
                return Calls.DotQuery_Count(connection, qry_from, "");
            }

            /// <summary>
            /// Elimina i record restituiti dall'interrogazione
            /// </summary>
            public void Delete()
            {
                connection.Command<String>("DELETE " + qry_from);
            }

            /// <summary>
            /// Restituisce un oggetto "parmBatchOperation" da utilizzare con un oggetto di tipo "Transaction" per eseguire l'eliminazione massiva di
            /// documenti in una transazione
            /// </summary>
            /// <returns>Operazione da utilizzare in un oggetto di tipo "Transaction"</returns>
            public parmBatchOperation DeleteToTransaction()
            {
                parmBatchOperation pbop = new parmBatchOperation();
                pbop.Type = "cmd";
                pbop.Language = "sql";
                pbop.Command = "DELETE " + qry_from;
                return pbop;
            }

            /// <summary>
            /// Clausola "WHERE" dell'interrogazione
            /// </summary>
            /// <param name="Conditions">Condizioni della clausola in linguaggio "SQL".
            /// Omettere la clausola "WHERE" nella stringa</param>
            /// <returns></returns>
            public DotQueryWhere Where(String Conditions)
            {
                DotQueryWhere res = new DotQueryWhere();
                res.result = new GenericResult();
                res.result.Code = ResultCode.OK;

                String qry_where = "";
                if (Conditions.Trim() != "")
                    qry_where = "WHERE " + Conditions;

                res.classes_name = classes_name;
                res.classes_struct = classes_struct;
                res.connection = connection;
                res.qry_from = qry_from;
                res.qry_where = qry_where;

                return res;
            }
        }

        public class DotQueryWhere
        {
            public GenericResult result { get; set; }
            public List<String> classes_name { get; set; }
            public List<ClassStruct> classes_struct { get; set; }
            public Connection connection { get; set; }

            public String qry_from { get; set; }
            public String qry_where { get; set; }

            /// <summary>
            /// Clausola "SELECT" dell'interrogazione
            /// </summary>
            /// <typeparam name="T">Classe dei documenti da restituire</typeparam>
            /// <param name="SelectAll">se "true" applica ilcarattere jolly "*" per estrarre tutti i campi</param>
            /// <returns>Lista dei documenti restituiti dall'interrogazione</returns>
            public List<T> Select<T>(Boolean SelectAll = true)
            {
                return Calls.DotQuery_Select<T>(connection, classes_struct, qry_from, qry_where, SelectAll);
            }

            /// <summary>
            /// Funzione "SELECT COUNT(*)" dell'interrogazione
            /// </summary>
            /// <returns>Restituisce il numero di record restituiti dall'interrogazione</returns>
            public Int32 Count()
            {
                return Calls.DotQuery_Count(connection, qry_from, qry_where);
            }

            /// <summary>
            /// Elimina i record restituiti dall'interrogazione
            /// </summary>
            public void Delete()
            {
                connection.Command<String>("DELETE " + qry_from + " " + qry_where);
            }

            /// <summary>
            /// Restituisce un oggetto "parmBatchOperation" da utilizzare con un oggetto di tipo "Transaction" per eseguire l'eliminazione massiva di
            /// documenti in una transazione
            /// </summary>
            /// <returns>Operazione da utilizzare in un oggetto di tipo "Transaction"</returns>
            public parmBatchOperation DeleteToTransaction()
            {
                parmBatchOperation pbop = new parmBatchOperation();
                pbop.Type = "cmd";
                pbop.Language = "sql";
                pbop.Command = "DELETE " + qry_from + " " + qry_where;
                return pbop;
            }
        }

        #endregion RELATED CLASSES

        #endregion DOT QUERY

        #region TRANSACTION

        /// <summary>
        /// Crea una nuova transazione/gruppo di operazioni
        /// </summary>
        /// <param name="isrealtransaction">Specifica se eseguire la lista di operazioni effettivamente come transazione oppure se confermare i dati nel DB al termine di ogni singola operazione</param>
        /// <returns>Oggetto transazione</returns>
        public Transaction CreateTransaction(Boolean isrealtransaction = true)
        {
            Transaction trans = new Transaction();
            trans.conn = this;
            trans.parmbatch.Transaction = isrealtransaction;
            return trans;
        }

        #region RELATED CLASSES

        public class Transaction : IDisposable
        {
            public parmBatch parmbatch { get; set; }
            public Connection conn { get; set; }

            /// <summary>
            /// Costruttore dell'oggetto Transaction
            /// </summary>
            public Transaction()
            {
                parmbatch = new parmBatch();
            }

            public void Dispose()
            {
                parmbatch = null;
            }

            /// <summary>
            /// Inserimento di un nuovo record in transazione
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="document"></param>
            public void Insert<T>(T document)
            {
                parmBatchOperation pbop = new parmBatchOperation();
                pbop.Type = "c";
                pbop.Record = document;
                parmbatch.Operations.Add(pbop);
            }

            /// <summary>
            /// Aggiornamento di un record esistente in transazione
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="document"></param>
            public void Update<T>(T document)
            {
                parmBatchOperation pbop = new parmBatchOperation();
                pbop.Type = "u";
                pbop.Record = document;
                parmbatch.Operations.Add(pbop);
            }

            /// <summary>
            /// Eliminazione di un record esistente in transazione
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="document"></param>
            public void Delete<T>(T document)
            {
                parmBatchOperation pbop = new parmBatchOperation();
                pbop.Type = "d";
                pbop.Record = document;
                parmbatch.Operations.Add(pbop);
            }

            /// <summary>
            /// Eliminazione di un record esistente in transazione
            /// </summary>
            /// <param name="Id"></param>
            public void Delete(String Id)
            {
                parmRecordRID prrid = new parmRecordRID();
                prrid.Id = Id;
                Delete<parmRecordRID>(prrid);
            }

            /// <summary>
            /// Esecuzione di un Command in transazione
            /// </summary>
            /// <param name="CommandText"></param>
            /// <param name="Language"></param>
            public void Command(String CommandText, CommandLanguage Language = CommandLanguage.SQL)
            {
                parmBatchOperation pbop = new parmBatchOperation();
                pbop.Type = "cmd";
                switch (Language)
                {
                    case CommandLanguage.SQL:
                        pbop.Language = "sql";
                        break;
                    default:
                        pbop.Language = "javascript";
                        break;
                }
                pbop.Command = CommandText;
                parmbatch.Operations.Add(pbop);
            }

            /// <summary>
            /// Esecuzione di uno Script in transazione
            /// </summary>
            /// <param name="ScriptText"></param>
            /// <param name="Language"></param>
            public void Script(String ScriptText, CommandLanguage Language = CommandLanguage.Javascript)
            {
                parmBatchOperation pbop = new parmBatchOperation();
                pbop.Type = "script";
                switch (Language)
                {
                    case CommandLanguage.SQL:
                        pbop.Language = "sql";
                        break;
                    default:
                        pbop.Language = "javascript";
                        break;
                }
                pbop.Script = ScriptText;
                parmbatch.Operations.Add(pbop);
            }

            /// <summary>
            /// Esegue la lista di operazioni specificate
            /// </summary>
            public void Execute()
            {
                Calls.Transaction_Execute(conn, parmbatch);
            }
        }

        #endregion RELATED CLASSES

        #endregion TRANSACTION
    }
    
}
