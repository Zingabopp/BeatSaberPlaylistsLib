using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    public abstract partial class Playlist : IPlaylist, INotifyCoverChanged
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
                return true;
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
            if (CustomDataInternal == null)
                CustomDataInternal = new Dictionary<string, object>();
            foreach (var item in extensionData)
            {
                string key = item.Key;
                if (!CustomDataInternal.ContainsKey(key))
                    CustomDataInternal[key] = item.Value;
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
        protected Dictionary<string, object>? CustomDataInternal { get; set; }

        /// <inheritdoc/>
        public bool TryGetCustomData(string key, out object? value)
        {
            value = null!;
            return CustomDataInternal?.TryGetValue(key, out value) ?? false;
        }

        /// <inheritdoc/>
        public void SetCustomData(string key, object value)
        {
            if (CustomDataInternal == null)
                CustomDataInternal = new Dictionary<string, object>();
            if (key.Equals("AllowDuplicates", StringComparison.OrdinalIgnoreCase))
            {
                if (!(value is bool))
                    throw new ArgumentException("'AllowDuplicates' must have a boolean value.", nameof(value));
            }

            CustomDataInternal[key] = value;
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object>? CustomData => CustomDataInternal != null ?
            new ReadOnlyDictionary<string, object>(CustomDataInternal) : null;

        /// <inheritdoc/>
        public abstract int Count { get; }
        /// <inheritdoc/>
        public abstract bool IsReadOnly { get; }

        /// <inheritdoc/>
        public abstract IPlaylistSong this[int index] { get; set; }


        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> from the given <paramref name="song"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        protected abstract IPlaylistSong CreateFrom(ISong song);

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> from the given values.
        /// </summary>
        /// <param name="songHash"></param>
        /// <param name="songName"></param>
        /// <param name="songKey"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        protected abstract IPlaylistSong CreateFromByHash(string songHash, string? songName, string? songKey, string? mapper);

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> from the given values.
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="songName"></param>
        /// <param name="songKey"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        protected abstract IPlaylistSong CreateFromByLevelId(string levelId, string? songName, string? songKey, string? mapper);

        #region Default Cover
        /// <summary>
        /// Cached data for the default cover.
        /// </summary>
        protected byte[]? _defaultCoverData;
#if !BeatSaber
        /// <inheritdoc/>
        public virtual Task<Stream?> GetDefaultCoverStream() => 
            _defaultCoverData != null ? Task.FromResult<Stream?>(new MemoryStream(_defaultCoverData)) : Task.FromResult<Stream?>(null);

#endif

        /// <inheritdoc/>
        public async Task SetDefaultCover(Stream coverStream)
        {
            using MemoryStream ms = new MemoryStream();
            await coverStream.CopyToAsync(ms);
            _defaultCoverData = ms.ToArray();
        }

        /// <inheritdoc/>
        public void RaiseCoverImageChangedForDefaultCover()
        {
#if BeatSaber
            if (!Utilities.ImageSharpLoaded())
            {
                return;
            }
#endif
            
            _defaultCoverData = null;

            if (!HasCover)
            {
                RaiseCoverImageChanged();
#if BeatSaber
                _ = Sprite;
#endif
            }
        }

        /// <inheritdoc/>
        public abstract IPlaylistSong? Add(ISong song);
        /// <inheritdoc/>
        public abstract IPlaylistSong? Add(string songHash, string? songName, string? songKey, string? mapper);
        /// <inheritdoc/>
        public abstract bool TryRemoveByHash(string songHash);
        /// <inheritdoc/>
        public abstract bool TryRemoveByKey(string songKey);
        /// <inheritdoc/>
        public abstract bool TryRemove(IPlaylistSong song);
        /// <inheritdoc/>
        public abstract void RemoveDuplicates();
        /// <inheritdoc/>
        public abstract void Sort();
        /// <inheritdoc/>
        public abstract int RemoveAll(Func<IPlaylistSong, bool> match);
        /// <inheritdoc/>
        public abstract int IndexOf(IPlaylistSong item);
        /// <inheritdoc/>
        public abstract void Insert(int index, IPlaylistSong item);
        /// <inheritdoc/>
        public abstract void RemoveAt(int index);
        /// <inheritdoc/>
        public abstract void Add(IPlaylistSong item);
        /// <inheritdoc/>
        public abstract void Clear();
        /// <inheritdoc/>
        public abstract bool Contains(IPlaylistSong item);
        /// <inheritdoc/>
        public abstract void CopyTo(IPlaylistSong[] array, int arrayIndex);
        /// <inheritdoc/>
        public abstract bool Remove(IPlaylistSong item);
        /// <inheritdoc/>
        public abstract IEnumerator<IPlaylistSong> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
