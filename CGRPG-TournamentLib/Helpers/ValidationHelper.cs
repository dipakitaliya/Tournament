using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CGRPG_Tournament.Models;
using CGRPG_TournamentLib.Models;
using Newtonsoft.Json;

namespace CGRPG_Tournament.Helpers
{
    public class ValidationHelper
    {
        public static async Task<ValidationTokenModel> Validate(string token)
        {
            try
            {
                string url = @"https://api.rpg.chainguardians.io/userInfo";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.PreAuthenticate = true;
                request.Headers.Add("Authorization", "Bearer " + token);
                request.Accept = "application/json";
                request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseData = await reader.ReadToEndAsync();
                    var model = JsonConvert.DeserializeObject<ValidationTokenModel>(responseData);
                    return model;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException.InnerException.Message);
                return new ValidationTokenModel(false, "Failed to connect", 0,
                    "Dammit Jim, I’m a Rendering Doctor, not a Backend Developer", "baduser");
            }
        }
    }
}