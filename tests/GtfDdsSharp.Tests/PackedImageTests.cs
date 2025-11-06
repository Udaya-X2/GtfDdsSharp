using System.Runtime.CompilerServices;

namespace GtfDdsSharp.Tests;

public class PackedImageTests
{
    [Fact]
    public void ConvertToPackedGtf_NullDdsPaths_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => DdsImage.ConvertToPackedGtf(null!, "out.gtf"));
    }

    [Fact]
    public void ConvertToPackedGtf_NullDdsPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => DdsImage.ConvertToPackedGtf([null!], "out.gtf"));
    }

    [Fact]
    public void ConvertToPackedGtf_NullGtfPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => DdsImage.ConvertToPackedGtf([], null!));
    }

    [Fact]
    public void ConvertToPackedGtf_EmptyGtfPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => DdsImage.ConvertToPackedGtf([], ""));
    }

    [Fact]
    public void ConvertToPackedGtf_NoDdsPaths_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DdsImage.ConvertToPackedGtf([], "out.gtf"));
    }

    [Fact]
    public void ConvertToPackedGtf_TooManyDdsPaths_ThrowsArgumentOutOfRangeException()
    {
        using TempFile tempDdsFile = new();
        File.WriteAllBytes(tempDdsFile, DdsImageTests.DdsBytes);
        string[] paths = new string[byte.MaxValue + 1];
        Array.Fill(paths, tempDdsFile);
        Assert.Throws<ArgumentOutOfRangeException>(() => DdsImage.ConvertToPackedGtf(paths, "out.gtf"));
    }

    [Theory]
    [InlineData([new string[]
    {
        "32-bit-uncompressed-odd.dds",
        "32-bit-uncompressed.dds",
        "32bit.dds",
        "dds_a1r5g5b5.dds",
        "dds_A4R4G4B4.dds",
        "dds_A8B8G8R8.dds",
        "dds_R5G6B5.dds",
        "dxt1-alpha.dds",
        "dxt1-simple.dds",
        "dxt1.dds",
        "dxt3-simple.dds",
        "dxt3.dds",
        "dxt5-simple-1x1.dds",
        "dxt5-simple-odd.dds",
        "dxt5-simple.dds",
        "dxt5.dds",
        "TestVolume_Noise3D.dds",
        "wose_BC1_UNORM.DDS"
    }])]
    public void ConvertToPackedGtf_TexturesMatchDdsFiles(string[] paths)
    {
        paths = [.. paths.Select(x => Path.Combine("data", x))];
        using TempFile tempGtfFile = new();
        DdsImage.ConvertToPackedGtf(paths, tempGtfFile);
        using GtfImage gtfImage = new(tempGtfFile);
        Assert.Equal(paths.Length, gtfImage.Count);
        Assert.Equal(paths.Length, gtfImage.Textures.Length);
        Assert.Equal((uint)paths.Length, gtfImage.GtfHeader.NumTexture);
        Assert.Equal(GtfImage.DefaultVersion, gtfImage.GtfHeader.Version);
        Assert.All(paths, (path, idx) =>
        {
            Span<byte> ddsBytes = File.ReadAllBytes(path).AsSpan(Unsafe.SizeOf<DdsHeader>());
            Span<byte> convertedDdsBytes = gtfImage[idx].ConvertToDds().AsSpan(Unsafe.SizeOf<DdsHeader>());
            Assert.Equal(ddsBytes, convertedDdsBytes);
        });
    }
}
