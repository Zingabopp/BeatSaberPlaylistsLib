using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLib.Blist.Converters
{
    internal class PlaylistTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(BlistPlaylistType) || t == typeof(BlistPlaylistType?);

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "hash":
                    return BlistPlaylistType.Hash;
                case "key":
                    return BlistPlaylistType.Key;
                case "levelID":
                    return BlistPlaylistType.LevelId;
            }
            return BlistPlaylistType.None;
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (BlistPlaylistType)untypedValue;
            switch (value)
            {
                case BlistPlaylistType.Hash:
                    serializer.Serialize(writer, "hash");
                    return;
                case BlistPlaylistType.Key:
                    serializer.Serialize(writer, "key");
                    return;
                case BlistPlaylistType.LevelId:
                    serializer.Serialize(writer, "levelID");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly PlaylistTypeConverter Singleton = new PlaylistTypeConverter();
    }
}
