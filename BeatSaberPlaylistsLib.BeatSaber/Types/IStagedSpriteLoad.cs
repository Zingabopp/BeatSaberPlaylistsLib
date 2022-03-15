extern alias BeatSaber;
using BeatSaber::UnityEngine;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// An interface that supports deferred sprite loading in 2 stages (low res and high res)
    /// </summary>
    public interface IStagedSpriteLoad : IDeferredSpriteLoad
    {
        /// <summary>
        /// Returns true if the Small Sprite is loaded.
        /// </summary>
        bool SmallSpriteWasLoaded { get; }
        /// <summary>
        /// Downscaled sprite
        /// </summary>
        Sprite? SmallSprite { get; }
    }
}