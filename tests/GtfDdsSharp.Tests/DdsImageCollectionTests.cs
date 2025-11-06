namespace GtfDdsSharp.Tests;

public class DdsImageCollectionTests
{
    [Fact]
    public void Constructor_PathsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DdsImageCollection((IEnumerable<string>)null!));
    }

    [Fact]
    public void Constructor_PathsEmpty_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DdsImageCollection((IEnumerable<string>)[]));
    }

    [Fact]
    public void Constructor_TooManyPaths_ThrowsArgumentOutOfRangeException()
    {
        using TempFile tempDdsFile = new();
        File.WriteAllBytes(tempDdsFile, DdsImageTests.DdsBytes);
        IEnumerable<string> paths = Enumerable.Repeat((string)tempDdsFile, byte.MaxValue + 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => new DdsImageCollection(paths));
    }

    [Fact]
    public void Constructor_ImagesNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DdsImageCollection(null!));
    }

    [Fact]
    public void Constructor_ImagesEmpty_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DdsImageCollection([]));
    }

    [Fact]
    public void Constructor_TooManyImages_ThrowsArgumentOutOfRangeException()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        IEnumerable<DdsImage> images = Enumerable.Repeat(image, byte.MaxValue + 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => new DdsImageCollection(images));
    }

    [Fact]
    public void Constructor_ImageDisposed_ThrowsObjectDisposedException()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        image.Dispose();
        Assert.Throws<ObjectDisposedException>(() => new DdsImageCollection(image));
    }

    [Fact]
    public void ConvertToGtf_Disposed_ThrowsObjectDisposedException()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        images.Dispose();
        Assert.Throws<ObjectDisposedException>(images.ConvertToGtf);
    }

    [Fact]
    public void ConvertToGtf_MatchesDdsBytes()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        Assert.Equal(GtfImageTests.GtfBytes, images.ConvertToGtf());
    }

    [Fact]
    public void ConvertToGtf_Path_MatchesDdsBytes()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        using TempFile tempGtfFile = new();
        images.ConvertToGtf(tempGtfFile);
        Assert.Equal(GtfImageTests.GtfBytes, File.ReadAllBytes(tempGtfFile));
    }

    [Fact]
    public void ConvertToGtf_Stream_MatchesDdsBytes()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        using MemoryStream stream = new((int)images.GtfFileSize);
        images.ConvertToGtf(stream);
        Assert.Equal(GtfImageTests.GtfBytes, stream.GetBuffer());
    }

    [Fact]
    public void ConvertToGtf_Buffer_MatchesDdsBytes()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        byte[] buffer = new byte[images.GtfFileSize];
        images.ConvertToGtf(buffer);
        Assert.Equal(GtfImageTests.GtfBytes, buffer);
    }

    [Fact]
    public void ConvertToGtf_BufferSegment_MatchesDdsBytes()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        byte[] buffer = new byte[images.GtfFileSize + 8];
        images.ConvertToGtf(buffer, 4, buffer.Length - 8);
        Assert.Equal(GtfImageTests.GtfBytes, buffer.AsSpan()[4..^4]);
    }

    [Fact]
    public void ConvertToGtf_Span_MatchesDdsBytes()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        Span<byte> buffer = stackalloc byte[(int)(images.GtfFileSize + 8)];
        images.ConvertToGtf(buffer[4..^4]);
        Assert.Equal(GtfImageTests.GtfBytes, buffer[4..^4]);
    }

    [Fact]
    public void ConvertToGtf_Pointer_MatchesDdsBytes()
    {
        using DdsImage image = new(DdsImageTests.DdsBytes);
        using DdsImageCollection images = new(image);
        using NativeMemoryHandle handle = new((int)images.GtfFileSize);
        images.ConvertToGtf(handle.Pointer, (uint)handle.Length);
        Assert.Equal(GtfImageTests.GtfBytes, handle.Span);
    }
}
