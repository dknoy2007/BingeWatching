using System;
using System.Net;
using BingeWatching.Models;
using BingeWatching.Models.Enums;
using Newtonsoft.Json;

namespace BingeWatching.Services.NetflixService
{
    public class NetflixService
    {
        private const string BaseAddress = "https://api.reelgood.com/v1/";
        private const string Endpoint = "roulette/netflix";

        // In case the program was a Web.Api service (and not a console app) I would have done it asynchronously with Task async/await
        public Content GetRandomContent(ContentKind kind)
        {
            try
            {
                var contentKind = ConvertContentTypeEnumToString(kind);

                if (string.IsNullOrWhiteSpace(contentKind))
                {
                    return null;
                }

                using var webClient = new WebClient
                {
                    BaseAddress = BaseAddress
                };

                webClient.QueryString.Add("nocache", "true");
                webClient.QueryString.Add("content_kind", contentKind);
                webClient.QueryString.Add("availability", "onAnySource");
                
                var json = webClient.DownloadString(Endpoint);
                
                return JsonConvert.DeserializeObject<Content>(json);
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Failed to get content from netflix service\nError: {ex.Message}");
                return null;
            }
        }

        public static string ConvertContentTypeEnumToString(ContentKind kind)
        {
            switch (kind)
            {
                case ContentKind.TvShow:
                    return "show";
                case ContentKind.Movie:
                    return "movie";
                case ContentKind.Any:
                    return "both";
                default:
                    Console.WriteLine($"Invalid content type argument {kind} was passed to netflix service");
                    return string.Empty;
            }
        }
    }
}