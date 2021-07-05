extern alias BeatSaber;
using BeatSaber::UnityEngine;
using System;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// An interface used to support deferred sprite loading.
    /// </summary>
    public interface IDeferredSpriteLoad
    {
        /// <summary>
        /// Raised when the Sprite has been loaded.
        /// </summary>
        event EventHandler? SpriteLoaded;
        /// <summary>
        /// Returns true if the Sprite is loaded.
        /// </summary>
        bool SpriteWasLoaded { get; }
        /// <summary>
        /// The Sprite.
        /// </summary>
        Sprite? Sprite { get; }
    }
}