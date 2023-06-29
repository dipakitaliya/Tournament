using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CGRPG_Tournament
{
    public class ValidateController : ApiController
    {
        [Route("Validate")]  
        [HttpGet]  
        public async Task<string> Validate(string token, string username)
        {
            var bytes = Encoding.ASCII.GetBytes(token);
            string url = @"https://api.rpg.chainguardians.io/userInfo";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using(HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        } 

    }
}