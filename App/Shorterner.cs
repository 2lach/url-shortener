using LiteDB;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace url_shortener.App
{
    public class ShortUrl
    {
        // Here is the database model
        // each column in the database will have one of the following fields
        public Guid ID { get; set; } // unique id
        public string URL { get; set; } // the original url
        public string ShortenedURL { get; set; } // the shortned url
        public string Token { get; set; } // well a token
        public int Clicked { get; set; } = 0; // click counter?
        public DateTime Created { get; set; } = DateTime.Now; // Created date

    }


    public class Shorterner
    {

        public string? Token { get; set; }
        private ShortUrl biturl;

        // generate the random token
        private Shorterner GenerateToken()
        {
            // create an empty string which will carry/contain all the chars of the url Safe
            string urlsafe = string.Empty;
            // this is the same as
            // string  urlsafe = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789"
            // but done in LINQ
            Enumerable.Range(48, 75)
                .Where(index => index < 58 || index > 64 && index < 91 || index > 96)
                .OrderBy(order => new Random().Next())
                .ToList()
                .ForEach(index => urlsafe += Convert.ToChar(index)); // Store each char into urlsafe

            // then generate a random string from the random string (2 to 6 chars long) into the member Token
            Token = urlsafe.Substring(new Random().Next(0, urlsafe.Length), new Random().Next(2, 6));
            // the return it
            return this;
        }

        public bool CheckUrlExists(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Send an HTTP HEAD request to the URL (HEAD request only retrieves headers, not the full content)
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;

                    // Check if the response status code indicates success (2xx range)
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                // nope
                return false;
            }
        }


        public bool Checkurl(string url)
        {
            bool urlExists = CheckUrlExists(url);
            bool result;
            if (urlExists)
            {
                Uri uriResult;
                result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            }
            else
            {
                Console.WriteLine("URL does not exist or is not reachable.");
                result = false;
            }
            return result;
        }

        public Shorterner(string url)
        {
            string dbPath = new AppConf().Config.DB_PATH;
            using (var db = new LiteDatabase(dbPath))
            {
                // Get a collection (or create, if doesn't exist)
                // var col = db.GetCollection<Customer>("customers");

                // point towards the relevant db data
                var urls = db.GetCollection<ShortUrl>();

                // if the token already exists create a new one
                while (urls.Exists(u => u.Token == GenerateToken().Token)) ;
                // store the values in the ShortUrl db model
                biturl = new ShortUrl()
                {
                    Token = Token,
                    URL = url,
                    ShortenedURL = new AppConf().Config.BASE_URL + Token
                };
                // weird error handeling basically this should not occur
                if (urls.Exists(u => u.URL == url))
                {
                    throw new Exception("Url already Exists");
                }

                urls.Insert(biturl);
            }

        }

    }
}

