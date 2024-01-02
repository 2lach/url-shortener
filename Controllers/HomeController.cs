using LiteDB;
using Microsoft.AspNetCore.Mvc;
using url_shortener.App;



namespace url_shorter.Controllers
{
    public class URLResponse
    {
        public string url { get; set; }
        public string status { get; set; }
        public string token { get; set; }
    }


    public class HomeController : Controller
    {
        string dbPath = new AppConf().Config.DB_PATH;

        // Index Route
        [HttpGet, Route("/")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet, Route("/all")]
        public IActionResult GetAllUrls()
        {
            using (var liteDb = new LiteDatabase(dbPath))
            {
                var collection = liteDb.GetCollection<ShortUrl>();
                var allUrls = collection.FindAll().ToList();

                return Json(allUrls);
            }
        }

        // Url shorterner route
        [HttpPost, Route("/")]
        public IActionResult PostURL([FromBody] string url)
        {

            using (var liteDb = new LiteDatabase(dbPath))
            {
                var DB = liteDb.GetCollection<ShortUrl>();

                try
                {


                    // verify url starts with http
                    if (!url.Contains("http") && url != null)
                    {
                        url = "http://" + url;
                    }
                    else if (url == null)
                    {
                        Console.WriteLine("Url is a null value " + url);
                    }
                    if (DB.Exists(u => u.ShortenedURL == url))
                    // if (new LiteDB.LiteDatabase("Data/Urls.db").GetCollection<NixURL>().Exists(u => u.ShortenedURL == url))
                    {
                        Response.StatusCode = 405;
                        return Json(new URLResponse()
                        {
                            url = url,
                            status = "already shortened",
                            token = null
                        });
                    }


                    Shorterner shortUrl = new Shorterner(url);
                    // Shorten the URL by calling new Shortener(url) and return the token as a JSON string.
                    return Json(shortUrl.Token);
                }
                catch (Exception error)
                {
                    if (error.Message == "Url already exists")
                    {
                        Response.StatusCode = 400;

                        return Json(new URLResponse()
                        {
                            url = url,
                            status = "Url already exists",
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                            token = DB.Find(u => u.URL == url)
                            .FirstOrDefault().Token
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        });

                    }

                    throw new Exception(error.Message);
                }
            }

        }

        [HttpGet, Route("/{token}")]
        public IActionResult UrlRedirect([FromRoute] string token)
        {
            using (var liteDb = new LiteDatabase(dbPath))
            {
                var db = liteDb;
                var collection = db.GetCollection<ShortUrl>();
                var shortUrl = collection.FindOne(u => u.Token == token);

                if (shortUrl != null)
                {
                    return Redirect(shortUrl.URL);
                }
                else
                {
                    // Handle the case where no matching record is found.
                    // You can return a specific error message or redirect to a default URL.
                    // For example:
                    return NotFound("URL not found for the provided token.");
                }
            }
        }

        /*

        private string FindRedirect(string url)
        {
            string Result = string.Empty;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode && response is not null)
                {
                    Result = response.Headers.Location.ToString();
                }
            }
            return Result;
        }
        */
    }
}

