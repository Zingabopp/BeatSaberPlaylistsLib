using BeatSaberPlaylistsLib.Legacy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeatSaberPlaylistsLibTests.Mock
{
    public class DerivedLegacyPlaylistHandler : LegacyPlaylistHandler
    {
        public override LegacyPlaylist Deserialize(Stream stream)
        {
            return Deserialize<DerivedLegacyPlaylist>(stream);
        }
    }
}
