using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLib.Blister
{
    /// <summary>
    /// An <see cref="IPlaylistSong"/> that can be serialized in a <see cref="BlisterPlaylist"/>.
    /// </summary>
    public class BlisterPlaylistSong : PlaylistSong
    {
        /// <summary>
        /// Creates an empty <see cref="BlisterPlaylistSong"/>.
        /// </summary>
        public BlisterPlaylistSong()
        { }

        /// <summary>
        /// Creates a new <see cref="BlisterPlaylistSong"/> from the given <paramref name="song"/>.
        /// </summary>
        /// <param name="song"></param>
        public BlisterPlaylistSong(ISong song)
            : this()
        {
            this.Populate(song);
        }

        /// <summary>
        /// Custom data not included in the schema
        /// </summary>
        [JsonProperty("customData", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object>? CustomData { get; set; }

        /// <summary>
        /// The optional RFC3339 date and time the beatmap was added to the playlist
        /// </summary>
        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// The optional recommended difficulties for the beatmap
        /// </summary>
        [JsonProperty("difficulties", NullValueHandling = NullValueHandling.Ignore)]
        public List<Difficulty>? Difficulties { get; set; }

        /// <summary>
        /// The SHA1 hash of the beatmap
        /// </summary>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        protected string? _serializedHash
        {
            get => Hash;
            set => Hash = value;
        }

        /// <summary>
        /// The BeatSaver key of the beatmap
        /// </summary>
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        protected string? _serializedKey
        {
            get => Key;
            set => Key = value;
        }

        /// <summary>
        /// The level ID of the BeatMap
        /// </summary>
        [JsonProperty("levelID", NullValueHandling = NullValueHandling.Ignore)]
        protected string? _serializedLevelId
        {
            get => LevelId;
            set => LevelId = value;
        }

        /// <summary>
        /// The entry type defining how the beatmap is identified in the playlist
        /// </summary>
        [JsonProperty("type")]
        public Identifier Type { get; set; }


        ///<inheritdoc/>
        public override bool Equals(IPlaylistSong other)
        {
            if (other == null)
                return false;
            return Hash == other?.Hash;
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            string? keyPart = string.IsNullOrEmpty(Key) ? string.Empty : $"({Key}) ";
            return $"{keyPart}{Name} by {LevelAuthorName}";
        }
    }
}
