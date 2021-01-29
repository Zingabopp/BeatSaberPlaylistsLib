#if BeatSaber
extern alias BeatSaber;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLib.Types
{

    /// <summary>
    /// A beatmap difficulty
    /// </summary>
    public partial struct Difficulty
    {
        /// <summary>
        /// Beat Saber's <see cref="BeatSaber.BeatmapDifficulty"/> value.
        /// Returns the int '99' if an invalid difficulty is set.
        /// </summary>
        [JsonIgnore]
        public BeatSaber.BeatmapDifficulty BeatmapDifficulty
        {
            get => (BeatSaber.BeatmapDifficulty)DifficultyValue;
            set
            {
                Name = value.ToString();
            }
        }
    }
}
#endif