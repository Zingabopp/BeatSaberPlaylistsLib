using System;
using System.Collections.Generic;

using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BeatSaberPlaylistsLib.Blist.Converters;
using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Schema: https://github.com/raftario/blist/blob/master/playlist.schema.json
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

        ///<inheritdoc/>
        [JsonProperty("customData", NullValueHandling = NullValueHandling.Ignore)]
        public override Dictionary<string, object>? CustomData { get; set; }


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


        private byte[]? _coverData;

        /// <summary>
        /// Raw data for the cover image.
        /// </summary>
        protected byte[]? CoverData
        {
            get => _coverData;
            set
            {
                _coverData = value;
                RaiseCoverImageChanged();
            }
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
