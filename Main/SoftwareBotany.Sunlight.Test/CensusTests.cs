using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareBotany.Sunlight
{
    [TestClass]
    public class CensusTests
    {
        [TestMethod]
        public void ComputePopulation()
        {
            Assert.AreEqual(0, Census.ComputePopulation(0u)); // Simple0
            Assert.AreEqual(1, Census.ComputePopulation(1u)); // Simple1
            Assert.AreEqual(32, Census.ComputePopulation(0xFFFFFFFFu)); // SimpleAll32
            Assert.AreEqual(8, Census.ComputePopulation(0xFF000000u)); // BytIsolated1stByteFull
            Assert.AreEqual(8, Census.ComputePopulation(0x00FF0000u)); // ByteIsolated2ndByteFull
            Assert.AreEqual(8, Census.ComputePopulation(0x0000FF00u)); // ByteIsolated3rdByteFull
            Assert.AreEqual(8, Census.ComputePopulation(0x000000FFu)); // ByteIsolated4thByteFull
            Assert.AreEqual(1, Census.ComputePopulation(0x01000000u)); // ByteIsolated1stByte1Tail
            Assert.AreEqual(1, Census.ComputePopulation(0x00010000u)); // ByteIsolated2ndByte1Tail
            Assert.AreEqual(1, Census.ComputePopulation(0x00000100u)); // ByteIsolated3rdByte1Tail
            Assert.AreEqual(1, Census.ComputePopulation(0x00000001u)); // ByteIsolated4thByte1Tail
            Assert.AreEqual(1, Census.ComputePopulation(0x80000000u)); // ByteIsolated1stByte1Head
            Assert.AreEqual(1, Census.ComputePopulation(0x00800000u)); // ByteIsolated2ndByte1Head
            Assert.AreEqual(1, Census.ComputePopulation(0x00008000u)); // ByteIsolated3rdByte1Head
            Assert.AreEqual(1, Census.ComputePopulation(0x00000080u)); // ByteIsolated4thByte1Head
            Assert.AreEqual(4, Census.ComputePopulation(0x55000000u)); // ByteIsolated1stByte1MixTail
            Assert.AreEqual(4, Census.ComputePopulation(0x00550000u)); // ByteIsolated2ndByte1MixTail
            Assert.AreEqual(4, Census.ComputePopulation(0x00005500u)); // ByteIsolated3rdByte1MixTail
            Assert.AreEqual(4, Census.ComputePopulation(0x00000055u)); // ByteIsolated4thByte1MixTail
            Assert.AreEqual(4, Census.ComputePopulation(0xAA000000u)); // ByteIsolated1stByte1MixHead
            Assert.AreEqual(4, Census.ComputePopulation(0x00AA0000u)); // ByteIsolated2ndByte1MixHead
            Assert.AreEqual(4, Census.ComputePopulation(0x0000AA00u)); // ByteIsolated3rdByte1MixHead
            Assert.AreEqual(4, Census.ComputePopulation(0x000000AAu)); // ByteIsolated4thByte1MixHead
            Assert.AreEqual(16, Census.ComputePopulation(0x55555555u)); // PatternedMixTail
            Assert.AreEqual(16, Census.ComputePopulation(0xAAAAAAAAu)); // PatternedMixHead
            Assert.AreEqual(24, Census.ComputePopulation(0xDEADBEEFu)); // PatternedDEADBEEF
            Assert.AreEqual(12, Census.ComputePopulation(0x01234567u)); // PatternedAsc
            Assert.AreEqual(20, Census.ComputePopulation(0xFEDCBA98u)); // PatternedDesc
        }
    }
}