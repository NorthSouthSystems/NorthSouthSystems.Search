namespace NorthSouthSystems.BitVectors;

using System.Numerics;

public class BitOperationsTests
{
    // This only tests "our" code when targeting netstandard2.0; however, there
    // is no harm in testing the System.Numerics.BitOperations.PopCount method too.
    [Fact]
    public void PopCount()
    {
        BitOperations.PopCount(0u).Should().Be(0); // Simple0
        BitOperations.PopCount(1u).Should().Be(1); // Simple1
        BitOperations.PopCount(0xFFFFFFFFu).Should().Be(32); // SimpleAll32
        BitOperations.PopCount(0xFF000000u).Should().Be(8); // BytIsolated1stByteFull
        BitOperations.PopCount(0x00FF0000u).Should().Be(8); // ByteIsolated2ndByteFull
        BitOperations.PopCount(0x0000FF00u).Should().Be(8); // ByteIsolated3rdByteFull
        BitOperations.PopCount(0x000000FFu).Should().Be(8); // ByteIsolated4thByteFull
        BitOperations.PopCount(0x01000000u).Should().Be(1); // ByteIsolated1stByte1Tail
        BitOperations.PopCount(0x00010000u).Should().Be(1); // ByteIsolated2ndByte1Tail
        BitOperations.PopCount(0x00000100u).Should().Be(1); // ByteIsolated3rdByte1Tail
        BitOperations.PopCount(0x00000001u).Should().Be(1); // ByteIsolated4thByte1Tail
        BitOperations.PopCount(0x80000000u).Should().Be(1); // ByteIsolated1stByte1Head
        BitOperations.PopCount(0x00800000u).Should().Be(1); // ByteIsolated2ndByte1Head
        BitOperations.PopCount(0x00008000u).Should().Be(1); // ByteIsolated3rdByte1Head
        BitOperations.PopCount(0x00000080u).Should().Be(1); // ByteIsolated4thByte1Head
        BitOperations.PopCount(0x55000000u).Should().Be(4); // ByteIsolated1stByte1MixTail
        BitOperations.PopCount(0x00550000u).Should().Be(4); // ByteIsolated2ndByte1MixTail
        BitOperations.PopCount(0x00005500u).Should().Be(4); // ByteIsolated3rdByte1MixTail
        BitOperations.PopCount(0x00000055u).Should().Be(4); // ByteIsolated4thByte1MixTail
        BitOperations.PopCount(0xAA000000u).Should().Be(4); // ByteIsolated1stByte1MixHead
        BitOperations.PopCount(0x00AA0000u).Should().Be(4); // ByteIsolated2ndByte1MixHead
        BitOperations.PopCount(0x0000AA00u).Should().Be(4); // ByteIsolated3rdByte1MixHead
        BitOperations.PopCount(0x000000AAu).Should().Be(4); // ByteIsolated4thByte1MixHead
        BitOperations.PopCount(0x55555555u).Should().Be(16); // PatternedMixTail
        BitOperations.PopCount(0xAAAAAAAAu).Should().Be(16); // PatternedMixHead
        BitOperations.PopCount(0xDEADBEEFu).Should().Be(24); // PatternedDEADBEEF
        BitOperations.PopCount(0x01234567u).Should().Be(12); // PatternedAsc
        BitOperations.PopCount(0xFEDCBA98u).Should().Be(20); // PatternedDesc
    }
}