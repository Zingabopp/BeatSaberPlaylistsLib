#if BeatSaber
extern alias BeatSaber;
using System;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Interface for the basic data of a song.
    /// </summary>
    public partial interface ISong : BeatSaber.IPreviewBeatmapLevel
    {
        /// <summary>
        /// The <see cref="BeatSaber.IPreviewBeatmapLevel"/> this playlist song is matched to, if any.
        /// Depends on SongCore being finished loading songs.
        /// </summary>
        BeatSaber.IPreviewBeatmapLevel? PreviewBeatmapLevel { get; }
        /// <summary>
        /// Sets the game's associated <see cref="BeatSaber.IPreviewBeatmapLevel"/>.
        /// </summary>
        /// <param name="beatmap"></param>
        internal void SetPreviewBeatmap(BeatSaber.IPreviewBeatmapLevel beatmap);

        public void RefreshFromSongCore();

    }
}
#endif