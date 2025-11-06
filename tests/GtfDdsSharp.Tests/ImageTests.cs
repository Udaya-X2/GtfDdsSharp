using System.IO.Hashing;
using System.Runtime.CompilerServices;

namespace GtfDdsSharp.Tests;

public class ImageTests
{
    private const ConvertOptions AllConvertOptions = ConvertOptions.Linearize | ConvertOptions.Unnormalize;

    [Theory]
    [InlineData("32-bit-uncompressed-odd.dds", 384, 13368563919410162214, default(ConvertOptions))]
    [InlineData("32-bit-uncompressed-odd.dds", 384, 13368563919410162214, ConvertOptions.Linearize)]
    [InlineData("32-bit-uncompressed-odd.dds", 384, 6245573085793769997, ConvertOptions.Unnormalize)]
    [InlineData("32-bit-uncompressed-odd.dds", 384, 6245573085793769997, AllConvertOptions)]
    [InlineData("32-bit-uncompressed.dds", 16512, 6079733080728544909, default(ConvertOptions))]
    [InlineData("32-bit-uncompressed.dds", 16512, 11189146906834253186, ConvertOptions.Linearize)]
    [InlineData("32-bit-uncompressed.dds", 16512, 2732672395443679178, ConvertOptions.Unnormalize)]
    [InlineData("32-bit-uncompressed.dds", 16512, 12392011970516654478, AllConvertOptions)]
    [InlineData("32bit.dds", 57728, 11939762269334349623, default(ConvertOptions))]
    [InlineData("32bit.dds", 57728, 11939762269334349623, ConvertOptions.Linearize)]
    [InlineData("32bit.dds", 57728, 17530006696580244543, ConvertOptions.Unnormalize)]
    [InlineData("32bit.dds", 57728, 17530006696580244543, AllConvertOptions)]
    [InlineData("TestVolume_Noise3D.dds", 1048704, 8927476597336240183, default(ConvertOptions))]
    [InlineData("TestVolume_Noise3D.dds", 1048704, 3053099804886213757, ConvertOptions.Linearize)]
    [InlineData("TestVolume_Noise3D.dds", 1048704, 3101610745380336905, ConvertOptions.Unnormalize)]
    [InlineData("TestVolume_Noise3D.dds", 1048704, 13434706072678970451, AllConvertOptions)]
    [InlineData("dds_A4R4G4B4.dds", 131200, 13869916551962422461, default(ConvertOptions))]
    [InlineData("dds_A4R4G4B4.dds", 131200, 6537353154927144173, ConvertOptions.Linearize)]
    [InlineData("dds_A4R4G4B4.dds", 131200, 4622421161250179721, ConvertOptions.Unnormalize)]
    [InlineData("dds_A4R4G4B4.dds", 131200, 441026016272005240, AllConvertOptions)]
    [InlineData("dds_A8B8G8R8.dds", 262272, 1146403996947177003, default(ConvertOptions))]
    [InlineData("dds_A8B8G8R8.dds", 262272, 17226293733861433395, ConvertOptions.Linearize)]
    [InlineData("dds_A8B8G8R8.dds", 262272, 13055301076085433008, ConvertOptions.Unnormalize)]
    [InlineData("dds_A8B8G8R8.dds", 262272, 1812400904982048007, AllConvertOptions)]
    [InlineData("dds_R5G6B5.dds", 131200, 10251878464762421789, default(ConvertOptions))]
    [InlineData("dds_R5G6B5.dds", 131200, 6051063024602076389, ConvertOptions.Linearize)]
    [InlineData("dds_R5G6B5.dds", 131200, 16679785491474682069, ConvertOptions.Unnormalize)]
    [InlineData("dds_R5G6B5.dds", 131200, 11811006498805052220, AllConvertOptions)]
    [InlineData("dds_a1r5g5b5.dds", 114560, 7037845165887179682, default(ConvertOptions))]
    [InlineData("dds_a1r5g5b5.dds", 114560, 7037845165887179682, ConvertOptions.Linearize)]
    [InlineData("dds_a1r5g5b5.dds", 114560, 350627878470599178, ConvertOptions.Unnormalize)]
    [InlineData("dds_a1r5g5b5.dds", 114560, 350627878470599178, AllConvertOptions)]
    [InlineData("dxt1-alpha.dds", 5632, 8752948139368314066, default(ConvertOptions))]
    [InlineData("dxt1-alpha.dds", 5632, 8752948139368314066, ConvertOptions.Linearize)]
    [InlineData("dxt1-alpha.dds", 5632, 11852152578149302192, ConvertOptions.Unnormalize)]
    [InlineData("dxt1-alpha.dds", 5632, 11852152578149302192, AllConvertOptions)]
    [InlineData("dxt1-simple-1x1.dds", 256, 13480979166680028493, default(ConvertOptions))]
    [InlineData("dxt1-simple-1x1.dds", 256, 13480979166680028493, ConvertOptions.Linearize)]
    [InlineData("dxt1-simple-1x1.dds", 256, 2655728796952893799, ConvertOptions.Unnormalize)]
    [InlineData("dxt1-simple-1x1.dds", 256, 2655728796952893799, AllConvertOptions)]
    [InlineData("dxt1-simple.dds", 2176, 5069656836473927730, default(ConvertOptions))]
    [InlineData("dxt1-simple.dds", 2176, 5069656836473927730, ConvertOptions.Linearize)]
    [InlineData("dxt1-simple.dds", 2176, 1413255193489245727, ConvertOptions.Unnormalize)]
    [InlineData("dxt1-simple.dds", 2176, 1413255193489245727, AllConvertOptions)]
    [InlineData("dxt1.dds", 7424, 6240573825080741778, default(ConvertOptions))]
    [InlineData("dxt1.dds", 7424, 6240573825080741778, ConvertOptions.Linearize)]
    [InlineData("dxt1.dds", 7424, 13110752887045527958, ConvertOptions.Unnormalize)]
    [InlineData("dxt1.dds", 7424, 13110752887045527958, AllConvertOptions)]
    [InlineData("dxt3-simple.dds", 4224, 17682222143087068818, default(ConvertOptions))]
    [InlineData("dxt3-simple.dds", 4224, 17682222143087068818, ConvertOptions.Linearize)]
    [InlineData("dxt3-simple.dds", 4224, 9804796354126972957, ConvertOptions.Unnormalize)]
    [InlineData("dxt3-simple.dds", 4224, 9804796354126972957, AllConvertOptions)]
    [InlineData("dxt3.dds", 14592, 3039135753413809229, default(ConvertOptions))]
    [InlineData("dxt3.dds", 14592, 3039135753413809229, ConvertOptions.Linearize)]
    [InlineData("dxt3.dds", 14592, 11197262481773969622, ConvertOptions.Unnormalize)]
    [InlineData("dxt3.dds", 14592, 11197262481773969622, AllConvertOptions)]
    [InlineData("dxt5-simple-1x1.dds", 256, 8977367952752584867, default(ConvertOptions))]
    [InlineData("dxt5-simple-1x1.dds", 256, 8977367952752584867, ConvertOptions.Linearize)]
    [InlineData("dxt5-simple-1x1.dds", 256, 14451321300793646481, ConvertOptions.Unnormalize)]
    [InlineData("dxt5-simple-1x1.dds", 256, 14451321300793646481, AllConvertOptions)]
    [InlineData("dxt5-simple-odd.dds", 256, 1548195155925347465, default(ConvertOptions))]
    [InlineData("dxt5-simple-odd.dds", 256, 1548195155925347465, ConvertOptions.Linearize)]
    [InlineData("dxt5-simple-odd.dds", 256, 4656576186140426620, ConvertOptions.Unnormalize)]
    [InlineData("dxt5-simple-odd.dds", 256, 4656576186140426620, AllConvertOptions)]
    [InlineData("dxt5-simple.dds", 4224, 5246229534004691144, default(ConvertOptions))]
    [InlineData("dxt5-simple.dds", 4224, 5246229534004691144, ConvertOptions.Linearize)]
    [InlineData("dxt5-simple.dds", 4224, 6748930379480057778, ConvertOptions.Unnormalize)]
    [InlineData("dxt5-simple.dds", 4224, 6748930379480057778, AllConvertOptions)]
    [InlineData("dxt5.dds", 14592, 10136549805414629483, default(ConvertOptions))]
    [InlineData("dxt5.dds", 14592, 10136549805414629483, ConvertOptions.Linearize)]
    [InlineData("dxt5.dds", 14592, 4596341016560852395, ConvertOptions.Unnormalize)]
    [InlineData("dxt5.dds", 14592, 4596341016560852395, AllConvertOptions)]
    [InlineData("wose_BC1_UNORM.DDS", 5632, 4906576834033846448, default(ConvertOptions))]
    [InlineData("wose_BC1_UNORM.DDS", 5632, 4906576834033846448, ConvertOptions.Linearize)]
    [InlineData("wose_BC1_UNORM.DDS", 5632, 10139705931491098583, ConvertOptions.Unnormalize)]
    [InlineData("wose_BC1_UNORM.DDS", 5632, 10139705931491098583, AllConvertOptions)]
    public void Dds_Supported_MatchesProperties(string path, uint length, ulong hash, ConvertOptions options)
    {
        // Read DDS file bytes.
        path = Path.Combine("data", path);
        byte[] ddsBytes = File.ReadAllBytes(path);
        using NativeMemoryHandle ddsHandle = new(ddsBytes);

        // Test each DDS image constructor.
        using DdsImage ddsImagePath = new(path, options);
        using DdsImage ddsImageBuffer = new(ddsBytes, options);
        using DdsImage ddsImageStream = new(new MemoryStream(ddsBytes), options);
        using DdsImage ddsImageUnseekableStream = new(new UnseekableStream(new MemoryStream(ddsBytes)), options);
        using DdsImage ddsImagePointer = new(ddsHandle.Pointer, (uint)ddsHandle.Length, options);
        byte[] gtfBytes = new byte[length];

        // Convert each DDS image to GTF and compare to correct length/hash.
        ConvertDdsToGtf(gtfBytes, ddsImagePath);
        ConvertDdsToGtf(gtfBytes, ddsImageBuffer);
        ConvertDdsToGtf(gtfBytes, ddsImageStream);
        ConvertDdsToGtf(gtfBytes, ddsImageUnseekableStream);
        ConvertDdsToGtf(gtfBytes, ddsImagePointer);

        // Write converted GTF bytes to temp GTF file.
        using TempFile tempGtfFile = new();
        using (FileStream fs = new(tempGtfFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read, 0))
        {
            fs.Write(gtfBytes, 0, gtfBytes.Length);
        }

        // Test each GTF image constructor.
        using NativeMemoryHandle gtfHandle = new(gtfBytes);
        using GtfImage gtfImagePath = new(tempGtfFile);
        using GtfImage gtfImageBuffer = new(gtfBytes);
        using GtfImage gtfImageStream = new(new MemoryStream(gtfBytes));
        using GtfImage gtfImageUnseekableStream = new(new UnseekableStream(new MemoryStream(gtfBytes)));
        using GtfImage gtfImagePointer = new(gtfHandle.Pointer, (uint)gtfHandle.Length);
        byte[] convDdsBytes = new byte[ddsBytes.Length];

        // Convert each GTF image to DDS and compare to correct header/length/span.
        ConvertGtfToDds(ddsBytes, convDdsBytes, gtfImagePath);
        ConvertGtfToDds(ddsBytes, convDdsBytes, gtfImageBuffer);
        ConvertGtfToDds(ddsBytes, convDdsBytes, gtfImageStream);
        ConvertGtfToDds(ddsBytes, convDdsBytes, gtfImageUnseekableStream);
        ConvertGtfToDds(ddsBytes, convDdsBytes, gtfImagePointer);

        void ConvertDdsToGtf(byte[] gtfBytes, DdsImage ddsImage)
        {
            Assert.Equal(length, ddsImage.GtfFileSize);
            Array.Clear(gtfBytes);
            ddsImage.ConvertToGtf(gtfBytes);
            Assert.Equal(hash, XxHash3.HashToUInt64(gtfBytes));
            ddsImage.Dispose();
        }

        void ConvertGtfToDds(byte[] ddsBytes, byte[] convDdsBytes, GtfImage gtfImage)
        {
            Assert.Equal(1u, gtfImage.GtfHeader.NumTexture);
            Assert.Equal(length - GtfTexture.Alignment, gtfImage.GtfHeader.Size);
            Assert.Equal(GtfImage.DefaultVersion, gtfImage.GtfHeader.Version);
            GtfTexture gtfTexture = gtfImage[0];
            Assert.Equal((uint)ddsBytes.Length, gtfTexture.DdsFileSize);
            Array.Clear(convDdsBytes);
            gtfTexture.ConvertToDds(convDdsBytes);
            Assert.Equal(ddsBytes.AsSpan(Unsafe.SizeOf<DdsHeader>()),
                         convDdsBytes.AsSpan(Unsafe.SizeOf<DdsHeader>()));
            gtfImage.Dispose();
        }
    }

