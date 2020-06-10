using System;
using System.Collections.Generic;

using System.Globalization;
using System.IO;
using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeatSaberPlaylistsLib.Blister
{
    /// <summary>
    /// A Beat Saber playlist
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class BlisterPlaylist : Playlist<BlisterPlaylistSong>
    {
        /// <summary>
        /// Creates an empty <see cref="BlisterPlaylist"/>.
        /// </summary>
        protected BlisterPlaylist()
        { }

        /// <summary>
        /// Creates a new <see cref="BlisterPlaylist"/> from the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        public BlisterPlaylist(string fileName, string title, string? author)
        {
            Filename = fileName;
            Title = title;
            Author = author;
        }

        /// <summary>
        /// The playlist author
        /// </summary>
        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public override string? Author { get; set; }

        /// <summary>
        /// The filename of the optional playlist cover image
        /// </summary>
        [JsonProperty("cover", NullValueHandling = NullValueHandling.Ignore)]
        public string? Cover { get; set; }

        /// <summary>
        /// Custom data not included in the schema
        /// </summary>
        [JsonProperty("customData", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object>? CustomData { get; set; }

        /// <summary>
        /// The optional playlist description
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MinMaxLengthCheckConverter))]
        public override string? Description { get; set; }

        /// <summary>
        /// The beatmaps contained in the playlist
        /// </summary>
        [JsonProperty("maps")]
        protected List<BlisterPlaylistSong> _serializedSongs
        {
            get => Songs;
            set { Songs = value ?? new List<BlisterPlaylistSong>(); }
        }

        /// <summary>
        /// The playlist title
        /// </summary>
        [JsonProperty("title")]
        public override string Title { get; set; } = "";

        protected override BlisterPlaylistSong CreateFrom(ISong song)
        {
            if (song is BlisterPlaylistSong legacySong)
                return legacySong;
            return new BlisterPlaylistSong(song);
        }

        public override Stream GetCoverStream()
        {
            return new MemoryStream(CoverData ?? Array.Empty<byte>());
        }

        public override void SetCover(byte[] coverImage)
        {
            CoverData = coverImage?.Clone() as byte[];
        }

        public override void SetCover(string? coverImageStr)
        {
            if (coverImageStr != null && coverImageStr.Length > 0)
                CoverData = Utilities.Base64ToByteArray(coverImageStr);
            else
                CoverData = null;
        }

        public override void SetCover(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                CoverData = null;
            else if (stream is MemoryStream cast)
            {
                CoverData = cast.ToArray();
            }
            else
            {
                using MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                CoverData = ms.ToArray();
            }
        }

        public bool HasCover => (CoverData?.Length ?? 0) > 0;

        protected byte[]? CoverData;
    }

    /// <summary>
    /// A beatmap difficulty
    /// </summary>
    public partial class Difficulty
    {
        /// <summary>
        /// The characteristic name
        /// </summary>
        [JsonProperty("characteristic")]
        public string Characteristic { get; set; }

        /// <summary>
        /// The difficulty name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// The entry type defining how the beatmap is identified in the playlist
    /// </summary>
    public enum TypeEnum { Hash, Key, LevelId };

    public partial class BlisterPlaylist
    {
        public static BlisterPlaylist FromJson(string json) => JsonConvert.DeserializeObject<BlisterPlaylist>(json, BeatSaberPlaylistsLib.Blister.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this BlisterPlaylist self) => JsonConvert.SerializeObject(self, BeatSaberPlaylistsLib.Blister.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class MinMaxLengthCheckConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            if (value.Length >= 1)
            {
                return value;
            }
            throw new Exception("Cannot unmarshal type string");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (string)untypedValue;
            if (value.Length >= 1)
            {
                serializer.Serialize(writer, value);
                return;
            }
            throw new Exception("Cannot marshal type string");
        }

        public static readonly MinMaxLengthCheckConverter Singleton = new MinMaxLengthCheckConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "hash":
                    return TypeEnum.Hash;
                case "key":
                    return TypeEnum.Key;
                case "levelID":
                    return TypeEnum.LevelId;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Hash:
                    serializer.Serialize(writer, "hash");
                    return;
                case TypeEnum.Key:
                    serializer.Serialize(writer, "key");
                    return;
                case TypeEnum.LevelId:
                    serializer.Serialize(writer, "levelID");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}
