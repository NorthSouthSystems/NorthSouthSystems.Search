namespace FOSStrich.Search;

public class CensusTests
{
    [Fact]
    public void ComputePopulation()
    {
        0u.Population().Should().Be(0); // Simple0
        1u.Population().Should().Be(1); // Simple1
        0xFFFFFFFFu.Population().Should().Be(32); // SimpleAll32
        0xFF000000u.Population().Should().Be(8); // BytIsolated1stByteFull
        0x00FF0000u.Population().Should().Be(8); // ByteIsolated2ndByteFull
        0x0000FF00u.Population().Should().Be(8); // ByteIsolated3rdByteFull
        0x000000FFu.Population().Should().Be(8); // ByteIsolated4thByteFull
        0x01000000u.Population().Should().Be(1); // ByteIsolated1stByte1Tail
        0x00010000u.Population().Should().Be(1); // ByteIsolated2ndByte1Tail
        0x00000100u.Population().Should().Be(1); // ByteIsolated3rdByte1Tail
        0x00000001u.Population().Should().Be(1); // ByteIsolated4thByte1Tail
        0x80000000u.Population().Should().Be(1); // ByteIsolated1stByte1Head
        0x00800000u.Population().Should().Be(1); // ByteIsolated2ndByte1Head
        0x00008000u.Population().Should().Be(1); // ByteIsolated3rdByte1Head
        0x00000080u.Population().Should().Be(1); // ByteIsolated4thByte1Head
        0x55000000u.Population().Should().Be(4); // ByteIsolated1stByte1MixTail
        0x00550000u.Population().Should().Be(4); // ByteIsolated2ndByte1MixTail
        0x00005500u.Population().Should().Be(4); // ByteIsolated3rdByte1MixTail
        0x00000055u.Population().Should().Be(4); // ByteIsolated4thByte1MixTail
        0xAA000000u.Population().Should().Be(4); // ByteIsolated1stByte1MixHead
        0x00AA0000u.Population().Should().Be(4); // ByteIsolated2ndByte1MixHead
        0x0000AA00u.Population().Should().Be(4); // ByteIsolated3rdByte1MixHead
        0x000000AAu.Population().Should().Be(4); // ByteIsolated4thByte1MixHead
        0x55555555u.Population().Should().Be(16); // PatternedMixTail
        0xAAAAAAAAu.Population().Should().Be(16); // PatternedMixHead
        0xDEADBEEFu.Population().Should().Be(24); // PatternedDEADBEEF
        0x01234567u.Population().Should().Be(12); // PatternedAsc
        0xFEDCBA98u.Population().Should().Be(20); // PatternedDesc
    }
}