    [Theory]
    [InlineData("Antenna_Metal_0_Normal.dds")]
    [InlineData("bc4-simple.dds")]
    [InlineData("bc5-simple-snorm.dds")]
    [InlineData("bc5-simple.dds")]
    public void Dds_UnsupportedConstructor_ThrowsIOException(string path)
    {
        path = Path.Combine("data", path);
        byte[] bytes = File.ReadAllBytes(path);
        Assert.Throws<IOException>(() => new DdsImage(path));
    }

    [Theory]
    [InlineData("24-bit-uncompressed-bgr-odd.dds")]
    [InlineData("24-bit-uncompressed-odd.dds")]
    [InlineData("dds_R8G8B8.dds")]
    public void Dds_UnsupportedConversion_ThrowsArgumentException(string path)
    {
        path = Path.Combine("data", path);
        byte[] bytes = File.ReadAllBytes(path);
        using DdsImage ddsImage = new(path);
        Assert.Throws<ArgumentException>(ddsImage.ConvertToGtf);
    }

    [Theory]
    [InlineData("b5g5r5a1.dds")]
    [InlineData("b8g8r8x8.dds")]
    [InlineData("BC1_UNORM_SRGB-47.dds")]
    [InlineData("bc2-simple-srgb.dds")]
    [InlineData("bc3-simple-srgb.dds")]
    [InlineData("bc6h-simple.dds")]
    [InlineData("bc7-simple.dds")]
    [InlineData("wose_BC1_UNORM_SRGB.DDS")]
    [InlineData("wose_R8G8B8A8_UNORM_SRGB.DDS")]
    public void Dds_UnsupportedDX10_ThrowsNotSupportedException(string path)
    {
        path = Path.Combine("data", path);
        byte[] bytes = File.ReadAllBytes(path);
        DdsHeader ddsHeader = Unsafe.ReadUnaligned<DdsHeader>(in bytes[0]);

        if (!BitConverter.IsLittleEndian)
        {
            ddsHeader.ReverseEndianness();
        }

        Assert.Equal(DdsInfo.FOURCC_DX10, ddsHeader.DdsPF.FourCC);
        Assert.Throws<NotSupportedException>(() => new DdsImage(bytes));
    }
}
