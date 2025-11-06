namespace GtfDdsSharp.Tests;

public class GtfTextureTests
{
    [Fact]
    public void Constructor_ImageNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GtfTexture(null!, 0));
    }

    [Fact]
    public void Constructor_ImageDisposed_ThrowsObjectDisposedException()
    {
        GtfImage image = new(GtfImageTests.GtfBytes);
        image.Dispose();
        Assert.Throws<ObjectDisposedException>(() => new GtfTexture(image, 0));
    }

    [Fact]
    public void Constructor_BadIndex_ThrowsArgumentException()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        Assert.Throws<ArgumentException>(() => new GtfTexture(image, 1));
    }

    [Fact]
    public void ConvertToDds_Disposed_ThrowsObjectDisposedException()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        image.Dispose();
        Assert.Throws<ObjectDisposedException>(texture.ConvertToDds);
    }

    [Fact]
    public void ConvertToDds_MatchesDdsBytes()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        Assert.Equal(DdsImageTests.DdsBytes, texture.ConvertToDds());
    }

    [Fact]
    public void ConvertToDds_Path_MatchesDdsBytes()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        using TempFile tempDdsFile = new();
        texture.ConvertToDds(tempDdsFile);
        Assert.Equal(DdsImageTests.DdsBytes, File.ReadAllBytes(tempDdsFile));
    }

    [Fact]
    public void ConvertToDds_Stream_MatchesDdsBytes()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        using MemoryStream stream = new((int)texture.DdsFileSize);
        texture.ConvertToDds(stream);
        Assert.Equal(DdsImageTests.DdsBytes, stream.GetBuffer());
    }

    [Fact]
    public void ConvertToDds_Buffer_MatchesDdsBytes()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        byte[] buffer = new byte[texture.DdsFileSize];
        texture.ConvertToDds(buffer);
        Assert.Equal(DdsImageTests.DdsBytes, buffer);
    }

    [Fact]
    public void ConvertToDds_BufferSegment_MatchesDdsBytes()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        byte[] buffer = new byte[texture.DdsFileSize + 8];
        texture.ConvertToDds(buffer, 4, buffer.Length - 8);
        Assert.Equal(DdsImageTests.DdsBytes, buffer.AsSpan()[4..^4]);
    }

    [Fact]
    public void ConvertToDds_Span_MatchesDdsBytes()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        Span<byte> buffer = stackalloc byte[(int)(texture.DdsFileSize + 8)];
        texture.ConvertToDds(buffer[4..^4]);
        Assert.Equal(DdsImageTests.DdsBytes, buffer[4..^4]);
    }

    [Fact]
    public void ConvertToDds_Pointer_MatchesDdsBytes()
    {
        using GtfImage image = new(GtfImageTests.GtfBytes);
        GtfTexture texture = image[0];
        using NativeMemoryHandle handle = new((int)texture.DdsFileSize);
        texture.ConvertToDds(handle.Pointer, (uint)handle.Length);
        Assert.Equal(DdsImageTests.DdsBytes, handle.Span);
    }
}
