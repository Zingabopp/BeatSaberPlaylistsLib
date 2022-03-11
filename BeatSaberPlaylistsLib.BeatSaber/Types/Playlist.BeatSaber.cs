#if BeatSaber
extern alias BeatSaber;
using BeatSaber::UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Graphics = System.Drawing.Graphics;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    public abstract partial class Playlist : IStagedSpriteLoad
    {
        /// <summary>
        /// Maximum width and height of the small cover image
        /// </summary>
        public const int kSmallImageSize = 128;

        /// <summary>
        /// Queue of <see cref="Action"/>s to load playlist sprites.
        /// </summary>
        protected static readonly Queue<Action> SpriteQueue = new Queue<Action>();

        /// <summary>
        /// Instance of the playlist cover sprite.
        /// </summary>
        protected Sprite? _sprite;
        /// <summary>
        /// Instance of the previous cover sprite.
        /// </summary>
        protected Sprite? _previousSprite;
        
        /// <summary>
        /// Instance of the downscaled playlist cover sprite.
        /// </summary>
        protected Sprite? _smallSprite;
        /// <summary>
        /// Instance of the previous downscaled cover sprite.
        /// </summary>
        protected Sprite? _previousSmallSprite;
        
        /// <summary>
        /// Returns true if the sprite for the playlist is already queued.
        /// </summary>
        protected bool SpriteLoadQueued;
        private static readonly object _loaderLock = new object();
        private static bool CoroutineRunning = false;

        /// <summary>
        /// Downscales <param name="original"/> to <see cref="kSmallImageSize"/>
        /// </summary>
        protected static Stream DownscaleImage(Stream original)
        {
            var ms = new MemoryStream();
            try
            {
                var originalImage = Image.FromStream(original);

                if (originalImage.Width <= kSmallImageSize && originalImage.Height <= kSmallImageSize)
                {
                    return original;
                }

                var resizedRect = new Rectangle(0, 0, kSmallImageSize, kSmallImageSize);
                var resizedImage = new Bitmap(kSmallImageSize, kSmallImageSize);

                resizedImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    using var wrapMode = new ImageAttributes();
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(originalImage, resizedRect, 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, wrapMode);
                }

                resizedImage.Save(ms, ImageFormat.Png);
                return ms;
            }
            catch (Exception)
            {
                return original;
            }
        }

        /// <summary>
        /// Adds a playlist to the sprite load queue.
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="downscaleImage"></param>
        protected async static void QueueLoadSprite(Playlist playlist, bool downscaleImage)
        {
            var stream = playlist.HasCover ? playlist.GetCoverStream() : await playlist.GetDefaultCoverStream() ?? Utilities.GetDefaultImageStream();
            if (downscaleImage)
            {
                var downscaleStream = await Task.Run(() => DownscaleImage(stream));
                stream.Dispose();
                SpriteQueue.Enqueue(() =>
                {
                    var sprite = Utilities.GetSpriteFromStream(downscaleStream);
                    playlist._smallSprite = sprite;
                    OnSmallSpriteLoaded(playlist);
                    downscaleStream.Dispose();
                });
            }
            else
            {
                SpriteQueue.Enqueue(() =>
                {
                    var sprite = Utilities.GetSpriteFromStream(stream);
                    playlist._sprite = sprite;
                    OnSpriteLoaded(playlist);
                    stream.Dispose();
                });   
            }

            if (!CoroutineRunning)
                BeatSaber.SharedCoroutineStarter.instance.StartCoroutine(SpriteLoadCoroutine());
        }

        private static void OnSpriteLoaded(Playlist playlist)
        {
            playlist.SpriteWasLoaded = true;
            playlist.SpriteLoaded?.Invoke(playlist, null);
            playlist._previousSprite = null;
            playlist.SpriteLoadQueued = false;
        }

        private static void OnSmallSpriteLoaded(Playlist playlist)
        {
            playlist.SmallSpriteWasLoaded = true;
            playlist.SpriteLoaded?.Invoke(playlist, null);
            playlist._previousSmallSprite = null;
            playlist.SpriteLoadQueued = false;
        }

        private static void OnDefaultCoverSpriteLoaded(Playlist playlist)
        {
            playlist.SpriteWasLoaded = true;
            playlist.SmallSpriteWasLoaded = true;
            playlist.SpriteLoaded?.Invoke(playlist, null);
            playlist._previousSprite = null;
            playlist._previousSmallSprite = null;
            playlist.SpriteLoadQueued = false;
        }

        /// <summary>
        /// Wait <see cref="YieldInstruction"/> between sprite loads.
        /// </summary>
        public static YieldInstruction LoadWait = new WaitForEndOfFrame();

        /// <summary>
        /// Coroutine to load sprites in the queue.
        /// </summary>
        /// <returns></returns>
        protected static IEnumerator<YieldInstruction> SpriteLoadCoroutine()
        {
            lock (_loaderLock)
            {
                if (CoroutineRunning)
                    yield break;
                CoroutineRunning = true;
            }
            while (SpriteQueue.Count > 0)
            {
                yield return LoadWait;
                var loader = SpriteQueue.Dequeue();
                loader?.Invoke();
            }
            CoroutineRunning = false;
            if (SpriteQueue.Count > 0) // Just in case
                BeatSaber.SharedCoroutineStarter.instance.StartCoroutine(SpriteLoadCoroutine());
        }

        #region IStagedSpriteLoad

        /// <inheritdoc/>
        public event EventHandler? SpriteLoaded;

        /// <inheritdoc/>
        public bool SpriteWasLoaded { get; protected set; }

        /// <inheritdoc/>
        public Sprite? Sprite
        {
            get
            {
                if (_sprite != null)
                    return _sprite;
                _sprite = _previousSprite ? _previousSprite : Utilities.DefaultSprite;
                if (!SpriteLoadQueued)
                {
                    SpriteLoadQueued = true;
                    QueueLoadSprite(this, false);
                }
                return _sprite;
            }
        }

        /// <inheritdoc/>
        public bool SmallSpriteWasLoaded { get; protected set; }

        /// <inheritdoc/>
        public Sprite? SmallSprite
        {
            get
            {
                if (_smallSprite != null)
                    return _smallSprite;
                _smallSprite = _previousSmallSprite ? _previousSmallSprite : Utilities.DefaultSprite;
                if (!SpriteLoadQueued)
                {
                    SpriteLoadQueued = true;
                    QueueLoadSprite(this, true);
                }
                return _smallSprite;
            }
        }

        /// <summary>
        /// Raises cover image changed if we are using default image. Called when we change the title in a Playlist UI.
        /// </summary>
        public void RaiseCoverImageChangedForDefaultCover()
        {
            if (!HasCover)
            {
                RaiseCoverImageChanged();
                _ = Sprite;
                _ = SmallSprite;
            }
        }

        /// <summary>
        /// Resets the sprite for...reasons.
        /// </summary>
        partial void ResetSprite()
        {
            _previousSprite = _sprite;
            _previousSmallSprite = _smallSprite;
            _sprite = null;
            _smallSprite = null;
        }

        #endregion
    }


    public abstract partial class Playlist<T> : BeatSaber.IBeatmapLevelCollection
    {
        /// <summary>
        /// Name of the collection, uses <see cref="Playlist.Title"/>.
        /// </summary>
        string BeatSaber.IAnnotatedBeatmapLevelCollection.collectionName => Regex.Replace(Title, @"\t|\n|\r", " ");

        /// <summary>
        /// Cover image sprite.
        /// </summary>
        Sprite? BeatSaber.IAnnotatedBeatmapLevelCollection.coverImage => Sprite;

        /// <summary>
        /// Cover image sprite.
        /// </summary>
        Sprite? BeatSaber.IAnnotatedBeatmapLevelCollection.smallCoverImage => SmallSprite;

        /// <summary>
        /// BeatmapLevelPack ID.
        /// </summary>
        public string packID => BeatSaber.CustomLevelLoader.kCustomLevelPackPrefixId + playlistID;

        /// <summary>
        /// BeatmapLevelPack Name, same as name of the collection.
        /// </summary>
        public string packName => Regex.Replace(Title, @"\t|\n|\r", " ");

        /// <summary>
        /// BeatmapLevelPack Short Name, same as name of the collection.
        /// </summary>
        public string shortPackName => Regex.Replace(Title, @"\t|\n|\r", " ");

        /// <summary>
        /// Returns itself.
        /// </summary>
        BeatSaber.IBeatmapLevelCollection BeatSaber.IAnnotatedBeatmapLevelCollection.beatmapLevelCollection => this;
        /// <summary>
        /// Returns a new array of IPreviewBeatmapLevels in this playlist.
        /// </summary>
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        IReadOnlyList<BeatSaber.IPreviewBeatmapLevel> BeatSaber.IBeatmapLevelCollection.beatmapLevels
        {
            get
            {
                Songs.ForEach(delegate (T s)
                {
                    if (s is PlaylistSong playlistSong)
                    {
                        playlistSong.RefreshFromSongCore();
                    }
                });
                return Songs.Where(s => s.PreviewBeatmapLevel != null).Select(s => s.PreviewBeatmapLevel).ToArray();
            }
        }
        /// <summary>
        /// Returns a new array of PlaylistSongs (cast as IPreviewBeatmapLevels) in this playlist.
        /// </summary>
        public BeatSaber.IPreviewBeatmapLevel[] BeatmapLevels
        {
            get
            {
                Songs.ForEach(delegate (T s)
                {
                    if (s is PlaylistSong playlistSong)
                    {
                        playlistSong.RefreshFromSongCore();
                    }
                });
                return Songs.Where(s => s.PreviewBeatmapLevel != null).Cast<BeatSaber.IPreviewBeatmapLevel>().ToArray();
            }
        }

#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        /// <inheritdoc/>
        public IPlaylistSong? Add(BeatSaber.IPreviewBeatmapLevel beatmap)
        {
            if (beatmap == null)
                return null;
            IPlaylistSong? song = Add(CreateFromByLevelId(beatmap.levelID, beatmap.songName, null, beatmap.levelAuthorName));
            song?.SetPreviewBeatmap(beatmap);
            return song;
        }

        /// <inheritdoc/>
        public IPlaylistSong? Add(BeatSaber.IDifficultyBeatmap beatmap)
        {
            if (beatmap == null)
                return null;

            Difficulty difficulty = new Difficulty();
            difficulty.BeatmapDifficulty = beatmap.difficulty;
            difficulty.Characteristic = beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
            IPlaylistSong? song = Add(CreateFromByLevelId(beatmap.level.levelID, beatmap.level.songName, null, beatmap.level.levelAuthorName));
            if (song != null)
                song.Difficulties = new List<Difficulty> { difficulty };

            song?.SetPreviewBeatmap(beatmap.level);
            return song;
        }

        #region Default Cover

        /// <inheritdoc cref="IPlaylist.GetDefaultCoverStream" />
        public override async Task<Stream?> GetDefaultCoverStream()
        {
            if (_defaultCoverData != null)
            {
                return new MemoryStream(_defaultCoverData);
            }
            
            using MemoryStream ms = new MemoryStream();
            
            if (BeatmapLevels.Length == 0)
            {
                using var coverStream = Utilities.GetDefaultImageStream();
                if (coverStream != null) await coverStream.CopyToAsync(ms);
            }
            else if (BeatmapLevels.Length == 1)
            {
                using var coverStream = Utilities.GetStreamFromSprite(await BeatmapLevels[0].GetCoverImageAsync(CancellationToken.None));
                if (coverStream != null) await coverStream.CopyToAsync(ms);
            }
            else if (BeatmapLevels.Length == 2)
            {
                using var imageStream1 = Utilities.GetStreamFromSprite(await BeatmapLevels[0].GetCoverImageAsync(CancellationToken.None));
                using var imageStream2 = Utilities.GetStreamFromSprite(await BeatmapLevels[1].GetCoverImageAsync(CancellationToken.None));
                using var coverStream = await ImageUtilities.GenerateCollage(imageStream1 ?? Stream.Null, imageStream2 ?? Stream.Null);
                await coverStream.CopyToAsync(ms);
            }
            else if (BeatmapLevels.Length == 3)
            {
                using var imageStream1 = Utilities.GetStreamFromSprite(await BeatmapLevels[0].GetCoverImageAsync(CancellationToken.None));
                using var imageStream2 = Utilities.GetStreamFromSprite(await BeatmapLevels[1].GetCoverImageAsync(CancellationToken.None));
                using var imageStream3 = Utilities.GetStreamFromSprite(await BeatmapLevels[2].GetCoverImageAsync(CancellationToken.None));
                using var coverStream = await ImageUtilities.GenerateCollage(imageStream1 ?? Stream.Null, imageStream2 ?? Stream.Null, imageStream3 ?? Stream.Null);
                await coverStream.CopyToAsync(ms);
            }
            else
            {
                using var imageStream1 = Utilities.GetStreamFromSprite(await BeatmapLevels[0].GetCoverImageAsync(CancellationToken.None));
                using var imageStream2 = Utilities.GetStreamFromSprite(await BeatmapLevels[1].GetCoverImageAsync(CancellationToken.None));
                using var imageStream3 = Utilities.GetStreamFromSprite(await BeatmapLevels[2].GetCoverImageAsync(CancellationToken.None));
                using var imageStream4 = Utilities.GetStreamFromSprite(await BeatmapLevels[3].GetCoverImageAsync(CancellationToken.None));
                using var coverStream = await ImageUtilities.GenerateCollage(imageStream1 ?? Stream.Null, imageStream2 ?? Stream.Null, imageStream3 ?? Stream.Null, imageStream4 ?? Stream.Null);
                await coverStream.CopyToAsync(ms);
            }

            _defaultCoverData = ms.ToArray();
            return ms;
        }

        #endregion
    }
}
#endif