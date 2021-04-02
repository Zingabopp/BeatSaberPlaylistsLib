using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    public abstract partial class Playlist : INotifyCoverChanged
    {
        /// <inheritdoc/>
        public event EventHandler? PlaylistChanged;
        /// <inheritdoc/>
        public event EventHandler? CoverImageChanged;

        /// <inheritdoc/>
        public abstract string Title { get; set; }
        /// <inheritdoc/>
        public abstract string? Author { get; set; }
        /// <inheritdoc/>
        public abstract string? Description { get; set; }
        /// <inheritdoc/>
        public virtual string Filename { get; set; } = "";
        /// <inheritdoc/>
        public string? SuggestedExtension { get; set; }
        /// <summary>
        /// Unique identifier for the playlist, used for distinguishing between duplicates.
        /// </summary>
        public Guid playlistID { get; } = Guid.NewGuid();
        /// <inheritdoc/>
        public bool AllowDuplicates
        {
            get
            {
                object returnVal;
                return TryGetCustomData("AllowDuplicates", out returnVal) ? (bool)returnVal : true;
            }
            set => SetCustomData("AllowDuplicates", value);
        }

        /// <inheritdoc/>
        public virtual bool IsReadOnly => false;

        /// <inheritdoc/>
        public abstract bool HasCover { get; }

        /// <inheritdoc/>
        public abstract Stream GetCoverStream();

        /// <inheritdoc/>
        public abstract void SetCover(byte[] coverImage);

        /// <inheritdoc/>
        public abstract void SetCover(string? coverImageStr);

        /// <inheritdoc/>
        public abstract void SetCover(Stream stream);

        /// <summary>
        /// Action to take when additional data that wasn't deserialized is found.
        /// </summary>
        /// <param name="extensionData"></param>
        protected virtual void OnExtensionData(IEnumerable<KeyValuePair<string, object>> extensionData)
        {
            if (CustomData == null)
                CustomData = new Dictionary<string, object>();
            foreach (var item in extensionData)
            {
                string key = item.Key;
                if (!CustomData.ContainsKey(key))
                    CustomData[key] = item.Value;
            }
        }

        /// <inheritdoc/>
        public void RaisePlaylistChanged()
        {
            EventHandler? handler = PlaylistChanged;
            handler?.Invoke(this, null);
        }

        /// <summary>
        /// Raises the <see cref="CoverImageChanged"/> event.
        /// </summary>
        protected void RaiseCoverImageChanged()
        {
#if BeatSaber
            _previousSprite = _sprite;
            _sprite = null;

#endif
            CoverImageChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Dictionary for the CustomData key in the playlist file.
        /// </summary>
        protected Dictionary<string, object>? CustomData { get; set; }

        /// <inheritdoc/>
        public bool TryGetCustomData(string key, out object value)
        {
            value = null!;
            return CustomData?.TryGetValue(key, out value) ?? false;
        }

        /// <inheritdoc/>
        public void SetCustomData(string key, object value)
        {
            if (CustomData == null)
                CustomData = new Dictionary<string, object>();
            if (key.Equals("AllowDuplicates", StringComparison.OrdinalIgnoreCase))
            {
                if (!(value is bool))
                    throw new ArgumentException("'AllowDuplicates' must have a boolean value.", nameof(value));
            }
            CustomData[key] = value;
        }
    }


    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class Playlist<T> : Playlist, IPlaylist<T>
        where T : class, IPlaylistSong, new()
    {
        /// <summary>
        /// Internal collection of songs in the playlist.
        /// </summary>
        protected List<T> Songs { get; set; } = new List<T>();


        /// <inheritdoc/>
        public IPlaylistSong this[int index]
        {
            get => Songs[index];
            set
            {
                Songs[index] = CreateFrom(value);
            }
        }


        /// <inheritdoc/>
        public int Count => Songs.Count;

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> of type <typeparamref name="T"/> from the given <paramref name="song"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        protected abstract T CreateFrom(ISong song);

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(ISong song)
        {
            if (AllowDuplicates || !Songs.Any(s => s.Hash == song.Hash || (song.Key != null && s.Key == song.Key)))
            {
                T playlistSong = CreateFrom(song);
                Songs.Add(playlistSong);
                return playlistSong;
            }
            return null;
        }

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(string songHash, string? songName, string? songKey, string? mapper) =>
            Add(new T()
            {
                Hash = songHash,
                Name = songName,
                Key = songKey,
                LevelAuthorName = mapper
            });

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(IPlaylistSong item) => Add((ISong)item);

        /// <inheritdoc/>
        public virtual void Clear()
        {
            Songs.Clear();
        }

        /// <inheritdoc/>
        public virtual void Sort()
        {
            Songs = Songs.OrderByDescending(s => s.DateAdded).ToList();
        }

        /// <inheritdoc/>
        public virtual bool Contains(IPlaylistSong item)
        {
            return Songs.Any(s => s.Equals(item));
        }

        /// <inheritdoc/>
        public void CopyTo(IPlaylistSong[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (T song in Songs)
            {
                array[index] = song;
                index++;
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that iterates through the playlist's songs.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Songs.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(IPlaylistSong item)
        {
            if (item is T song)
                return Songs.IndexOf(song);
            else
                return -1;
        }

        /// <inheritdoc/>
        public void Insert(int index, IPlaylistSong item)
        {
            Songs.Insert(index, CreateFrom(item));
        }

        /// <inheritdoc/>
        public bool Remove(IPlaylistSong item)
        {
            bool songRemoved = false;
            if (item is T matchedType)
                songRemoved = Songs.Remove(matchedType);
            else
            {
                T song = Songs.FirstOrDefault(s => s.Equals(item));
                if (song != null)
                    songRemoved = Songs.Remove(song);
            }
            return songRemoved;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            Songs.RemoveAt(index);
        }

        /// <inheritdoc/>
        public int RemoveAll(Func<IPlaylistSong, bool> match)
        {
            int songsRemoved = 0;
            T[]? toRemove = Songs.Where(s => match(s)).ToArray();
            foreach (T song in toRemove)
            {
                if (Songs.Remove(song))
                    songsRemoved++;
            }
            return songsRemoved;
        }

        /// <inheritdoc/>
        public virtual int RemoveAll(Func<T, bool> match)
        {
            int removedSongs = 0;
            if (match != null)
                removedSongs = Songs.RemoveAll(s => match(s));
            return removedSongs;
        }

        /// <inheritdoc/>
        public void RemoveDuplicates()
        {
            int previousCount = Songs.Count;
            Songs = Songs.Distinct(IPlaylistSongComparer<T>.Default).ToList();
            if (Songs.Count != previousCount)
                RaisePlaylistChanged();
        }


        /// <inheritdoc/>
        public bool TryRemoveByHash(string songHash)
        {
            songHash = songHash.ToUpper();
            return RemoveAll((IPlaylistSong s) => s.Hash == songHash) > 0;
        }

        /// <inheritdoc/>
        public bool TryRemoveByKey(string songKey)
        {
            songKey = songKey.ToLower();
            return RemoveAll((IPlaylistSong s) => s.Key == songKey) > 0;
        }

        /// <inheritdoc/>
        public bool TryRemove(IPlaylistSong song)
        {
            if (song == null)
                return false;
            return RemoveAll((IPlaylistSong s) => s == song) > 0;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Songs.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<IPlaylistSong> IEnumerable<IPlaylistSong>.GetEnumerator()
        {
            foreach (var song in Songs)
            {
                yield return song;
            }
        }

        /// <inheritdoc/>
        void ICollection<IPlaylistSong>.Add(IPlaylistSong item)
        {
            Add(item);
        }
    }
}
