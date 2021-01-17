#if BeatSaber
extern alias BeatSaber;
using System;
using System.Collections.Generic;
using System.IO;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Interface for a playlist.
    /// </summary>
    public partial interface IPlaylist : BeatSaber.IPlaylist, BeatSaber.IAnnotatedBeatmapLevelCollection
    {
        /// <summary>
        /// Adds the <see cref="BeatSaber.IPreviewBeatmapLevel"/> to the playlist. 
        /// Does nothing if <see cref="AllowDuplicates"/> is false and the song is already in the playlist. 
        /// </summary>
        /// <param name="song"></param>
        /// <returns>The added <see cref="IPlaylistSong"/> (not the <see cref="BeatSaber.IPreviewBeatmapLevel"/>), null if nothing was added.</returns>
        IPlaylistSong? Add(BeatSaber.IPreviewBeatmapLevel song);
    }
}

#endif