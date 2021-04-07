using System;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Notifies when a cover image has changed.
    /// </summary>
    public interface INotifyCoverChanged
    {

        /// <summary>
        /// Raised when the cover image is changed.
        /// </summary>
        event EventHandler? CoverImageChanged;
    }
}
