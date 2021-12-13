using BeatSaberPlaylistsLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BeatSaberPlaylistsLibTests.Utilities_Tests
{
    [TestClass]
    public class GetBase64DataStartIndex_Tests
    {
        [TestMethod]
        public void ToBase64EscapeQuotes()
        {
            string path = Path.Combine("ReadOnlyData", "DrawingTests", "bigCover.jpg");
            byte[] bytes = File.ReadAllBytes(path);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string base64 = Utilities.ByteArrayToBase64(bytes);
            base64 = base64.Replace("\"", "\\\"");
            sw.Stop();
            Console.WriteLine($"Time: {sw.Elapsed} | Length: {base64.Length}");
        }

        [TestMethod]
        public void NoPrefix()
        {
            string base64Str = "gk/wX+ne+PFCI8W67X8v";
            int expectedStartIndex = 0;

            int actualStartIndex = Utilities.GetBase64DataStartIndex(base64Str);
            string dataString = base64Str.Substring(actualStartIndex);
            Assert.AreEqual(expectedStartIndex, actualStartIndex);
            if (dataString.Length % 4 == 0)
                _ = Convert.FromBase64String(dataString);
            else
                _ = Convert.FromBase64String(dataString);
        }

        [TestMethod]
        public void Base64Prefix()
        {
            string base64Str = "base64,gk/wX+ne+PFCI8W67X8v";
            int expectedStartIndex = 7;

            int actualStartIndex = Utilities.GetBase64DataStartIndex(base64Str);

            Assert.AreEqual(expectedStartIndex, actualStartIndex);
            _ = Convert.FromBase64String(base64Str.Substring(actualStartIndex));
        }

        [TestMethod]
        public void DataTypePrefix()
        {
            string base64Str = "data:image/png;base64,gk/wX+ne+PFCI8W67X8v";
            int expectedStartIndex = 22;

            int actualStartIndex = Utilities.GetBase64DataStartIndex(base64Str);

            Assert.AreEqual(expectedStartIndex, actualStartIndex);
            _ = Convert.FromBase64String(base64Str.Substring(actualStartIndex));
        }
    }
}
