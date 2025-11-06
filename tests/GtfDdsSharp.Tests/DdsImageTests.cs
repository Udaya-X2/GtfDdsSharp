using System.Runtime.CompilerServices;

namespace GtfDdsSharp.Tests;

public class DdsImageTests
{
    /// <summary>
    /// Returns a byte array that represents "data/dxt1-simple-1x1.dds".
    /// </summary>
    internal static byte[] DdsBytes
    {
        get
        {
            byte[] ddsBytes = new byte[Unsafe.SizeOf<DdsHeader>() + 8];
            ref DdsHeader ddsHeader = ref Unsafe.As<byte, DdsHeader>(ref ddsBytes[0]);
            ddsHeader.Magic = DdsInfo.FOURCC_DDS;
            ddsHeader.Size = DdsInfo.DDS_HEADER_SIZE;
            ddsHeader.Flags = DdsInfo.DDSF_CAPS
                              | DdsInfo.DDSF_HEIGHT
                              | DdsInfo.DDSF_WIDTH
                              | DdsInfo.DDSF_PIXELFORMAT
                              | DdsInfo.DDSF_LINEARSIZE;
            ddsHeader.Height = 1;
            ddsHeader.Width = 1;
            ddsHeader.PitchOrLinearSize = 8;
            ddsHeader.DdsPF.Size = DdsInfo.DDS_PIXELFORMAT_SIZE;
            ddsHeader.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
            ddsHeader.DdsPF.FourCC = DdsInfo.FOURCC_DXT1;
            ddsHeader.Caps1 = DdsInfo.DDSCAPS_TEXTURE;

            if (!BitConverter.IsLittleEndian)
            {
                ddsHeader.ReverseEndianness();
            }

            Unsafe.WriteUnaligned(ref ddsBytes[Unsafe.SizeOf<DdsHeader>()], uint.MaxValue);
            return ddsBytes;
        }
    }

    [Fact]
    public void Constructor_PathNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DdsImage((string)null!));
    }

    [Fact]
    public void Constructor_PathEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DdsImage(string.Empty));
    }

    [Fact]
    public void Constructor_StreamNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DdsImage((Stream)null!));
    }

    [Fact]
    public void Constructor_UnreadableStream_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DdsImage(new UnreadableStream()));
    }

    [Fact]
    public void Constructor_SmallStream_ThrowsIOException()
    {
        // Any stream smaller than the DDS header should fail.
        Assert.Throws<IOException>(() => new DdsImage(new MemoryStream(new byte[Unsafe.SizeOf<DdsHeader>() - 1])));
    }

    [Fact]
    public void Constructor_BufferNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DdsImage((byte[])null!));
    }

    [Fact]
    public void Constructor_BufferNegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DdsImage(DdsBytes, -1, 0));
    }

    [Fact]
    public void Constructor_BufferInvalidOffLen_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DdsImage(DdsBytes, 0, -1));
    }

    [Fact]
    public void Constructor_SmallBuffer_ThrowsIOException()
    {
        // Any buffer smaller than the DDS header should fail.
        Assert.Throws<IOException>(() => new DdsImage(new byte[Unsafe.SizeOf<DdsHeader>() - 1]));
    }

    [Fact]
    public void Constructor_SmallSpan_ThrowsIOException()
    {
        // Any span smaller than the DDS header should fail.
        Assert.Throws<IOException>(() => new DdsImage(stackalloc byte[Unsafe.SizeOf<DdsHeader>() - 1]));
    }

    [Fact]
    public void Constructor_PointerNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DdsImage(nint.Zero, 0));
    }

    [Fact]
    public void Constructor_SmallPointer_ThrowsIOException()
    {
        // Any pointer smaller than the DDS header should fail (before the pointer is dereferenced).
        Assert.Throws<IOException>(() => new DdsImage(nint.MaxValue, (uint)(Unsafe.SizeOf<DdsHeader>() - 1)));
    }

    [Fact]
    public void ConvertToGtf_Path_MatchesGtfBytes()
    {
        using TempFile tempDdsFile = new();
        File.WriteAllBytes(tempDdsFile, DdsBytes);
        using DdsImage image = new(tempDdsFile);
        Assert.Equal(GtfImageTests.GtfBytes, image.ConvertToGtf());
    }

    [Fact]
    public void ConvertToGtf_Stream_MatchesGtfBytes()
    {
        using DdsImage image = new(new MemoryStream(DdsBytes));
        Assert.Equal(GtfImageTests.GtfBytes, image.ConvertToGtf());
    }

    [Fact]
    public void ConvertToGtf_Buffer_MatchesGtfBytes()
    {
        using DdsImage image = new(DdsBytes);
        Assert.Equal(GtfImageTests.GtfBytes, image.ConvertToGtf());
    }

    [Fact]
    public void ConvertToGtf_BufferSegment_MatchesGtfBytes()
    {
        byte[] buffer = [0, 0, 0, 0, .. DdsBytes, 0, 0, 0, 0];
        using DdsImage image = new(buffer, 4, buffer.Length - 8);
        Assert.Equal(GtfImageTests.GtfBytes, image.ConvertToGtf());
    }

    [Fact]
    public void ConvertToGtf_Span_MatchesGtfBytes()
    {
        Span<byte> buffer = [0, 0, 0, 0, .. DdsBytes, 0, 0, 0, 0];
        using DdsImage image = new(buffer[4..^4]);
        Assert.Equal(GtfImageTests.GtfBytes, image.ConvertToGtf());
    }

    [Fact]
    public void ConvertToGtf_Pointer_MatchesGtfBytes()
    {
        using NativeMemoryHandle handle = new(DdsBytes);
        using DdsImage image = new(handle.Pointer, (uint)handle.Length);
        Assert.Equal(GtfImageTests.GtfBytes, image.ConvertToGtf());
    }

    [Fact]
    public void ConvertToGtf_EmptyPaths_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => DdsImage.ConvertToGtf(string.Empty, string.Empty));
    }

    [Fact]
    public void ConvertToPackedGtf_NullPaths_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => DdsImage.ConvertToPackedGtf(null!, "out.gtf"));
    }

    [Fact]
    public void ConvertToPackedGtf_NullGtfPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => DdsImage.ConvertToPackedGtf(["input.dds"], null!));
    }

    [Fact]
    public void ConvertToPackedGtf_EmptyGtfPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => DdsImage.ConvertToPackedGtf(["input.dds"], string.Empty));
    }
}
