using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                if (TryGetCustomData("AllowDuplicates", out object? returnVal) && returnVal is bool boolVal)
                    return boolVal;
                return  true;
            }
            set => SetCustomData("AllowDuplicates", value);
        }

        /// <inheritdoc/>
        public bool ReadOnly
        {
            get
            {
                if (TryGetCustomData("ReadOnly", out object? returnVal) && returnVal is bool boolVal)
                    return boolVal;
                return false;
            }
            set => SetCustomData("ReadOnly", value);
        }

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
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="CoverImageChanged"/> event.
        /// </summary>
        protected void RaiseCoverImageChanged()
        {
            ResetSprite();
            CoverImageChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Used in the Beat Saber mod version.
        /// </summary>
        partial void ResetSprite();

        /// <summary>
        /// Dictionary for the CustomData key in the playlist file.
        /// </summary>
        protected Dictionary<string, object>? CustomData { get; set; }

        /// <inheritdoc/>
        public bool TryGetCustomData(string key, out object? value)
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

        #region Default Cover

        protected byte[]? _defaultCoverData;
        
        /// <summary>
        /// Get Stream for Default Cover if Cover is not set
        /// </summary>
        /// <returns></returns>
        public virtual Task<Stream?> GetDefaultCoverStream() => 
            _defaultCoverData != null ? Task.FromResult<Stream?>(new MemoryStream(_defaultCoverData)) : Task.FromResult<Stream?>(null);

#if BeatSaber
#else

        /// <summary>
        /// Set the Default Cover data after generating it
        /// </summary>
        /// <param name="coverStream"></param>
        public async Task SetDefaultCover(Stream coverStream)
        {
            using MemoryStream ms = new MemoryStream();
            await coverStream.CopyToAsync(ms);
            _defaultCoverData = ms.ToArray();
        }
#endif
        
        /// <summary>
        /// Raises cover image changed if we are using default image. Called when the level collection changes.
        /// </summary>
        public void RaiseCoverImageChangedForDefaultCover()
        {
            if (!HasCover)
            {
                RaiseCoverImageChanged();
                _defaultCoverData = null;
#if BeatSaber
                _ = Sprite;
#endif
            }
        }

        #endregion
    }


    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class Playlist<T> : Playlist, IPlaylist<T>
        where T : class, IPlaylistSong
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

        /// <inheritdoc/>
        public virtual bool IsReadOnly => ReadOnly;

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> of type <typeparamref name="T"/> from the given <paramref name="song"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        protected abstract T CreateFrom(ISong song);

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> of type <typeparamref name="T"/> from the given values.
        /// </summary>
        /// <param name="songHash"></param>
        /// <param name="songName"></param>
        /// <param name="songKey"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        protected abstract T CreateFromByHash(string songHash, string? songName, string? songKey, string? mapper);

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> of type <typeparamref name="T"/> from the given values.
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="songName"></param>
        /// <param name="songKey"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        protected abstract T CreateFromByLevelId(string levelId, string? songName, string? songKey, string? mapper);

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(ISong song)
        {
            if (AllowDuplicates || !Songs.Any(s => s.Hash == song.Hash || (song.Key != null && s.Key == song.Key)))
            {
                T playlistSong = CreateFrom(song);
                Songs.Add(playlistSong);
                
                if (Count <= 4)
                {
                    RaiseCoverImageChangedForDefaultCover();
                }
                
                return playlistSong;
            }
            return null;
        }

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(string songHash, string? songName, string? songKey, string? mapper) =>
            Add(CreateFromByHash(songHash, songName, songKey, mapper));

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(IPlaylistSong item) => Add((ISong)item);

        /// <inheritdoc/>
        public virtual void Clear()
        {
            Songs.Clear();
            RaiseCoverImageChangedForDefaultCover();
        }

        /// <inheritdoc/>
        public virtual void Sort()
        {
            Songs = Songs.OrderByDescending(s => s.DateAdded).ToList();
            RaiseCoverImageChangedForDefaultCover();
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
            if (index < 4)
            {
                RaiseCoverImageChangedForDefaultCover();
            }
        }

        /// <inheritdoc/>
        public bool Remove(IPlaylistSong item)
        {
            int index = -1;
            if (item is T matchedType)
            {
                index = Songs.IndexOf(matchedType);
            }
            else
            {
                index = Songs.FindIndex(s => s.Equals(item));
            }

            if (index != -1)
            {
                Songs.RemoveAt(index);
                if (index < 4)
                {
                    RaiseCoverImageChangedForDefaultCover();
                }
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            Songs.RemoveAt(index);
            if (index < 3)
            {
                RaiseCoverImageChangedForDefaultCover();
            }
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
            RaiseCoverImageChangedForDefaultCover();
            return songsRemoved;
        }

        /// <inheritdoc/>
        public virtual int RemoveAll(Func<T, bool> match)
        {
            int removedSongs = 0;
            if (match != null)
                removedSongs = Songs.RemoveAll(s => match(s));
            RaiseCoverImageChangedForDefaultCover();
            return removedSongs;
        }

        /// <inheritdoc/>
        public void RemoveDuplicates()
        {
            int previousCount = Songs.Count;
            Songs = Songs.Distinct(IPlaylistSongComparer<T>.Default).ToList();
            if (Songs.Count != previousCount)
            {
                RaisePlaylistChanged();
                RaiseCoverImageChangedForDefaultCover();
            }
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
