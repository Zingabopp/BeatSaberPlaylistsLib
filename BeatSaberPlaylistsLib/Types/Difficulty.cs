using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLib.Types
{

    /// <summary>
    /// A beatmap difficulty
    /// </summary>
    public struct Difficulty
    {
        /// <summary>
        /// The characteristic name
        /// </summary>
        [JsonProperty("characteristic")]
        public string Characteristic { get; set; }

        /// <summary>
        /// The difficulty name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        ///<inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Difficulty diff)
            {
                if (Characteristic?.Equals(diff.Characteristic, StringComparison.OrdinalIgnoreCase) ?? diff.Characteristic != null)
                    return false;
                if (Name?.Equals(diff.Name, StringComparison.OrdinalIgnoreCase) ?? diff.Name != null)
                    return false;
                return true;
            }
            return false;
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            int hash = 238947239;
            hash ^= Characteristic?.GetHashCode() ?? 23408234;
            hash ^= Name?.GetHashCode() ?? 12987213;
            return hash;
        }

        ///<inheritdoc/>
        public static bool operator ==(Difficulty left, Difficulty right)
        {
            return left.Equals(right);
        }

        ///<inheritdoc/>
        public static bool operator !=(Difficulty left, Difficulty right)
        {
            return !(left.Equals(right));
        }
    }
}
