using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeatSaberPlaylistsLib.Legacy
{
    /// <summary>
    /// An <see cref="IPlaylistSong"/> that can be serialized in a <see cref="LegacyPlaylist"/>.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LegacyPlaylistSong : JSONPlaylistSong, IEquatable<IPlaylistSong>
    {
        /// <summary>
        /// Creates an empty <see cref="LegacyPlaylistSong"/>.
        /// </summary>
        public LegacyPlaylistSong()
        { }

        /// <summary>
        /// Creates a new <see cref="LegacyPlaylistSong"/> from the given <paramref name="song"/>.
        /// </summary>
        /// <param name="song"></param>
        public LegacyPlaylistSong(IPlaylistSong song)
            : this()
        {
            this.Populate(song);
        }
        /// <summary>
        /// Creates a new <see cref="LegacyPlaylistSong"/> with the given parameters.
        /// At least one identifier must be provided (<paramref name="hash"/>, <paramref name="levelId"/>, <paramref name="songKey"/>).
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="levelId"></param>
        /// <param name="songName"></param>
        /// <param name="songKey"></param>
        /// <param name="mapper"></param>
        public LegacyPlaylistSong(string? hash = null, string? levelId = null, string? songName = null, string? songKey = null, string? mapper = null)
            : this()
        {
            if (hash != null && hash.Length > 0 && levelId != null && levelId.Length > 0)
            {
                if (levelId.StartsWith(PlaylistSong.CustomLevelIdPrefix, StringComparison.OrdinalIgnoreCase)
                    && !levelId.EndsWith(hash))
                    throw new ArgumentException(nameof(levelId), "CustomLevel levelId and hash do not match.");
            }
            LevelId = levelId;
            if (hash != null) // Don't assign null, LevelId could've set Hash
                Hash = hash;
            Name = songName;
            Key = songKey;
            LevelAuthorName = mapper;
            DateAdded = Utilities.CurrentTime;
        }

        /// <summary>
        /// Creates a new <see cref="LegacyPlaylistSong"/> from the given <see cref="ISong"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="song"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="song"/> has no valid identifiers.</exception>
        public LegacyPlaylistSong(ISong song)
            : this()
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song), "song cannot be null.");
            LevelId = song.LevelId;
            Hash = song.Hash;
            Name = song.Name;
            Key = song.Key;
            if (Identifiers == Identifier.None)
                throw new ArgumentException("song has no identifiers.", nameof(song));
            LevelAuthorName = song.LevelAuthorName;
            DateAdded = Utilities.CurrentTime;
        }

        /// <summary>
        /// Custom data not included in the schema. Returns null if there are no entries.
        /// </summary>
        [JsonProperty("customData", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object>? _serializedCustomData
        {
            get => CustomData;
            set => CustomData = value;
        }

        ///<inheritdoc/>
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore, Order = -10)]
        private string? _serializedKey
        {
            get => Key;
            set => Key = value;
        }

        ///<inheritdoc/>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore, Order = -7)]
        private string? _serializedHash
        {
            get => Hash;
            set => Hash = value;
        }
        ///<inheritdoc/>
        [JsonProperty("levelid", NullValueHandling = NullValueHandling.Ignore, Order = -6)]
        private string? _serializedLevelId
        {
            get => LevelId;
            set => LevelId = value;
        }


        ///<inheritdoc/>
        [JsonProperty("songName", NullValueHandling = NullValueHandling.Ignore, Order = -9)]
        public string? _serializedName
        {
            get => Name;
            set => Name = value;
        }

        ///<inheritdoc/>
        [JsonProperty("levelAuthorName", NullValueHandling = NullValueHandling.Ignore, Order = -8)]
        public string? _serializedLevelAuthorName
        {
            get => LevelAuthorName;
            set => LevelAuthorName = value;
        }

        ///<inheritdoc/>
        [JsonProperty("dateAdded", NullValueHandling = NullValueHandling.Ignore, Order = 10)]
        private DateTime? _serializedDate
        {
            get => DateAdded;
            set => DateAdded = value;
        }

        /// <summary>
        /// The optional recommended difficulties for the beatmap
        /// </summary>
        [JsonProperty("difficulties", NullValueHandling = NullValueHandling.Ignore, Order = 12)]
        public List<Difficulty>? _serializedDifficulties
        {
            get => Difficulties;
            set => Difficulties = value;
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            string? keyPart = string.IsNullOrEmpty(Key) ? string.Empty : $"({Key}) ";
            return $"{keyPart}{Name} by {LevelAuthorName}";
        }

        ///<inheritdoc/>
        public override bool Equals(IPlaylistSong other)
        {
            if (other == null)
                return false;
            return LevelId == other?.LevelId;
        }
    }
}
