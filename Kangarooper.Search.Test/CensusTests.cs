namespace Kangarooper.Search
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CensusTests
    {
        [TestMethod]
        public void ComputePopulation()
        {
            Assert.AreEqual(0, 0u.Population()); // Simple0
            Assert.AreEqual(1, 1u.Population()); // Simple1
            Assert.AreEqual(32, 0xFFFFFFFFu.Population()); // SimpleAll32
            Assert.AreEqual(8, 0xFF000000u.Population()); // BytIsolated1stByteFull
            Assert.AreEqual(8, 0x00FF0000u.Population()); // ByteIsolated2ndByteFull
            Assert.AreEqual(8, 0x0000FF00u.Population()); // ByteIsolated3rdByteFull
            Assert.AreEqual(8, 0x000000FFu.Population()); // ByteIsolated4thByteFull
            Assert.AreEqual(1, 0x01000000u.Population()); // ByteIsolated1stByte1Tail
            Assert.AreEqual(1, 0x00010000u.Population()); // ByteIsolated2ndByte1Tail
            Assert.AreEqual(1, 0x00000100u.Population()); // ByteIsolated3rdByte1Tail
            Assert.AreEqual(1, 0x00000001u.Population()); // ByteIsolated4thByte1Tail
            Assert.AreEqual(1, 0x80000000u.Population()); // ByteIsolated1stByte1Head
            Assert.AreEqual(1, 0x00800000u.Population()); // ByteIsolated2ndByte1Head
            Assert.AreEqual(1, 0x00008000u.Population()); // ByteIsolated3rdByte1Head
            Assert.AreEqual(1, 0x00000080u.Population()); // ByteIsolated4thByte1Head
            Assert.AreEqual(4, 0x55000000u.Population()); // ByteIsolated1stByte1MixTail
            Assert.AreEqual(4, 0x00550000u.Population()); // ByteIsolated2ndByte1MixTail
            Assert.AreEqual(4, 0x00005500u.Population()); // ByteIsolated3rdByte1MixTail
            Assert.AreEqual(4, 0x00000055u.Population()); // ByteIsolated4thByte1MixTail
            Assert.AreEqual(4, 0xAA000000u.Population()); // ByteIsolated1stByte1MixHead
            Assert.AreEqual(4, 0x00AA0000u.Population()); // ByteIsolated2ndByte1MixHead
            Assert.AreEqual(4, 0x0000AA00u.Population()); // ByteIsolated3rdByte1MixHead
            Assert.AreEqual(4, 0x000000AAu.Population()); // ByteIsolated4thByte1MixHead
            Assert.AreEqual(16, 0x55555555u.Population()); // PatternedMixTail
            Assert.AreEqual(16, 0xAAAAAAAAu.Population()); // PatternedMixHead
            Assert.AreEqual(24, 0xDEADBEEFu.Population()); // PatternedDEADBEEF
            Assert.AreEqual(12, 0x01234567u.Population()); // PatternedAsc
            Assert.AreEqual(20, 0xFEDCBA98u.Population()); // PatternedDesc
        }
    }
}