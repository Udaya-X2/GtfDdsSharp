namespace GtfDdsSharp.Tests;

public class PointerSpanEnumeratorTests
{
    [Fact]
    public void ZeroLength_ReturnsEmptyEnumerator()
    {
        foreach (Span<byte> _ in new PointerSpanEnumerator(0, 0))
        {
            Assert.Fail();
        }
    }

    [InlineData(0x40000)]
    [InlineData(0x7FFFFFC6)]
    [InlineData(0x7FFFFFC7)]
    [InlineData(0x7FFFFFC8)]
    [InlineData(int.MaxValue)]
    [InlineData(uint.MaxValue)]
    [Theory]
    public void Span_MatchesRemainingLength(uint length)
    {
        PointerSpanEnumerator enumerator = new(0, length);
        Assert.Equal(enumerator.Current, default);

        foreach (Span<byte> span in enumerator)
        {
            uint spanLength = Math.Min(length, (uint)Array.MaxLength);
            Assert.Equal(spanLength, (uint)span.Length);
            length -= spanLength;
        }

        Assert.Equal(enumerator.Current, default);
    }
}
