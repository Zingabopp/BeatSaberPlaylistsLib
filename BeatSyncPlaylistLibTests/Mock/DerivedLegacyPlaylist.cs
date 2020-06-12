using BeatSaberPlaylistsLib.Legacy;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLibTests.Mock
{
    public class DerivedLegacyPlaylist : LegacyPlaylist
    {
        public string collectionName => this.Title;
    }
}
