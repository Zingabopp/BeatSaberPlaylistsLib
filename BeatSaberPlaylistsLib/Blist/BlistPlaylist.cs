using System;
using System.Collections.Generic;

using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using BeatSaberPlaylistsLib.Blist.Converters;
using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeatSaberPlaylistsLib.Blist
{
    /// <summary>
    /// A Beat Saber playlist
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class BlistPlaylist : Playlist<BlistPlaylistSong>
    {
        /// <summary>
        /// Creates an empty <see cref="BlistPlaylist"/>.
        /// </summary>
        protected BlistPlaylist()
        { }

        /// <summary>
        /// Creates a new <see cref="BlistPlaylist"/> from the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        public BlistPlaylist(string fileName, string title, string? author)
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


        private string? _description;
        /// <summary>
        /// The optional playlist description
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public override string? Description
        {
            get => string.IsNullOrEmpty(_description) ? null : _description;
            set => _description = string.IsNullOrEmpty(value) ? null : value;
        }
        /// <summary>
        /// The beatmaps contained in the playlist
        /// </summary>
        [JsonProperty("maps")]
        protected List<BlistPlaylistSong> _serializedSongs
        {
            get => Songs;
            set { Songs = value ?? new List<BlistPlaylistSong>(); }
        }

        /// <summary>
        /// The playlist title
        /// </summary>
        [JsonProperty("title")]
        public override string Title { get; set; } = "";

        ///<inheritdoc/>
        protected override BlistPlaylistSong CreateFrom(ISong song)
        {
            if (song is BlistPlaylistSong legacySong)
                return legacySong;
            return new BlistPlaylistSong(song);
        }

        ///<inheritdoc/>
        public override Stream GetCoverStream()
        {
            return new MemoryStream(CoverData ?? Array.Empty<byte>());
        }

        ///<inheritdoc/>
        public override void SetCover(byte[] coverImage)
        {
            CoverData = coverImage?.Clone() as byte[];
        }

        ///<inheritdoc/>
        public override void SetCover(string? coverImageStr)
        {
            if (coverImageStr != null && coverImageStr.Length > 0)
                CoverData = Utilities.Base64ToByteArray(coverImageStr);
            else
                CoverData = null;
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public override bool HasCover => (CoverData?.Length ?? 0) > 0;

        /// <summary>
        /// Raw data for the cover image.
        /// </summary>
        protected byte[]? CoverData;
    }

    /// <summary>
    /// A beatmap difficulty
    /// </summary>
    public struct Difficulty
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

        ///<inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Difficulty diff)
            {
                if (Characteristic?.Equals(diff.Characteristic, StringComparison.OrdinalIgnoreCase) ?? diff.Characteristic != null)
                    return false;
                if (Name?.Equals(diff.Name, StringComparison.OrdinalIgnoreCase) ?? diff.Name != null)
                    return false;
                return true;
            }
            return false;
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            int hash = 238947239;
            hash ^= Characteristic?.GetHashCode() ?? 23408234;
            hash ^= Name?.GetHashCode() ?? 12987213;
            return hash;
        }

        ///<inheritdoc/>
        public static bool operator ==(Difficulty left, Difficulty right)
        {
            return left.Equals(right);
        }

        ///<inheritdoc/>
        public static bool operator !=(Difficulty left, Difficulty right)
        {
            return !(left.Equals(right));
        }
    }

    /// <summary>
    /// The entry type defining how the beatmap is identified in the playlist
    /// </summary>
    public enum BlistPlaylistType
    {
        /// <summary>
        /// No known type.
        /// </summary>
        None,
        /// <summary>
        /// Song is identified by its Hash.
        /// </summary>
        Hash,
        /// <summary>
        /// Song is identified by its Key.
        /// </summary>
        Key,
        /// <summary>
        /// Song is identified by its LevelId.
        /// </summary>
        LevelId
    };
}
