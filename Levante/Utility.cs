using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Levante
{
    class Utility
    {
        public static Object CallWS<T>(parmConnection Connection, String parameter, String actionToCall, String method = "POST")
        {
            String WSREST_JDeliver = @"http://" + Connection.Server.Name;
            if (Connection.Server.Port != null)
                WSREST_JDeliver += ":" + Connection.Server.Port;
            WSREST_JDeliver += @"/";

            HttpWebRequest request = null;

            Uri uri = new Uri(WSREST_JDeliver + HttpUtility.UrlEncode(actionToCall));
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.ContentType = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.KeepAlive = false;
            request.Timeout = 1000 * 60 * 5;

            // Autenticazione
            if (Connection.Authentication != null)
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(
                                            Encoding.Default.GetBytes(Connection.Authentication.Name + ":" + Connection.Authentication.Password));

            if (parameter != null)
            {
                //String postData = JsonHelper.SerializeObjectToString(parameter);
                String postData = parameter;
                Byte[] dataToPost = Encoding.UTF8.GetBytes(postData);
                //request.Headers.Add("Content-Encoding", "gzip");
                //request.SendChunked = true;
                request.ContentLength = dataToPost.Length;
                using (Stream grStream = request.GetRequestStream())
                {
                    //using (GZipStream gzip = new GZipStream(grStream, CompressionMode.Compress))
                    //{
                        //gzip.Write(dataToPost, 0, dataToPost.Length);
                    grStream.Write(dataToPost, 0, dataToPost.Length);
                    //}
                }
            }

            String result = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }

            Object res = JsonHelper.DeserializeObjectFromString<T>(result);

            return res;
        }
    }

    class JsonHelper
    {
        public static String SerializeObjectToString(Object objectToSerialize)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (JsonWriter jsonWriter = new JsonTextWriter(sw))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    JsonSerializer js = new JsonSerializer();
                    js.Serialize(jsonWriter, objectToSerialize);
                }
            }

            return sb.ToString();
        }

        public static T DeserializeObjectFromString<T>(String stringToDeserialize)
        {
            using (StringReader sr = new StringReader(stringToDeserialize))
            {
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer js = new JsonSerializer();
                    return js.Deserialize<T>(jr);
                }
            }
        }
    }
}
