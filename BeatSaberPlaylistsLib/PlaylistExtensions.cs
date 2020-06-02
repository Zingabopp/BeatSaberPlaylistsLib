using BeatSaberPlaylistsLib.Types;
using System;

namespace BeatSaberPlaylistsLib
{
    /// <summary>
    /// Helpful extension methods for playlists.
    /// </summary>
    public static class PlaylistExtensions
    {
        /// <summary>
        /// Creates and returns a new <see cref="IPlaylistSong"/> of type <typeparamref name="T"/> with values populated from <paramref name="song"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="IPlaylistSong"/> type.</typeparam>
        /// <param name="song">Song to clone the values from.</param>
        /// <returns>A new <see cref="IPlaylistSong"/> of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="song"/> is null.</exception>
        public static T ConvertTo<T>(this IPlaylistSong song) where T : IPlaylistSong, new()
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song), "song cannot be null");
            T ret = new T();
            ret.Populate(song);
            return ret;
        }

        /// <summary>
        /// Populates the target <see cref="IPlaylistSong"/> with values from <paramref name="song"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="song"></param>
        /// <param name="overwriteTarget">If true, overwrites existing values on <paramref name="target"/></param>
        public static void Populate(this IPlaylistSong target, ISong song, bool overwriteTarget = false)
        {
            if (target == null) throw new ArgumentNullException(nameof(target), "target song cannot be null.");
            if (song == null) throw new ArgumentNullException(nameof(song), "source song cannot be null.");
            if (overwriteTarget)
            {
                target.LevelId = song.LevelId;
                target.Hash = song.Hash;
                target.Key = song.Key;
                if (song is IPlaylistSong playlistSong)
                    target.DateAdded = playlistSong.DateAdded;
                else
                    target.DateAdded = DateTime.Now;
                target.Name = song.Name;
                target.LevelAuthorName = song.LevelAuthorName;
            }
            else
            {
                target.LevelId ??= song.LevelId;
                target.Hash ??= song.Hash;
                target.Key ??= song.Key;
                if (song is IPlaylistSong playlistSong)
                    target.DateAdded ??= playlistSong.DateAdded;
                else
                    target.DateAdded ??= DateTime.Now;
                target.Name ??= song.Name;
                target.LevelAuthorName ??= song.LevelAuthorName;
            }
        }
    }
}
