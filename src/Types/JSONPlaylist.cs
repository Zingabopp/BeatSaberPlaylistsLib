using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// An abstract class for a playlist storing information in a JSON file.
    /// </summary>
    public abstract class JSONPlaylist<T> : Playlist<T> where T : class, IPlaylistSong, new()
    {
        /// <summary>
        /// Additional data not deserialized into the object.
        /// </summary>
        [JsonExtensionData(ReadData = true, WriteData = false)]
        protected Dictionary<string, JToken>? ExtensionData;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (ExtensionData != null && ExtensionData.Count > 0)
            {
                OnExtensionData(ExtensionData.Select(p =>
                {
                    object value;
                    if (p.Value.Type == JTokenType.Array)
                        value = p.Value.ToObject(typeof(IList<object>)) ?? p.Value;
                    else
                        value = p.Value.ToObject<object>() ?? p.Value;

                    return new KeyValuePair<string, object>(p.Key, value);
                }));
            }
        }
    }
}
