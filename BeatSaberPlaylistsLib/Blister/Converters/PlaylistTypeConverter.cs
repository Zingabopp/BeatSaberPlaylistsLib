using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLib.Blister.Converters
{
    internal class PlaylistTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(BlisterPlaylistType) || t == typeof(BlisterPlaylistType?);

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "hash":
                    return BlisterPlaylistType.Hash;
                case "key":
                    return BlisterPlaylistType.Key;
                case "levelID":
                    return BlisterPlaylistType.LevelId;
            }
            return BlisterPlaylistType.None;
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (BlisterPlaylistType)untypedValue;
            switch (value)
            {
                case BlisterPlaylistType.Hash:
                    serializer.Serialize(writer, "hash");
                    return;
                case BlisterPlaylistType.Key:
                    serializer.Serialize(writer, "key");
                    return;
                case BlisterPlaylistType.LevelId:
                    serializer.Serialize(writer, "levelID");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly PlaylistTypeConverter Singleton = new PlaylistTypeConverter();
    }
}
