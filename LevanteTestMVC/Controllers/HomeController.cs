using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Levante;
using Newtonsoft.Json;

namespace LevanteTestMVC.Controllers
{
    class Utente/* : LevanteSync */
    {
        [JsonProperty("@type")]
        public String Type { get; set; }
        [JsonProperty("@rid")]
        public String Id { get; set; }
        [JsonProperty("@version")]
        public Int32 Version { get; set; }
        [JsonProperty("@class")]
        public String Class { get; set; }

        [JsonProperty("Nome")]
        public String Nome { get; set; }
        [JsonProperty("Cognome")]
        public String Cognome { get; set; }
        [JsonProperty("Immagine")]
        public Byte[] Immagine { get; set; }
        [JsonProperty("Ruoli")]
        public List<Ruolo> Ruoli { get; set; }

        // Id Fake utilizzato solamente per aggiornare l'Id reale dopo una proiezione
        private String _Id_Fake;
        [JsonProperty("Utente_rid")]
        private String Id_Fake
        {
            get
            {
                return _Id_Fake;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _Id_Fake = value;
                    Id = value;
                }
            }
        }
        
        public Utente()
        {
            Id = "";
            Type = "d";
            Class = "Utente";
            Ruoli = new List<Ruolo>();
        }
    }

    class Ruolo
    {
        [JsonProperty("@type")]
        public String Type { get; set; }
        [JsonProperty("@rid")]
        public String Id { get; set; }
        [JsonProperty("@version")]
        public Int32 Version { get; set; }
        [JsonProperty("@class")]
        public String Class { get; set; }

        [JsonProperty("Descrizione")]
        public String Descrizione { get; set; }

        // Id Fake utilizzato solamente per aggiornare l'Id reale dopo una proiezione
        private String _Id_Fake;
        [JsonProperty("Ruolo_rid")]
        private String Id_Fake
        {
            get
            {
                return _Id_Fake;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _Id_Fake = value;
                    Id = value;
                }
            }
        }

        public Ruolo()
        {
            //Id = "";
            Type = "d";
            Class = "Ruolo";
        }
    }

    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            Levante.Server pServer = new Levante.Server();
            pServer.Name = "localhost";
            pServer.Port = 2480;

            pServer.DatabasesList();
            
            Levante.parmConnection pConn = new Levante.parmConnection();
            pConn.Server = pServer;
            pConn.DBName = "Test";
            pConn.Authentication = new Levante.parmAuthentication();
            pConn.Authentication.Name = "admin";
            pConn.Authentication.Password = "admin";
            using (Levante.Connection conn = new Levante.Connection(pConn))
            {
                if (conn.Connect().Code == Levante.ResultCode.OK)
                {
                    String Log = "";

                    Log += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " INIZIO\r\n";
                    // Elimino tutti gli utenti
                    conn.From("Utente").Delete();
                    Log += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " Pulita tabella Utente\r\n";
                    // Ottengo la lista dei Ruoli
                    List<Ruolo> Ruoli = conn.From("Ruolo").Select<Ruolo>();
                    // Inserisco 100000 Utenti differenti
                    using (Levante.Connection.Transaction trans = conn.CreateTransaction())
                    {
                        for (Int32 idxUtente = 1; idxUtente <= 100000; idxUtente++)
                        {
                            Utente nuovoute = new Utente();
                            nuovoute.Nome = "Utente" + idxUtente;
                            nuovoute.Cognome = "CogUtente" + idxUtente;
                            nuovoute.Ruoli.Add(Ruoli[idxUtente % 2]);
                            trans.Insert<Utente>(nuovoute);
                        }
                        trans.Execute();
                    }
                    Log += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " Inseriti 100.000 Utenti\r\n";
                    List<Utente> Utenti = conn.From("Utente").Select<Utente>();
                    Log += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " Effettuata query di selezione\r\n";
                }
            }

            return View();
        }

    }
}
