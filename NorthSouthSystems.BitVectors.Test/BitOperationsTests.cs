namespace NorthSouthSystems.BitVectors;

public class BitOperationsTests
{
    [Fact]
    public void PopCountUint()
    {
        int size = sizeof(uint) * 8;

        for (int shift = 0; shift < size; shift++)
        {
            BitOperations.PopCount(1u << shift).Should().Be(1);
            BitOperations.PopCount(uint.MaxValue >> shift).Should().Be(size - shift);
        }
    }

    [Fact]
    public void PopCountUlong()
    {
        int size = sizeof(ulong) * 8;

        for (int shift = 0; shift < size; shift++)
        {
            BitOperations.PopCount(1ul << shift).Should().Be(1);
            BitOperations.PopCount(ulong.MaxValue >> shift).Should().Be(size - shift);
        }
    }

    [Fact]
    public void PopCountTargeted()
    {
        BitOperations.PopCount(0u).Should().Be(0); // Simple0
        BitOperations.PopCount(1u).Should().Be(1); // Simple1
        BitOperations.PopCount(0xFFFF_FFFFu).Should().Be(32); // SimpleAll32
        BitOperations.PopCount(0xFF00_0000u).Should().Be(8); // ByteIsolated1stByteFull
        BitOperations.PopCount(0x00FF_0000u).Should().Be(8); // ByteIsolated2ndByteFull
        BitOperations.PopCount(0x0000_FF00u).Should().Be(8); // ByteIsolated3rdByteFull
        BitOperations.PopCount(0x0000_00FFu).Should().Be(8); // ByteIsolated4thByteFull
        BitOperations.PopCount(0x0100_0000u).Should().Be(1); // ByteIsolated1stByte1Tail
        BitOperations.PopCount(0x0001_0000u).Should().Be(1); // ByteIsolated2ndByte1Tail
        BitOperations.PopCount(0x0000_0100u).Should().Be(1); // ByteIsolated3rdByte1Tail
        BitOperations.PopCount(0x0000_0001u).Should().Be(1); // ByteIsolated4thByte1Tail
        BitOperations.PopCount(0x8000_0000u).Should().Be(1); // ByteIsolated1stByte1Head
        BitOperations.PopCount(0x0080_0000u).Should().Be(1); // ByteIsolated2ndByte1Head
        BitOperations.PopCount(0x0000_8000u).Should().Be(1); // ByteIsolated3rdByte1Head
        BitOperations.PopCount(0x0000_0080u).Should().Be(1); // ByteIsolated4thByte1Head
        BitOperations.PopCount(0x5500_0000u).Should().Be(4); // ByteIsolated1stByte1MixTail
        BitOperations.PopCount(0x0055_0000u).Should().Be(4); // ByteIsolated2ndByte1MixTail
        BitOperations.PopCount(0x0000_5500u).Should().Be(4); // ByteIsolated3rdByte1MixTail
        BitOperations.PopCount(0x0000_0055u).Should().Be(4); // ByteIsolated4thByte1MixTail
        BitOperations.PopCount(0xAA00_0000u).Should().Be(4); // ByteIsolated1stByte1MixHead
        BitOperations.PopCount(0x00AA_0000u).Should().Be(4); // ByteIsolated2ndByte1MixHead
        BitOperations.PopCount(0x0000_AA00u).Should().Be(4); // ByteIsolated3rdByte1MixHead
        BitOperations.PopCount(0x0000_00AAu).Should().Be(4); // ByteIsolated4thByte1MixHead
        BitOperations.PopCount(0x5555_5555u).Should().Be(16); // PatternedMixTail
        BitOperations.PopCount(0xAAAA_AAAAu).Should().Be(16); // PatternedMixHead
        BitOperations.PopCount(0xDEAD_BEEFu).Should().Be(24); // PatternedDEADBEEF
        BitOperations.PopCount(0x0123_4567u).Should().Be(12); // PatternedAsc
        BitOperations.PopCount(0xFEDC_BA98u).Should().Be(20); // PatternedDesc

        BitOperations.PopCount(0ul).Should().Be(0); // Simple0
        BitOperations.PopCount(1ul).Should().Be(1); // Simple1
        BitOperations.PopCount(0xFFFF_FFFF_FFFF_FFFFul).Should().Be(64); // SimpleAll64
        BitOperations.PopCount(0xFF00_0000_0000_0000ul).Should().Be(8); // ByteIsolated1stByteFull
        BitOperations.PopCount(0x0000_00FF_0000_0000ul).Should().Be(8); // ByteIsolated4thByteFull
        BitOperations.PopCount(0x0000_0000_FF00_0000ul).Should().Be(8); // ByteIsolated5thByteFull
        BitOperations.PopCount(0x0000_0000_0000_00FFul).Should().Be(8); // ByteIsolated8thByteFull
    }
}