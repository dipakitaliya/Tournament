using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CGRPG_Tournament.Models;
using CGRPG_TournamentLib.Models;
using Newtonsoft.Json;

namespace CGRPG_TournamentLib.Helpers
{
    public class ChilliConnectHelper
    {
#if STAGING
        private readonly static string url = "https://5ygoaoppuk.execute-api.ap-northeast-1.amazonaws.com/Staging"; //staging
#else
        private readonly static string url = "https://rpgbackend.chainguardians.io/"; //prod
#endif
        public static async Task<ChilliConnectBalanceModel> GetBalance(string token)
        {
            try
            {
                string url = $"{ChilliConnectHelper.url}/currency/get";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Authorization", token);
                request.Headers.Add("sessionid", "ebb065d1-bc6d-449b-b9a4-263d298b1d43");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.PreAuthenticate = true;
                request.Accept = "*/*";
                //request.Connection = "keep-alive";
                request.ContentType = "application/json";
                    
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
                
                try
                {
                    requestWriter.Write("{\"data\":\"" + AESHelper.EncryptStringToBytes_Aes("{\"Keys\":[\"FIAT\"]}", token) + "\"}");
                }
                finally
                {
                    requestWriter.Close();
                }
                
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream() ?? throw new InvalidOperationException())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseData = await reader.ReadToEndAsync();
                    var model = JsonConvert.DeserializeObject<ChilliConnectBalanceModel>(AESHelper.DecryptStringFromBytes_Aes(responseData, token));
                    return model;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ChilliConnectBalanceModel();
            }
        }
        
        public static async Task<ChilliConnectBalanceModel> SetBalance(string token,
            long balance, long amount)
        {
            try
            {
                string url = $"{ChilliConnectHelper.url}/script/run";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Authorization", token);
                request.Headers.Add("sessionid", "ebb065d1-bc6d-449b-b9a4-263d298b1d43");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.PreAuthenticate = true;
                request.Accept = "*/*";
                //request.Connection = "keep-alive";
                request.ContentType = "application/json";
                
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
                
                try
                {
                    requestWriter.Write("{\"data\":\"" + AESHelper.EncryptStringToBytes_Aes(
                        "{\"Key\":\"UPDATE_FIAT\",\"Params\":{\"fiatCount\":\"" + (balance + amount) + "\"}}", token)+ "\"}");
                    //requestWriter.Write("{\"Key\":\"UPDATE_FIAT\",\"Params\":{\"fiatCount\":\"" + (balance + amount) + "\"}}");
                }
                finally
                {
                    requestWriter.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream() ?? throw new InvalidOperationException())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseData = await reader.ReadToEndAsync();
                    var model = JsonConvert.DeserializeObject<ChilliConnectBalanceModel>(AESHelper.DecryptStringFromBytes_Aes(responseData, token));
                    return model;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ChilliConnectBalanceModel();
            }
        }
    }
}