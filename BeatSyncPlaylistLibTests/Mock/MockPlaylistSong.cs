using BeatSaberPlaylistsLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLibTests.Mock
{
    public class MockPlaylistSong : PlaylistSong
    {
        public override bool Equals(IPlaylistSong other)
        {
            if (other == null)
                return false;
            return Hash == other?.Hash;
        }
    }
}
