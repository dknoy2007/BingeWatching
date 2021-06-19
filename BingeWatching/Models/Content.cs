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
        public int? Rank { get; set; }

        public string ToString(bool printId = true)
        {
            var sb = new StringBuilder();

            if (printId)
            {
                sb.AppendLine($"Id:\n{Id}");
                sb.AppendLine();
            }

            sb.AppendLine($"Title:\n{Title}");
            sb.AppendLine();
            sb.AppendLine($"Overview:\n{Overview}");
            sb.AppendLine();
            sb.AppendLine($"Rating:\n{Rating}");
            sb.AppendLine();
            sb.AppendLine($"UserRank:\n{Rank}");

            return sb.ToString();
        }
    }
}