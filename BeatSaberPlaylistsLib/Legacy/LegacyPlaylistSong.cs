using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using System;

namespace BeatSaberPlaylistsLib.Legacy
{
    /// <summary>
    /// An <see cref="IPlaylistSong"/> that can be serialized in a <see cref="LegacyPlaylist"/>.
    /// </summary>
    public class LegacyPlaylistSong : IPlaylistSong, IEquatable<IPlaylistSong>
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
        /// <exception cref="ArgumentException">Thrown if no identifiers are provided (<paramref name="hash"/>, <paramref name="levelId"/>, <paramref name="songKey"/>).</exception>
        public LegacyPlaylistSong(string? hash = null, string? levelId = null, string? songName = null, string? songKey = null, string? mapper = null)
            : this()
        {
            LevelId = levelId;
            Hash = hash;
            Name = songName;
            Key = songKey;
            if (Identifiers == Identifier.None)
                throw new ArgumentException("song has no identifiers.");
            LevelAuthorName = mapper;
            DateAdded = DateTime.Now;
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
            DateAdded = DateTime.Now;
        }


        ///<inheritdoc/>
        [JsonProperty("hash", Order = -9)]
        public string? Hash
        {
            get
            {
                return _hash;
            }
            set
            {
                if (_hash == value)
                    return;
                if (!string.IsNullOrEmpty(value))
                {
                    _hash = value;
                    AddIdentifierFlag(Identifier.Hash);
                }
                else
                {
                    RemoveIdentifierFlag(Identifier.Hash);
                }
            }
        }
        ///<inheritdoc/>
        [JsonProperty("levelid")]
        public string? LevelId
        {
            get
            {
                if (_levelId != null && _levelId.Length > 0)
                    return _levelId;
                else if (_hash != null && _hash.Length > 0)
                    return PlaylistSong.CustomLevelIdPrefix + Hash;
                return null;
            }
            set
            {
                if (_levelId == value)
                    return;
                if (!string.IsNullOrEmpty(value))
                {
                    _levelId = value;
                    AddIdentifierFlag(Identifier.LevelId);
                }
                else
                {
                    RemoveIdentifierFlag(Identifier.LevelId);
                }
            }
        }


        ///<inheritdoc/>
        [JsonProperty("key", Order = -10)]
        public string? Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_key == value)
                    return;
                if (!string.IsNullOrEmpty(value))
                {
                    _key = value;
                    AddIdentifierFlag(Identifier.Key);
                }
                else
                {
                    RemoveIdentifierFlag(Identifier.Key);
                }
            }
        }

        ///<inheritdoc/>
        [JsonProperty("songName", Order = -8)]
        public string? Name { get; set; }

        ///<inheritdoc/>
        [JsonProperty("levelAuthorName")]
        public string? LevelAuthorName { get; set; }

        ///<inheritdoc/>
        [JsonProperty("dateAdded", Order = -7)]
        public DateTime? DateAdded { get; set; }

        ///<inheritdoc/>
        public Identifier Identifiers { get; protected set; }


        [JsonIgnore]
        private string? _hash;
        [JsonIgnore]
        private string? _levelId;
        [JsonIgnore]
        private string? _key;

        /// <summary>
        /// Adds the given flag to the <see cref="Identifiers"/> property.
        /// </summary>
        /// <param name="identifier"></param>
        protected void AddIdentifierFlag(Identifier identifier)
        {
            Identifiers |= identifier;
        }
        /// <summary>
        /// Removes the given flag from the <see cref="Identifiers"/> property.
        /// </summary>
        /// <param name="identifier"></param>
        protected void RemoveIdentifierFlag(Identifier identifier)
        {
            Identifiers &= ~identifier;
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            string? keyPart = string.IsNullOrEmpty(Key) ? string.Empty : $"({Key}) ";
            return $"{keyPart}{Name} by {LevelAuthorName}";
        }

        ///<inheritdoc/>
        public bool Equals(IPlaylistSong other)
        {
            if (other == null)
                return false;
            return Hash == other?.Hash;
        }
    }
}
