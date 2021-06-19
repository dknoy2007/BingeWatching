using System.Text;
using BingeWatching.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BingeWatching.Models
{
    public class Content
    {
        public string Id { get; set; }

        [JsonProperty("content_type"), JsonConverter(typeof(StringEnumConverter))]
        public ContentType Type { get; set; }

        public string Title { get; set; }
        
        public string Overview { get; set; }
        
        [JsonProperty("imdb_rating")]
        public float? Rating { get; set; }

        [JsonIgnore]
        public int Rank { get; set; }

        public string ToString(bool printId = true)
        {
            var sb = new StringBuilder();

            if (printId)
            {
                sb.AppendLine($"Id: {Id}");
            }

            sb.AppendLine($"Title: {Title}");
            sb.AppendLine($"Overview: {Overview}");

            if (Rating.HasValue)
            {
                sb.AppendLine($"Rating: {Rating}");
            }
            
            sb.AppendLine($"UserRank: {Rank}");

            return sb.ToString();
        }
    }
}