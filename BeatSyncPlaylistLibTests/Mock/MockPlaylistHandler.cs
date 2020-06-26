using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BeatSaberPlaylistsLib.Types;
namespace BeatSaberPlaylistsLibTests.Mock
{
    public class MockPlaylistHandler : IPlaylistHandler<MockPlaylist>
    {
        public string DefaultExtension { get; set; } = "mock";
        public Type HandledType { get; set; } = typeof(MockPlaylist);

        public HashSet<string> SupportedExtensions = new HashSet<string>() { "mock" };
        public string[] GetSupportedExtensions()
        {
            EventCalled?.Invoke(this, MockHandlerMethod.SerializeIPlaylist);
            return SupportedExtensions.ToArray();
        }
        
        public bool SupportsExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;
            return SupportedExtensions.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public IPlaylist Deserialize(Stream stream)
        {
            EventCalled?.Invoke(this, MockHandlerMethod.Deserialize);
            return new MockPlaylist();
        }

        public void Populate(Stream stream, MockPlaylist target)
        {
            EventCalled?.Invoke(this, MockHandlerMethod.PopulateMockPlaylist);
        }

        public void Populate(Stream stream, IPlaylist target)
        {
            EventCalled?.Invoke(this, MockHandlerMethod.PopulateIPlaylist);
        }

        public void Serialize(MockPlaylist playlist, Stream stream)
        {
            EventCalled?.Invoke(this, MockHandlerMethod.SerializeMockPlaylist);
        }

        public void Serialize(IPlaylist playlist, Stream stream)
        {
            EventCalled?.Invoke(this, MockHandlerMethod.SerializeIPlaylist);
        }

        public MockPlaylist Deserialize<TPlaylist>(Stream stream) where TPlaylist : MockPlaylist
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="MockPlaylist"/> using the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="description"></param>
        /// <param name="suggestedExtension"></param>
        /// <returns></returns>
        public MockPlaylist CreatePlaylist(string fileName, string title, string? author, string? description, string? suggestedExtension)
        {
            MockPlaylist playlist = new MockPlaylist()
            {
                Filename = fileName,
                Title = title,
                Author = author,
                Description = description,
                SuggestedExtension = suggestedExtension
            };
            return playlist;
        }

        ///<inheritdoc/>
        IPlaylist IPlaylistHandler.CreatePlaylist(string fileName, string title, string? author, string? description, string? suggestedExtension)
        => CreatePlaylist(fileName, title, author, description, suggestedExtension);

        public event EventHandler<MockHandlerMethod>? EventCalled;
    }
    public enum MockHandlerMethod
    {
        Deserialize,
        GetSupportedExtensions,
        PopulateMockPlaylist,
        PopulateIPlaylist,
        SerializeMockPlaylist,
        SerializeIPlaylist
    }
}
