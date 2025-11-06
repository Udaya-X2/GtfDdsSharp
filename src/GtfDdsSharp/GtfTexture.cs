using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents a texture in a GTF file. Provides properties and methods related to DDS file conversion.
/// </summary>
public unsafe class GtfTexture
{
    /// <summary>
    /// The byte boundary at which GTF textures are aligned.
    /// </summary>
    public const uint Alignment = 128;

    private readonly GtfImage _image;
    private readonly GtfTextureAttribute _texture;
    private readonly DdsHeader _ddsHeader;
    private readonly Layout[] _layouts;
    private readonly uint _ddsImageSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfTexture"/> class from the given GTF file and texture index.
    /// </summary>
    /// <param name="image">The GTF file containing one or more textures.</param>
    /// <param name="index">The zero-based index of the texture.</param>
    public GtfTexture(GtfImage image, int index)
    {
        ArgumentNullException.ThrowIfNull(image);
        ObjectDisposedException.ThrowIf(image.IsClosed, image);

        // Get the texture at the specified index in the GTF file.
        _image = image;
        _texture = image.GetTextureAttribute(index);

        // Create a DDS header based on the texture.
        _ddsHeader = ConvertHeader(in _texture.Info);

        // Generate a layout buffer based on the texture and DDS header.
        _layouts = TextureConversion.CreateLayoutBuffer(in _texture.Info,
                                                        in _ddsHeader,
                                                        out uint gtfImageSize,
                                                        out _ddsImageSize);

        if (_image.Size < gtfImageSize) ThrowHelper.ThrowIO_InvalidGtfTexture_EOF();
    }

    /// <summary>
    /// Gets the containing GTF file.
    /// </summary>
    public GtfImage Image => _image;

    /// <summary>
    /// Gets the corresponding texture attribute from the GTF file.
    /// </summary>
    public GtfTextureAttribute Texture => _texture;

    /// <summary>
    /// Gets the DDS header used for conversion.
    /// </summary>
    public DdsHeader DdsHeader => _ddsHeader;

    /// <summary>
    /// Gets the layout buffer used for conversion.
    /// </summary>
    public Layout[] Layouts => _layouts;

    /// <summary>
    /// Gets the size of the DDS image produced by converting this texture.
    /// </summary>
    public uint DdsImageSize => _ddsImageSize;

    /// <summary>
    /// Gets the size of the DDS file produced by converting this texture.
    /// </summary>
    public uint DdsFileSize => (uint)sizeof(DdsHeader) + _ddsImageSize;

    /// <summary>
    /// Converts the texture to a DDS file and writes it to a byte array.
    /// </summary>
    /// <returns>A byte array containing the DDS file.</returns>
    public byte[] ConvertToDds()
    {
        if (DdsFileSize > (uint)Array.MaxLength) ThrowHelper.ThrowIO_FileTooLong2GB();

        byte[] bytes = new byte[DdsFileSize];
        ConvertToDds(bytes);
        return bytes;
    }

