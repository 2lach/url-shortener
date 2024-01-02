using System;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using url_shortener.App;



namespace url_shorter.Controllers
{
    public class URLResponse
    {
        public string url { get; set; }
        public string status { get; set; }
        public string token { get; set; }
    }


    public class ShortenRequest
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }


    public class HomeController : Controller
    {
        string dbPath = new AppConf().Config.DB_PATH;
        private ShortUrl biturl;

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
                    if (!url.Contains("http"))
                    {
                        url = "http://" + url;
                    }
                    else if (url == null)
                    {
                        throw new Exception("Url is null");
                    }


                    // check if url is valid
                    Shorterner shortUrl = new Shorterner(url);
                    bool isUrlValid = shortUrl.Checkurl(url);

                    if (!isUrlValid)
                    {
                        Response.StatusCode = 400;
                        return Json(new URLResponse()
                        {
                            url = url,
                            status = "Not a valid URL",
                            token = null
                        });
                    }
                    if (DB.Exists(u => u.ShortenedURL == url))
                    {
                        Response.StatusCode = 405;
                        return Json(new URLResponse()
                        {
                            url = url,
                            status = "already shortened",
                            token = null
                        });
                    }
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
        //TODO sanity checks, make sure url does not exist and stuff like that
        [HttpPost, Route("/shorten")]
        public IActionResult ShortenUrl([FromBody] ShortenRequest request)
        {
            var originalUrl = request.Url;
            var desiredToken = request.Token;

            Shorterner shortUrlChecker = new Shorterner(originalUrl);
            bool isUrlValid = shortUrlChecker.Checkurl(originalUrl);


            if (!isUrlValid)
            {
                Response.StatusCode = 400;
                return Json(new URLResponse()
                {
                    url = originalUrl,
                    status = "Not a valid URL",
                    token = null
                });
            }


            if (originalUrl == null || desiredToken == null)
            {
                throw new Exception("No null values motherfucker");
            }

            string dbPath = new AppConf().Config.DB_PATH;
            using (var db = new LiteDatabase(dbPath))
            {
                try
                {
                    var urls = db.GetCollection<ShortUrl>();

                    biturl = new ShortUrl()
                    {
                        Token = desiredToken,
                        URL = originalUrl,
                        ShortenedURL = new AppConf().Config.BASE_URL + desiredToken
                    };

                    string shortenedUrl = new AppConf().Config.BASE_URL + desiredToken;
                    urls.Insert(biturl);
                    return Json(new { shortenedUrl });

                }
                catch (Exception error)
                {
                    throw new Exception("Oh shit..." + error);
                }
            }
        }

    }
}

