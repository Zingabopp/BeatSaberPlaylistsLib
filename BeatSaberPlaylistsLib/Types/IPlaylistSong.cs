using System;
using System.Collections.Generic;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// An interface for a basic playlist song.
    /// </summary>
    public interface IPlaylistSong : ISong, IEquatable<IPlaylistSong>
    {
        /// <summary>
        /// Date and time the song added to the playlist.
        /// </summary>
        DateTime? DateAdded { get; set; }
        /// <summary>
        /// Unique identifier for playlist song, used for distinguishing between duplicates.
        /// </summary>
        public Guid playlistSongID { get; }
        /// <summary>
        /// The optional recommended difficulties for the beatmap
        /// </summary>
        public List<Difficulty>? Difficulties { get; set; }
    }
}
