using System.Runtime.CompilerServices;

namespace GtfDdsSharp.Tests;

public class GtfImageTests
{
    /// <summary>
    /// Returns a byte array that represents "data/dxt1-simple-1x1.dds" converted to a GTF file.
    /// </summary>
    internal static byte[] GtfBytes
    {
        get
        {
            byte[] gtfBytes = new byte[2 * GtfTexture.Alignment];
            ref var gtfHeader = ref Unsafe.As<byte, GtfHeader>(ref gtfBytes[0]);
            gtfHeader.Version = GtfImage.DefaultVersion;
            gtfHeader.Size = GtfTexture.Alignment;
            gtfHeader.NumTexture = 1;
            ref var texture = ref Unsafe.As<byte, GtfTextureAttribute>(ref gtfBytes[Unsafe.SizeOf<GtfHeader>()]);
            texture.OffsetToTex = GtfTexture.Alignment;
            texture.TextureSize = 8;
            texture.Info.Format = TextureFormat.CompressedDxt1;
            texture.Info.Mipmap = 1;
            texture.Info.Dimension = TextureDimension.TwoDimensional;
            texture.Info.Remap = TextureRemap.OrderARGB;
            texture.Info.Width = 1;
            texture.Info.Height = 1;
            texture.Info.Depth = 1;

            if (BitConverter.IsLittleEndian)
            {
                gtfHeader.ReverseEndianness();
                texture.ReverseEndianness();
            }

            Unsafe.WriteUnaligned(ref gtfBytes[GtfTexture.Alignment], uint.MaxValue);
            return gtfBytes;
        }
    }

    [Fact]
    public void Constructor_PathNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GtfImage((string)null!));
    }

    [Fact]
    public void Constructor_PathEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new GtfImage(string.Empty));
    }

    [Fact]
    public void Constructor_StreamNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GtfImage((Stream)null!));
    }

    [Fact]
    public void Constructor_UnreadableStream_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new GtfImage(new UnreadableStream()));
    }

    [Fact]
    public void Constructor_EmptyStream_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage(new MemoryStream([])));
    }

    [Fact]
    public void Constructor_UnalignedStream_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage(new MemoryStream(new byte[GtfTexture.Alignment - 1])));
    }

    [Fact]
    public void Constructor_BufferNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GtfImage((byte[])null!));
    }

    [Fact]
    public void Constructor_BufferNegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GtfImage(GtfBytes, -1, 0));
    }

    [Fact]
    public void Constructor_BufferInvalidOffLen_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new GtfImage(GtfBytes, 0, -1));
    }

    [Fact]
    public void Constructor_EmptyBuffer_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage(Array.Empty<byte>()));
    }

    [Fact]
    public void Constructor_UnalignedBuffer_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage(new byte[GtfTexture.Alignment - 1]));
    }

    [Fact]
    public void Constructor_EmptySpan_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage([]));
    }

    [Fact]
    public void Constructor_UnalignedSpan_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage(stackalloc byte[(int)(GtfTexture.Alignment - 1)]));
    }

    [Fact]
    public void Constructor_PointerNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GtfImage(nint.Zero, 0));
    }

    [Fact]
    public void Constructor_EmptyPointer_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage(nint.MaxValue, 0));
    }

    [Fact]
    public void Constructor_UnalignedPointer_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new GtfImage(nint.MaxValue, GtfTexture.Alignment - 1));
    }

    [Fact]
    public void ConvertToDds_Path_MatchesGtfBytes()
    {
        using TempFile tempGtfFile = new();
        File.WriteAllBytes(tempGtfFile, GtfBytes);
        using GtfImage image = new(tempGtfFile);
        Assert.Equal(DdsImageTests.DdsBytes, image[0].ConvertToDds());
    }

    [Fact]
    public void ConvertToDds_Stream_MatchesGtfBytes()
    {
        using GtfImage image = new(new MemoryStream(GtfBytes));
        Assert.Equal(DdsImageTests.DdsBytes, image[0].ConvertToDds());
    }

    [Fact]
    public void ConvertToDds_Buffer_MatchesGtfBytes()
    {
        using GtfImage image = new(GtfBytes);
        Assert.Equal(DdsImageTests.DdsBytes, image[0].ConvertToDds());
    }

    [Fact]
    public void ConvertToDds_BufferSegment_MatchesGtfBytes()
    {
        byte[] buffer = [0, 0, 0, 0, .. GtfBytes, 0, 0, 0, 0];
        using GtfImage image = new(buffer, 4, buffer.Length - 8);
        Assert.Equal(DdsImageTests.DdsBytes, image[0].ConvertToDds());
    }

    [Fact]
    public void ConvertToDds_Span_MatchesGtfBytes()
    {
        Span<byte> buffer = [0, 0, 0, 0, .. GtfBytes, 0, 0, 0, 0];
        using GtfImage image = new(buffer[4..^4]);
        Assert.Equal(DdsImageTests.DdsBytes, image[0].ConvertToDds());
    }

    [Fact]
    public void ConvertToDds_Pointer_MatchesGtfBytes()
    {
        using NativeMemoryHandle handle = new(GtfBytes);
        using GtfImage image = new(handle.Pointer, (uint)handle.Length);
        Assert.Equal(DdsImageTests.DdsBytes, image[0].ConvertToDds());
    }

    [Fact]
    public void ConvertToDds_EmptyPaths_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => GtfImage.ConvertToDds(string.Empty, string.Empty));
    }
}
