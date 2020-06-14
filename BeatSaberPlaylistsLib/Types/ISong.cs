#if BeatSaber
extern alias BeatSaber;
#endif
using System;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Interface for the basic data of a song.
    /// </summary>
    public interface ISong
#if BeatSaber
        : BeatSaber.IPreviewBeatmapLevel
#endif
    {
#if BeatSaber
        /// <summary>
        /// The <see cref="BeatSaber.IPreviewBeatmapLevel"/> this playlist song is matched to, if any.
        /// Depends on SongCore being finished loading songs.
        /// </summary>
        public BeatSaber.IPreviewBeatmapLevel? PreviewBeatmapLevel { get; }
#endif
        /// <summary>
        /// Beat Saver hash of the song, always uppercase.
        /// Setting Hash will also set <see cref="LevelId"/> to Hash prefixed by "custom_level_".
        /// </summary>
        string? Hash { get; set; }
        /// <summary>
        /// LevelId of the song given by Beat Saber. If the song is a custom level, the hash is uppercase and prefixed by "custom_level_".
        /// Setting a LevelId prefixed by "custom_level_" will also set <see cref="Hash"/>.
        /// </summary>
        string? LevelId { get; set; }
        /// <summary>
        /// Name of the song.
        /// </summary>
        string? Name { get; set; }
        /// <summary>
        /// Beat Saver key of the song, always uppercase. 
        /// </summary>
        string? Key { get; set; }
        /// <summary>
        /// Mapper of the song.
        /// </summary>
        string? LevelAuthorName { get; set; }
        /// <summary>
        /// Flags enum identifying which properties can be used to uniquely identify the song.
        /// </summary>
        Identifier Identifiers { get; }
    }

    /// <summary>
    /// Flags enum identifying which properties can be used to uniquely identify the song.
    /// </summary>
    [Flags]
    public enum Identifier
    {
        /// <summary>
        /// Song has no identifiers.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates the Hash is available.
        /// </summary>
        Hash = 1 << 0,
        /// <summary>
        /// Indicates the LevelId is available.
        /// </summary>
        LevelId = 1 << 1,
        /// <summary>
        /// Indicates the Beat Saver key is available.
        /// </summary>
        Key = 1 << 2
    }
}