    /// <summary>
    /// Converts the texture to a DDS file and writes it to the specified file.
    /// </summary>
    /// <param name="path">The file to write the DDS file to.</param>
    public void ConvertToDds(string path)
    {
        using FileStream stream = new(path, new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Share = FileShare.Read,
            PreallocationSize = DdsFileSize,
            BufferSize = 0
        });
        ConvertToDds(stream);
    }

    /// <summary>
    /// Converts the texture to a DDS file and writes it to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write the DDS file to.</param>
    public void ConvertToDds(Stream stream)
    {
        uint length = DdsFileSize;
        byte* pointer = (byte*)NativeMemory.Alloc(length);

        try
        {
            ConvertToDds(pointer, length);

            foreach (Span<byte> buffer in new PointerSpanEnumerator(pointer, length))
            {
                stream.Write(buffer);
            }
        }
        finally
        {
            NativeMemory.Free(pointer);
        }
    }

    /// <summary>
    /// Converts the texture to a DDS file and writes it to the specified array.
    /// </summary>
    /// <param name="buffer">The array to write the DDS file to.</param>
    public void ConvertToDds(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        ConvertToDds(buffer.AsSpan());
    }

    /// <summary>
    /// Converts the texture to a DDS file and writes it to the specified array region.
    /// </summary>
    /// <param name="buffer">The array to write the DDS file to.</param>
    /// <param name="offset">The zero-based byte offset at which to write bytes.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    public void ConvertToDds(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        ConvertToDds(buffer.AsSpan(offset, count));
    }

    /// <summary>
    /// Converts the texture to a DDS file and writes it to the specified region of memory.
    /// </summary>
    /// <param name="buffer">The region of memory to write the DDS file to.</param>
    public void ConvertToDds(Span<byte> buffer)
    {
        fixed (byte* pointer = buffer)
        {
            ConvertToDds(pointer, (uint)buffer.Length);
        }
    }

    /// <summary>
    /// Converts the texture to a DDS file and writes it to the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to write the DDS file to.</param>
    /// <param name="length">The maximum number of bytes to write.</param>
    public void ConvertToDds(nint pointer, uint length) => ConvertToDds((byte*)pointer, length);

    /// <summary>
    /// Converts the texture to a DDS file and writes it to the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to write the DDS file to.</param>
    /// <param name="length">The maximum number of bytes to write.</param>
    public void ConvertToDds(byte* pointer, uint length)
    {
        ObjectDisposedException.ThrowIf(_image.IsClosed, _image);
        ArgumentOutOfRangeException.ThrowIfLessThan(length, DdsFileSize);

        // Write the DDS file header in little-endian format.
        Unsafe.WriteUnaligned(pointer, _ddsHeader);

        if (!BitConverter.IsLittleEndian)
        {
            Unsafe.AsRef<DdsHeader>(pointer).ReverseEndianness();
        }

        // Initialize the DDS image pointer.
        byte* ddsImage = pointer + sizeof(DdsHeader);
        Unsafe.InitBlockUnaligned(ddsImage, 0, _ddsImageSize);

        // Convert the image data from GTF to DDS.
        byte* gtfImage = (byte*)_image.Data + _texture.OffsetToTex;
        TextureConversion.ConvertBufferByLayout(gtfImage,
                                                _image.Size - _texture.OffsetToTex,
                                                ddsImage,
                                                _ddsImageSize,
                                                _layouts,
                                                _texture.Info,
                                                true);
    }

    /// <summary>
    /// Creates a DDS file header based on the specified GTF texture.
    /// </summary>
    /// <param name="texture">The GTF texture.</param>
    /// <returns>A DDS file header.</returns>
    private static DdsHeader ConvertHeader(in GtfTextureInfo texture)
    {
        DdsHeader header = new();
        TextureFormat rawFormat = texture.GetRawFormat();
        bool isDxt = rawFormat.IsDxtn();
        bool noPitchOrLinearSize = rawFormat.IsRawCompressed();

        switch (rawFormat)
        {
            case TextureFormat.B8:
                header.DdsPF.Flags = DdsInfo.DDSF_LUMINANCE;
                header.DdsPF.RgbBitCount = 8;
                header.DdsPF.RBitMask = 0x000000ff;
                break;
            case TextureFormat.A1R5G5B5:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB | DdsInfo.DDPF_ALPHAPIXELS;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.ABitMask = 0x00008000;
                header.DdsPF.RBitMask = 0x00007c00;
                header.DdsPF.GBitMask = 0x000003e0;
                header.DdsPF.BBitMask = 0x0000001f;
                break;
            case TextureFormat.A4R4G4B4:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB | DdsInfo.DDPF_ALPHAPIXELS;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.ABitMask = 0x0000f000;
                header.DdsPF.RBitMask = 0x00000f00;
                header.DdsPF.GBitMask = 0x000000f0;
                header.DdsPF.BBitMask = 0x0000000f;
                break;
            case TextureFormat.R5G6B5:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.ABitMask = 0x00000000;
                header.DdsPF.RBitMask = 0x0000f800;
                header.DdsPF.GBitMask = 0x000007e0;
                header.DdsPF.BBitMask = 0x0000001f;
                break;
            case TextureFormat.R6G5B5:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.ABitMask = 0x00000000;
                header.DdsPF.RBitMask = 0x0000fc00;
                header.DdsPF.GBitMask = 0x000003e0;
                header.DdsPF.BBitMask = 0x0000001f;
                break;
            case TextureFormat.R5G5B5A1:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.ABitMask = 0x00000001;
                header.DdsPF.RBitMask = 0x0000f800;
                header.DdsPF.GBitMask = 0x000007c0;
                header.DdsPF.BBitMask = 0x0000003e;
                break;
            case TextureFormat.D1R5G5B5:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.ABitMask = 0x00008000;
                header.DdsPF.RBitMask = 0x00007c00;
                header.DdsPF.GBitMask = 0x000003e0;
                header.DdsPF.BBitMask = 0x0000001f;
                break;
            case TextureFormat.A8R8G8B8:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB | DdsInfo.DDPF_ALPHAPIXELS;
                header.DdsPF.RgbBitCount = 32;
                header.DdsPF.ABitMask = 0xff000000;
                header.DdsPF.RBitMask = 0x00ff0000;
                header.DdsPF.GBitMask = 0x0000ff00;
                header.DdsPF.BBitMask = 0x000000ff;
                break;
            case TextureFormat.D8R8G8B8:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB;
                header.DdsPF.RgbBitCount = 32;
                header.DdsPF.ABitMask = 0xff000000;
                header.DdsPF.RBitMask = 0x00ff0000;
                header.DdsPF.GBitMask = 0x0000ff00;
                header.DdsPF.BBitMask = 0x000000ff;
                break;
            // A8L8
            case TextureFormat.G8B8:
                header.DdsPF.Flags = DdsInfo.DDSF_LUMINANCE | DdsInfo.DDPF_ALPHAPIXELS;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.ABitMask = 0x0000ff00;
                header.DdsPF.RBitMask = 0x000000ff;
                break;
            // L16
            case TextureFormat.X16:
                header.DdsPF.Flags = DdsInfo.DDSF_LUMINANCE;
                header.DdsPF.RgbBitCount = 16;
                header.DdsPF.RBitMask = 0x0000ffff;
                break;
            // G16R16
            case TextureFormat.Y16X16:
                header.DdsPF.Flags = DdsInfo.DDPF_RGB;
                header.DdsPF.RgbBitCount = 32;
                header.DdsPF.ABitMask = 0x00000000;
                header.DdsPF.RBitMask = 0x0000ffff;
                header.DdsPF.GBitMask = 0xffff0000;
                header.DdsPF.BBitMask = 0x00000000;
                break;
            case TextureFormat.CompressedDxt1:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_DXT1;
                break;
            case TextureFormat.CompressedDxt23:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_DXT3;
                break;
            case TextureFormat.CompressedDxt45:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_DXT5;
                break;
            case TextureFormat.W16Z16Y16X16Float:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_A16B16G16R16F;
                break;
            case TextureFormat.W32Z32Y32X32Float:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_A32B32G32R32F;
                break;
            case TextureFormat.X32Float:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_R32F;
                break;
            case TextureFormat.Y16X16Float:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_G16R16F;
                break;
            case TextureFormat.CompressedB8R8G8R8Raw:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_R8G8_B8G8;
                break;
            case TextureFormat.CompressedR8B8R8G8Raw:
                header.DdsPF.Flags = DdsInfo.DDPF_FOURCC;
                header.DdsPF.FourCC = DdsInfo.FOURCC_G8R8_G8B8;
                break;
            // Unsupported
            case TextureFormat.CompressedHilo8:
            case TextureFormat.CompressedHiloS8:
            default:
                ThrowHelper.ThrowNotSupported_TextureFormat(rawFormat);
                break;
        }

        header.Magic = DdsInfo.FOURCC_DDS;
        header.Size = DdsInfo.DDS_HEADER_SIZE;
        header.Flags |= DdsInfo.DDSD_CAPS;
        header.Flags |= DdsInfo.DDSD_PIXELFORMAT;
        header.Flags |= DdsInfo.DDSD_WIDTH;
        header.Flags |= DdsInfo.DDSD_HEIGHT;
        header.Caps1 |= DdsInfo.DDSCAPS_TEXTURE;

        if (texture.Mipmap != 1)
        {
            header.Flags |= DdsInfo.DDSD_MIPMAPCOUNT;
            header.Caps1 |= DdsInfo.DDSCAPS_MIPMAP;
            header.Caps1 |= DdsInfo.DDSCAPS_COMPLEX;
            header.MipmapCount = texture.Mipmap;
        }
        if (texture.Dimension == TextureDimension.ThreeDimensional)
        {
            header.Flags |= DdsInfo.DDSD_DEPTH;
            header.Caps1 |= DdsInfo.DDSCAPS2_VOLUME;
            header.Caps1 |= DdsInfo.DDSCAPS_COMPLEX;
            header.Depth = texture.Depth;
        }
        if (texture.IsCubemap)
        {
            header.Caps1 |= DdsInfo.DDSCAPS2_CUBEMAP;
            header.Caps1 |= DdsInfo.DDSCAPS_COMPLEX;
            header.Caps2 |= DdsInfo.DDSCAPS2_CUBEMAP_POSITIVEX;
            header.Caps2 |= DdsInfo.DDSCAPS2_CUBEMAP_NEGATIVEX;
            header.Caps2 |= DdsInfo.DDSCAPS2_CUBEMAP_POSITIVEY;
            header.Caps2 |= DdsInfo.DDSCAPS2_CUBEMAP_NEGATIVEY;
            header.Caps2 |= DdsInfo.DDSCAPS2_CUBEMAP_POSITIVEZ;
            header.Caps2 |= DdsInfo.DDSCAPS2_CUBEMAP_NEGATIVEZ;
        }
        if ((header.DdsPF.Flags & DdsInfo.DDPF_ALPHAPIXELS) != 0)
        {
            header.Caps1 |= DdsInfo.DDSCAPS_ALPHA;
        }

        header.DdsPF.Size = DdsInfo.DDS_PIXELFORMAT_SIZE;
        header.Width = texture.Width;
        header.Height = texture.Height;

        if (noPitchOrLinearSize)
        {
            header.PitchOrLinearSize = 0;
        }
        else if (isDxt)
        {
            ushort colorDepth = rawFormat.GetDepth();
            header.PitchOrLinearSize = (texture.Width + 3u) / 4u * ((texture.Height + 3u) / 4u) * colorDepth;
            header.Flags |= DdsInfo.DDSD_LINEARSIZE;
        }
        else if (texture.Pitch != 0)
        {
            header.PitchOrLinearSize = texture.Pitch;
            header.Flags |= DdsInfo.DDSD_PITCH;
        }

        return header;
    }
}
