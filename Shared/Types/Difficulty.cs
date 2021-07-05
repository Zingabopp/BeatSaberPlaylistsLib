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
        private const int kEasyValue = 0;
        private const int kNormalValue = 1;
        private const int kHardValue = 2;
        private const int kExpertValue = 3;
        private const int kExpertPlusValue = 4;
        private const int kInvalidDifficultyValue = 99;
        /// <summary>
        /// Integer value of the 'Easy' difficulty.
        /// </summary>
        public static readonly int EasyValue = kEasyValue;
        /// <summary>
        /// Integer value of the 'Normal' difficulty.
        /// </summary>
        public static readonly int NormalValue = kNormalValue;
        /// <summary>
        /// Integer value of the 'Hard' difficulty.
        /// </summary>
        public static readonly int HardValue = kHardValue;
        /// <summary>
        /// Integer value of the 'Expert' difficulty.
        /// </summary>
        public static readonly int ExpertValue = kExpertValue;
        /// <summary>
        /// Integer value of the 'ExpertPlus' difficulty.
        /// </summary>
        public static readonly int ExpertPlusValue = kExpertPlusValue;
        /// <summary>
        /// Integer value of an invalid difficulty.
        /// </summary>
        public static readonly int InvalidDifficultyValue = kInvalidDifficultyValue;
        private string? _name;
        /// <summary>
        /// Integer value of the difficulty. Matches Beat Saber's BeatmapDifficulty enum.
        /// </summary>
        [JsonIgnore]
        public int DifficultyValue { get; private set; }
        /// <summary>
        /// The characteristic name
        /// </summary>
        [JsonProperty("characteristic")]
        public string Characteristic { get; set; }

        /// <summary>
        /// The difficulty name
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (_name == value)
                    return;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _name = null;
                    return;
                }
                _name = value;
                DifficultyValue = DifficultyStringToValue(value);
            }
        }
        /// <summary>
        /// Converts a difficulty string to an integer. 
        /// Returns 99 for invalid difficulty strings.
        /// </summary>
        /// <param name="difficultyString"></param>
        /// <returns></returns>
        public static int DifficultyStringToValue(string difficultyString)
        {
            if (difficultyString.Equals("ExpertPlus", StringComparison.OrdinalIgnoreCase))
                return ExpertPlusValue;
            else if (difficultyString.Equals("Expert", StringComparison.OrdinalIgnoreCase))
                return ExpertValue;
            else if (difficultyString.Equals("Hard", StringComparison.OrdinalIgnoreCase))
                return HardValue;
            else if (difficultyString.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                return NormalValue;
            else if (difficultyString.Equals("Easy", StringComparison.OrdinalIgnoreCase))
                return EasyValue;
            else
                return 99;
        }

        /// <summary>
        /// Converts a difficulty string to an integer. 
        /// Returns 99 for invalid difficulty strings.
        /// </summary>
        /// <param name="difficultyValue"></param>
        /// <returns></returns>
        public static string DifficultyValueToString(int difficultyValue)
        {
            return difficultyValue switch
            {
                kEasyValue => "Easy",
                kNormalValue => "Normal",
                kHardValue => "Hard",
                kExpertValue => "Expert",
                kExpertPlusValue => "ExpertPlus",
                _ => string.Empty
            };
        }

        ///<inheritdoc/>
        public override bool Equals(object? obj)
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
