using System.Collections.Generic;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Compares two <see cref="IPlaylistSong"/> using their <see cref="ISong.LevelId"/>.
    /// Falls back to using <see cref="ISong.Key"/> if <see cref="ISong.LevelId"/> is null.
    /// </summary>
    public class IPlaylistSongComparer : IEqualityComparer<IPlaylistSong>
    {
        /// <summary>
        /// Default instance of <see cref="IPlaylistSongComparer"/>.
        /// </summary>
        public static readonly IPlaylistSongComparer Default = new IPlaylistSongComparer();

        /// <summary>
        /// Compares two <see cref="IPlaylistSong"/> using their <see cref="ISong.LevelId"/>.
        /// Falls back to using <see cref="ISong.Key"/> if <see cref="ISong.LevelId"/> is null.
        /// </summary>
        public bool Equals(IPlaylistSong x, IPlaylistSong y)
        {
            if (x == null)
                return y == null;
            if (y == null)
                return x == null;
            if (GetHashCode(x) != GetHashCode(y))
                return false;
            string? levelId = x.LevelId;
            if (levelId != null)
                return levelId == y.LevelId;
            if (x.Key != null)
                return x.Key == y.Key;
            return false;
        }

        ///<inheritdoc/>
        public int GetHashCode(IPlaylistSong obj)
        {
            int hash = 238947239;
            if (obj != null)
            {
                string? levelId = obj.LevelId;
                if (levelId != null)
                    hash ^= levelId.GetHashCode();
                else if (obj.Key != null)
                    hash ^= obj.Key.GetHashCode();
            }
            return hash;
        }
    }

    ///<inheritdoc/>
    public class IPlaylistSongComparer<T> : IPlaylistSongComparer, IEqualityComparer<T>
        where T : class, IPlaylistSong, new()
    {
        /// <summary>
        /// Default instance of <see cref="IPlaylistSongComparer"/>.
        /// </summary>
        public static new readonly IPlaylistSongComparer<T> Default = new IPlaylistSongComparer<T>();

        ///<inheritdoc/>
        public bool Equals(T x, T y)
        {
            return base.Equals(x, y);
        }

        ///<inheritdoc/>
        public int GetHashCode(T obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
