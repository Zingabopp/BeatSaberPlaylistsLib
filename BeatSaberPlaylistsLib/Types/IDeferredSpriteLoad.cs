#if BeatSaber
extern alias BeatSaber;
using BeatSaber::UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLib.Types
{
    public interface IDeferredSpriteLoad
    {
        event EventHandler? SpriteLoaded;
        bool SpriteWasLoaded { get; }
        Sprite? Sprite { get; }
    }
}
#endif