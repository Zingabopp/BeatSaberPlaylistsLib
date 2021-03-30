using System;
using System.Collections.Generic;
using System.IO;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Interface for a playlist.
    /// </summary>
    public partial interface IPlaylist : IList<IPlaylistSong>
    {
        /// <summary>
        /// Playlist title.
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Playlist author.
        /// </summary>
        string? Author { get; set; }
        /// <summary>
        /// Playlist description.
        /// </summary>
        string? Description { get; set; }
        /// <summary>
        /// Filename without extension, does not include directory path.
        /// </summary>
        string Filename { get; set; }
        /// <summary>
        /// Suggested file extension for the playlist. May be null.
        /// </summary>
        string? SuggestedExtension { get; set; }

        /// <summary>
        /// True if a playlist cover image is available.
        /// </summary>
        bool HasCover { get; }
        /// <summary>
        /// Returns a <see cref="Stream"/> for the playlist cover image.
        /// </summary>
        /// <returns></returns>
        Stream? GetCoverStream();
        /// <summary>
        /// Sets the cover image from a byte array.
        /// </summary>
        /// <param name="coverImage"></param>
        void SetCover(byte[] coverImage);
        /// <summary>
        /// Sets the cover image from a base64 string.
        /// </summary>
        /// <param name="coverImageStr"></param>
        void SetCover(string coverImageStr);
        /// <summary>
        /// Sets the cover image from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        void SetCover(Stream stream);
        /// <summary>
        /// Allow duplicate songs in the playlist.
        /// </summary>
        bool AllowDuplicates { get; set; }
        /// <summary>
        /// Adds the <see cref="ISong"/> to the playlist. 
        /// Does nothing if <see cref="AllowDuplicates"/> is false and the song is already in the playlist. 
        /// Converts the <see cref="ISong"/> if needed.
        /// </summary>
        /// <param name="song"></param>
        /// <returns>The added IPlaylistSong, null if nothing was added.</returns>
        IPlaylistSong? Add(ISong song);

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> and adds it to the playlist.
        /// Does nothing if <see cref="AllowDuplicates"/> is false and the song is already in the playlist. 
        /// </summary>
        /// <param name="songHash"></param>
        /// <param name="songName"></param>
        /// <param name="songKey"></param>
        /// <param name="mapper"></param>
        /// <returns>The added IPlaylistSong, null if nothing was added.</returns>
        IPlaylistSong? Add(string songHash, string? songName, string? songKey, string? mapper);
        /// <summary>
        /// Tries to remove all songs with the given hash from the playlist. Returns true if successful.
        /// </summary>
        /// <param name="songHash"></param>
        /// <returns></returns>
        bool TryRemoveByHash(string songHash);
        /// <summary>
        /// Tries to remove all songs with the given key from the playlist. Returns true if successful.
        /// </summary>
        /// <param name="songKey"></param>
        /// <returns></returns>
        bool TryRemoveByKey(string songKey);
        /// <summary>
        /// Tries to remove the given song from the playlist. Returns true if successful.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        bool TryRemove(IPlaylistSong song);
        /// <summary>
        /// Removes all duplicate songs from the playlist.
        /// </summary>
        void RemoveDuplicates();
        /// <summary>
        /// Raised when <see cref="RaisePlaylistChanged"/> is called.
        /// </summary>
        event EventHandler? PlaylistChanged;
        /// <summary>
        /// Raises the PlaylistChanged event.
        /// </summary>
        void RaisePlaylistChanged();

        /// <summary>
        /// Sorts the playlist in descending order of DateAdded.
        /// </summary>
        void Sort();

        /// <summary>
        /// Removes all songs matched by the predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        int RemoveAll(Func<IPlaylistSong, bool> match);

        /// <summary>
        /// Tries to access the object for key in <see cref="CustomData"/> if found and returns true. Else, returns false.
        /// </summary>
        public bool TryGetCustomData(string key, out object value);

        /// <summary>
        /// Sets value for key in <see cref="CustomData"/>.
        /// </summary>
        public void SetCustomData(string key, object value);
    }

    /// <summary>
    /// Interface for an <see cref="IPlaylist"/> that uses type <typeparamref name="T"/> as its <see cref="IPlaylistSong"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPlaylist<T> : IPlaylist
        where T : IPlaylistSong, new()
    {
        /// <summary>
        /// Removes all songs matched by the predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        int RemoveAll(Func<T, bool> match);
    }
}
