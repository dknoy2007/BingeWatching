using System.Runtime.Serialization;

namespace BingeWatching.Models.Enums
{
    public enum ContentType
    {
        [EnumMember(Value = "m")]
        Movie,

        [EnumMember(Value = "s")]
        TvShow
    }
}