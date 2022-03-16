#if BeatSaber
extern alias BeatSaber;
using BeatSaber::UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BeatSaberPlaylistsLib.Types
{

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
            IPlaylistSong? song = AddSong(CreateFromByLevelId(beatmap.levelID, beatmap.songName, null, beatmap.levelAuthorName));
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
            IPlaylistSong? song = Add(beatmap.level);
            if (song != null)
                song.Difficulties = new List<Difficulty> { difficulty };

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

            if (!Utilities.ImageSharpLoaded() || BeatmapLevels.Length == 0)
            {
                return null;
            }

            var ms = new MemoryStream();

            if (BeatmapLevels.Length == 1)
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