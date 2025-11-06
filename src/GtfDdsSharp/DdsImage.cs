using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Provides properties and methods to read a DDS file and convert it to a GTF file.
/// </summary>
public unsafe partial class DdsImage : IDisposable
{
    private readonly uint _size;
    private readonly ConvertOptions _options;
    private readonly DdsHeader _ddsHeader;
    private readonly GtfTextureInfo _texture;
    private readonly Layout[] _layouts;
    private readonly uint _gtfImageSize;

    private byte* _data;
    private nint _handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImage"/> class from the specified file.
    /// </summary>
    /// <param name="path">The path to the DDS file.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public DdsImage(string path, ConvertOptions options = default)
        : this(ReadFile(path, out uint length, options), 0, length, MemoryType.Native, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImage"/> class from the specified stream.
    /// </summary>
    /// <param name="stream">The stream containing the DDS file data.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public DdsImage(Stream stream, ConvertOptions options = default)
        : this(ReadStream(stream, out uint length, options), 0, length, MemoryType.Native, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImage"/> class from the specified array.
    /// </summary>
    /// <param name="buffer">An array of bytes containing the DDS file data.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public DdsImage(byte[] buffer, ConvertOptions options = default)
        : this(MemoryUtils.PinMemory(buffer), 0, (uint)buffer.Length, MemoryType.Handle, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImage"/> class from the specified array region.
    /// </summary>
    /// <param name="buffer">An array of bytes containing the DDS file data.</param>
    /// <param name="offset">The zero-based byte offset at which to read bytes.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public DdsImage(byte[] buffer, int offset, int count, ConvertOptions options = default)
        : this(MemoryUtils.PinMemory(buffer, offset, count), (uint)offset, (uint)count, MemoryType.Handle, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImage"/> class from the specified region of memory.
    /// </summary>
    /// <param name="buffer">The region of memory containing the DDS file data.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public DdsImage(ReadOnlySpan<byte> buffer, ConvertOptions options = default)
        : this(MemoryUtils.CopyMemory(buffer), 0, (uint)buffer.Length, MemoryType.Native, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImage"/> class from the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to the DDS file data.</param>
    /// <param name="length">The maximum number of bytes to read.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public DdsImage(nint pointer, uint length, ConvertOptions options = default)
        : this(pointer, 0, length, MemoryType.External, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImage"/> class from the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to the DDS file data.</param>
    /// <param name="offset">The zero-based byte offset at which to read bytes.</param>
    /// <param name="length">The maximum number of bytes to read.</param>
    /// <param name="memoryType">Specifies the backing memory for <paramref name="pointer"/>.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    private DdsImage(nint pointer, uint offset, uint length, MemoryType memoryType, ConvertOptions options)
    {
        ArgumentNullException.ThrowIfNull((void*)pointer);

        // Initialize the backing memory for the file data.
        MemoryUtils.InitializeBackingMemory(pointer, offset, memoryType, out _data, out _handle);
        _size = length;
        _options = options;

        try
        {
            // Read the header and convert it to a GTF texture.
            _ddsHeader = ReadHeader();
            _texture = ConvertHeader(in _ddsHeader, options);

            // Generate a layout buffer based on the texture and DDS header.
            _layouts = TextureConversion.CreateLayoutBuffer(in _texture,
                                                            in _ddsHeader,
                                                            out _gtfImageSize,
                                                            out uint ddsImageSize);

            if (_size - (uint)sizeof(DdsHeader) < ddsImageSize) ThrowHelper.ThrowIO_InvalidDds_EOF();
        }
        catch
        {
            // Dispose if an error occurs.
            Dispose(true);
            throw;
        }
    }

    /// <summary>
    /// Frees unmanaged memory in the event that the <see cref="DdsImage"/> is not properly disposed.
    /// </summary>
    ~DdsImage()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets a pointer to the file data.
    /// </summary>
    public nint Data => (nint)_data;

    /// <summary>
    /// Gets the size of the file data in bytes.
    /// </summary>
    public uint Size => _size;

    /// <summary>
    /// Gets the conversion options for this <see cref="DdsImage"/>.
    /// </summary>
    public ConvertOptions Options => _options;

    /// <summary>
    /// Gets the DDS file header.
    /// </summary>
    public DdsHeader DdsHeader => _ddsHeader;

    /// <summary>
    /// Gets the GTF texture used for conversion.
    /// </summary>
    public GtfTextureInfo Texture => _texture;

    /// <summary>
    /// Gets the layout buffer used for conversion.
    /// </summary>
    public Layout[] Layouts => _layouts;

    /// <summary>
    /// Gets the size of the GTF image produced by converting this texture.
    /// </summary>
    public uint GtfImageSize => _gtfImageSize;

    /// <summary>
    /// Gets the size of the GTF file produced by converting this texture.
    /// </summary>
    public uint GtfFileSize => MemoryUtils.GetGtfAlignment(GtfTexture.Alignment + _gtfImageSize);

    /// <summary>
    /// Gets whether the <see cref="DdsImage"/> was disposed.
    /// </summary>
    public bool IsClosed => _data is null;

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to a byte array.
    /// </summary>
    /// <returns>A byte array containing the GTF file.</returns>
    public byte[] ConvertToGtf() => new DdsImageCollection(this).ConvertToGtf();

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to the specified file.
    /// </summary>
    /// <param name="path">The file to write the GTF file to.</param>
    public void ConvertToGtf(string path) => new DdsImageCollection(this).ConvertToGtf(path);

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write the GTF file to.</param>
    public void ConvertToGtf(Stream stream) => new DdsImageCollection(this).ConvertToGtf(stream);

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to the specified array.
    /// </summary>
    /// <param name="buffer">The array to write the GTF file to.</param>
    public void ConvertToGtf(byte[] buffer) => new DdsImageCollection(this).ConvertToGtf(buffer);

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to the specified array region.
    /// </summary>
    /// <param name="buffer">The array to write the GTF file to.</param>
    /// <param name="offset">The zero-based byte offset at which to write bytes.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    public void ConvertToGtf(byte[] buffer, int offset, int count)
        => new DdsImageCollection(this).ConvertToGtf(buffer, offset, count);

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to the specified region of memory.
    /// </summary>
    /// <param name="buffer">The region of memory to write the GTF file to.</param>
    public void ConvertToGtf(Span<byte> buffer) => new DdsImageCollection(this).ConvertToGtf(buffer);

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to write the GTF file to.</param>
    /// <param name="length">The maximum number of bytes to write.</param>
    public void ConvertToGtf(nint pointer, uint length) => new DdsImageCollection(this).ConvertToGtf(pointer, length);

    /// <summary>
    /// Converts the DDS image to a GTF file and writes it to the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to write the GTF file to.</param>
    /// <param name="length">The maximum number of bytes to write.</param>
    public void ConvertToGtf(byte* pointer, uint length) => new DdsImageCollection(this).ConvertToGtf(pointer, length);

    /// <summary>
    /// Converts the specified DDS file to a GTF file.
    /// </summary>
    /// <param name="ddsPath">The path of the DDS file to read from.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public static void ConvertToGtf(string ddsPath, ConvertOptions options = default)
        => ConvertToGtf(ddsPath, Path.ChangeExtension(ddsPath, ".gtf"), options);

    /// <summary>
    /// Converts the specified DDS file to a GTF file.
    /// </summary>
    /// <param name="ddsPath">The path of the DDS file to read from.</param>
    /// <param name="gtfPath">The path to write the GTF file to.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public static void ConvertToGtf(string ddsPath, string gtfPath, ConvertOptions options = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(ddsPath);
        ArgumentException.ThrowIfNullOrEmpty(gtfPath);

        using DdsImage image = new(ddsPath, options);
        image.ConvertToGtf(gtfPath);
    }

    /// <summary>
    /// Converts the specified DDS files to a packed GTF file.
    /// </summary>
    /// <param name="ddsPaths">The paths of the DDS files to read from.</param>
    /// <param name="gtfPath">The path to write the packed GTF file to.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public static void ConvertToPackedGtf(IEnumerable<string> ddsPaths,
                                          string gtfPath,
                                          ConvertOptions options = default)
    {
        ArgumentNullException.ThrowIfNull(ddsPaths);
        ArgumentException.ThrowIfNullOrEmpty(gtfPath);

        using DdsImageCollection images = new(ddsPaths, options);
        images.ConvertToGtf(gtfPath);
    }

    /// <summary>
    /// Reads the bytes of the specified file to a block of memory.
    /// </summary>
    /// <param name="path">The file to read.</param>
    /// <param name="length">The length of the stream.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    /// <returns>A pointer to the block of memory containing the file bytes.</returns>
    private static nint ReadFile(string path, out uint length, ConvertOptions options)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0);
        return ReadStream(stream, out length, options);
    }

    /// <summary>
    /// Reads the bytes of the specified stream to a block of memory.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="length">The length of the stream.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    /// <returns>A pointer to the block of memory containing the stream bytes.</returns>
    private static nint ReadStream(Stream stream, out uint length, ConvertOptions options)
        => MemoryUtils.TryReadStream(stream, out length, out nint pointer)
        ? pointer
        : ReadUnseekableStream(stream, out length, options);

    /// <summary>
    /// Reads the bytes of the specified unseekable stream to a block of memory.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="length">The length of the stream.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    /// <returns>A pointer to the block of memory containing the stream bytes.</returns>
    private static nint ReadUnseekableStream(Stream stream, out uint length, ConvertOptions options)
    {
        // Allocate a block of memory to store the file header.
        byte* pointer = (byte*)NativeMemory.Alloc((uint)sizeof(DdsHeader));

        try
        {
            // Read and validate the file header in little-endian format.
            stream.ReadExactly(new Span<byte>(pointer, sizeof(DdsHeader)));
            DdsHeader ddsHeader = Unsafe.ReadUnaligned<DdsHeader>(pointer);

            if (!BitConverter.IsLittleEndian)
            {
                ddsHeader.ReverseEndianness();
            }

            ValidateHeader(in ddsHeader);

            // Convert the DDS header to a GTF texture.
            GtfTextureInfo texture = ConvertHeader(in ddsHeader, options);

            // Compute the file size.
            TextureConversion.CreateLayoutBuffer(in texture, in ddsHeader, out _, out uint ddsImageSize);
            length = (uint)sizeof(DdsHeader) + ddsImageSize;

            // Reallocate the block of memory to store the texture data.
            pointer = (byte*)NativeMemory.Realloc(pointer, length);

            // Read the texture data.
            foreach (Span<byte> buffer in new PointerSpanEnumerator(pointer + sizeof(DdsHeader), ddsImageSize))
            {
                stream.ReadExactly(buffer);
            }

            return (nint)pointer;
        }
        catch
        {
            NativeMemory.Free(pointer);
            throw;
        }
    }

    /// <summary>
    /// Reads the header from the start of the DDS file.
    /// </summary>
    /// <returns>The DDS file header.</returns>
    private DdsHeader ReadHeader()
    {
        if (_size < (uint)sizeof(DdsHeader)) ThrowHelper.ThrowIO_InvalidDdsHeader_EOF();

        DdsHeader ddsHeader = Unsafe.ReadUnaligned<DdsHeader>(_data);

        // Read the header in little-endian format.
        if (!BitConverter.IsLittleEndian)
        {
            ddsHeader.ReverseEndianness();
        }

        ValidateHeader(in ddsHeader);
        return ddsHeader;
    }

    /// <summary>
    /// Validates the specified DDS file header.
    /// </summary>
    /// <param name="ddsHeader">The DDS file header.</param>
    private static void ValidateHeader(in DdsHeader ddsHeader)
    {
        if (ddsHeader.Magic != DdsInfo.FOURCC_DDS) ThrowHelper.ThrowIO_InvalidDdsHeader_Magic();
        if (ddsHeader.Size != DdsInfo.DDS_HEADER_SIZE) ThrowHelper.ThrowIO_InvalidDdsHeader_Size();
        if (ddsHeader.DdsPF.Size != DdsInfo.DDS_PIXELFORMAT_SIZE) ThrowHelper.ThrowIO_InvalidDdsHeader_PFSize();
        if (ddsHeader.DdsPF.FourCC == DdsInfo.FOURCC_DX10) ThrowHelper.ThrowNotSupported_DX10();
    }

    /// <summary>
    /// Creates a GTF texture based on the specified DDS file header and conversion options.
    /// </summary>
    /// <param name="ddsHeader">The DDS file header.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    /// <returns>A GTF texture.</returns>
    private static GtfTextureInfo ConvertHeader(in DdsHeader ddsHeader, ConvertOptions options)
    {
        GtfTextureInfo texture;
        ConvertFormat(in ddsHeader, out texture.Format, out texture.Remap);
        texture.Width = (ushort)ddsHeader.Width;
        texture.Height = (ushort)ddsHeader.Height;
        texture.Mipmap = 1;
        texture.Depth = 1;
        texture.Dimension = TextureDimension.TwoDimensional;
        texture.IsCubemap = false;
        texture.Pitch = 0;
        texture.Location = 0;
        texture.Offset = 0;
        texture.Padding = 0;

        if ((ddsHeader.Caps2 & DdsInfo.DDSCAPS2_VOLUME) != 0 && (ddsHeader.Flags & DdsInfo.DDSD_DEPTH) != 0)
        {
            texture.Depth = (ushort)ddsHeader.Depth;
            texture.Dimension = TextureDimension.ThreeDimensional;
        }
        if ((ddsHeader.Flags & DdsInfo.DDSD_MIPMAPCOUNT) != 0)
        {
            if (MemoryUtils.GetMipmapSize(texture.Width, texture.Height, texture.Depth) < ddsHeader.MipmapCount)
            {
                ThrowHelper.ThrowIO_InvalidDdsHeader_Convert();
            }

            texture.Mipmap = (byte)ddsHeader.MipmapCount;
        }
        if ((ddsHeader.Caps2 & DdsInfo.DDSCAPS2_CUBEMAP) != 0)
        {
            if ((ddsHeader.Caps2 & DdsInfo.DDSCAPS2_CUBEMAP_ALL_FACES) != DdsInfo.DDSCAPS2_CUBEMAP_ALL_FACES)
            {
                ThrowHelper.ThrowIO_InvalidDdsHeader_Convert();
            }

            texture.IsCubemap = true;
        }
        if (texture.Dimension == TextureDimension.ThreeDimensional)
        {
            if (texture.Width > 512 || texture.Height > 512 || texture.Depth > 512)
            {
                ThrowHelper.ThrowIO_InvalidDdsHeader_Convert();
            }
        }
        else if (texture.Dimension == TextureDimension.TwoDimensional)
        {
            if (texture.Width > 4096 || texture.Height > 4096)
            {
                ThrowHelper.ThrowIO_InvalidDdsHeader_Convert();
            }
        }

        // Swizzle check
        TextureFormat rawFormat = texture.GetRawFormat();
        bool isDxt = rawFormat.IsDxtn();
        bool isSwizzle = texture.IsSwizzlable();

        if (!isDxt)
        {
            isSwizzle &= !options.HasFlag(ConvertOptions.Linearize);
        }
        if (isSwizzle)
        {
            texture.Format &= ~TextureFormat.Linear;
            texture.Pitch = 0;
        }
        else
        {
            texture.Format |= TextureFormat.Linear;
            texture.Pitch = rawFormat.GetPitch(texture.Width);
        }

        // Normalize check
        if (options.HasFlag(ConvertOptions.Unnormalize))
        {
            texture.Format |= TextureFormat.Unnormalize;
        }

        return texture;
    }

    /// <summary>
    /// Converts the specified DDS file header to an equivalent GTF texture format and remap value.
    /// </summary>
    /// <param name="ddsHeader">The DDS file header.</param>
    /// <param name="format">The converted GTF texture format.</param>
    /// <param name="remap">The converted remap value.</param>
    private static void ConvertFormat(in DdsHeader ddsHeader, out TextureFormat format, out uint remap)
    {
        format = default;
        remap = TextureRemap.OrderARGB;
        int aBits = BitOperations.PopCount(ddsHeader.DdsPF.ABitMask);
        int rBits = BitOperations.PopCount(ddsHeader.DdsPF.RBitMask);
        int gBits = BitOperations.PopCount(ddsHeader.DdsPF.GBitMask);
        int bBits = BitOperations.PopCount(ddsHeader.DdsPF.BBitMask);

        switch (ddsHeader.DdsPF.Flags)
        {
            case DdsInfo.DDPF_FOURCC:
            case DdsInfo.DDPF_FOURCC | DdsInfo.DDPF_NORMAL:
                switch (ddsHeader.DdsPF.FourCC)
                {
                    case DdsInfo.FOURCC_DXT1:
                        format = TextureFormat.CompressedDxt1;
                        break;
                    case DdsInfo.FOURCC_DXT2:
                        format = TextureFormat.CompressedDxt23;
                        break;
                    case DdsInfo.FOURCC_DXT3:
                        format = TextureFormat.CompressedDxt23;
                        break;
                    case DdsInfo.FOURCC_DXT4:
                        format = TextureFormat.CompressedDxt45;
                        break;
                    case DdsInfo.FOURCC_DXT5:
                        format = TextureFormat.CompressedDxt45;
                        break;
                    case DdsInfo.FOURCC_R16F:
                        format = TextureFormat.Y16X16Float;
                        break;
                    case DdsInfo.FOURCC_G16R16F:
                        format = TextureFormat.Y16X16Float;
                        break;
                    case DdsInfo.FOURCC_A16B16G16R16F:
                        format = TextureFormat.W16Z16Y16X16Float;
                        break;
                    case DdsInfo.FOURCC_R32F:
                        format = TextureFormat.X32Float;
                        break;
                    case DdsInfo.FOURCC_A32B32G32R32F:
                        format = TextureFormat.W32Z32Y32X32Float;
                        break;
                    case DdsInfo.FOURCC_R8G8_B8G8:
                        format = TextureFormat.CompressedB8R8G8R8;
                        remap = TextureRemap.OrderAGRB;
                        break;
                    case DdsInfo.FOURCC_G8R8_G8B8:
                        format = TextureFormat.CompressedR8B8R8G8;
                        remap = TextureRemap.OrderAGRB;
                        break;
                    case DdsInfo.FOURCC_YVYU:
                        format = TextureFormat.CompressedR8B8R8G8;
                        remap = TextureRemap.OrderARBG;
                        break;
                    case DdsInfo.FOURCC_YUY2:
                        format = TextureFormat.CompressedB8R8G8R8;
                        remap = TextureRemap.OrderARBG;
                        break;
                    // Unsupported
                    case DdsInfo.FOURCC_DDS:
                    case DdsInfo.FOURCC_RXGB:
                    case DdsInfo.FOURCC_ATI1:
                    case DdsInfo.FOURCC_ATI2:
                    default:
                        format = default;
                        break;
                }
                break;
            case DdsInfo.DDPF_RGB: // RGBBitCount, RGBBitMask are defined
            case DdsInfo.DDPF_RGB | DdsInfo.DDPF_ALPHAPIXELS: // Alpha is also defined
            case DdsInfo.DDPF_ALPHA: // Only alpha (1 component)
            case DdsInfo.DDPF_LUMINANCE: // Expand 1 ch data to RGB
            case DdsInfo.DDPF_LUMINANCE | DdsInfo.DDPF_ALPHAPIXELS: // Contains alpha
            case 0x00040000: // R6G5B5
            case DdsInfo.DDSF_BUMPDUDV: // X16Y16 bumpmap
                // In case of any of color bits is more than 8 bits long.
                if (aBits == 16 || rBits == 16 || gBits == 16 || bBits == 16)
                {
                    remap = TextureRemap.OrderARGB;
                }
                else if (ddsHeader.DdsPF.RgbBitCount == 8)
                {
                    format = TextureFormat.B8;
                    remap = ddsHeader.DdsPF.RBitMask != 0
                        ? TextureRemap.Order1BBB // L8 format
                        : TextureRemap.OrderB000; // R8 format
                }
                // If RGB bits are not null (defined)
                else if ((ddsHeader.DdsPF.RBitMask
                    | ddsHeader.DdsPF.GBitMask
                    | ddsHeader.DdsPF.BBitMask
                    | ddsHeader.DdsPF.ABitMask) != 0)
                {
                    remap = GetRemapFromBitMask(in ddsHeader.DdsPF);
                }
                // Raw bitmap
                else
                {
                    remap = TextureRemap.OrderBGRA;
                }
                break;
            default:
                // Pixel format is not supported
                format = default;
                remap = default;
                return;
        }

        // Convert format
        if ((ddsHeader.DdsPF.Flags & DdsInfo.DDPF_RGB) != 0)
        {
            // 8-bit
            if (ddsHeader.DdsPF.RgbBitCount == 8)
            {
                format = TextureFormat.B8;
            }
            // 16-bit
            else if (ddsHeader.DdsPF.RgbBitCount == 16)
            {
                if (aBits == 0)
                {
                    if (rBits == 5 && gBits == 6 && bBits == 5)
                    {
                        format = TextureFormat.R5G6B5;
                        remap = TextureRemap.Order1RGB;
                    }
                    else if (rBits == 6 && gBits == 5 && bBits == 5)
                    {
                        format = TextureFormat.R6G5B5;
                        remap = TextureRemap.Order1RGB;
                    }
                    else if (rBits == 5 && gBits == 5 && bBits == 5)
                    {
                        format = TextureFormat.D1R5G5B5;
                        remap = TextureRemap.Order1RGB;
                    }
                    else if (rBits == 4 && gBits == 4 && bBits == 4)
                    {
                        format = TextureFormat.A4R4G4B4;
                        remap = TextureRemap.Order1RGB;
                    }
                }
                else if (aBits == 1)
                {
                    if (ddsHeader.DdsPF.ABitMask == 0x00008000)
                    {
                        format = TextureFormat.A1R5G5B5;
                    }
                    else if (ddsHeader.DdsPF.ABitMask == 0x00000001)
                    {
                        format = TextureFormat.R5G5B5A1;
                    }
                }
                else if (aBits == 4)
                {
                    format = TextureFormat.A4R4G4B4;
                }
                else if ((aBits == 8 && rBits == 8) || (gBits == 8 && bBits == 8))
                {
                    format = TextureFormat.G8B8;
                }
                else if (aBits == 16 || rBits == 16 || gBits == 16 || bBits == 16)
                {
                    format = TextureFormat.X16;
                }
            }
            // 24-bit
            else if (ddsHeader.DdsPF.RgbBitCount == 24)
            {
                format = TextureFormat.D8R8G8B8;
                remap = TextureRemap.Order1RGB;
            }
            // 32-bit
            else if (ddsHeader.DdsPF.RgbBitCount == 32)
            {
                if (rBits != 8)
                {
                    int count = 0;

                    if (aBits == 16) count++;
                    if (rBits == 16) count++;
                    if (gBits == 16) count++;
                    if (bBits == 16) count++;

                    if (count >= 2)
                    {
                        format = TextureFormat.Y16X16;
                    }
                }
                else if ((ddsHeader.DdsPF.Flags & DdsInfo.DDPF_ALPHAPIXELS) != 0)
                {
                    format = TextureFormat.A8R8G8B8;
                }
                else
                {
                    format = TextureFormat.D8R8G8B8;
                    remap = TextureRemap.Order1RGB;
                }
            }
        }
        else if ((ddsHeader.DdsPF.Flags & DdsInfo.DDSF_LUMINANCE) != 0)
        {
            if (ddsHeader.DdsPF.RgbBitCount == 16)
            {
                if (rBits == 16)
                {
                    format = TextureFormat.X16;
                }
                else if ((aBits == 8 && rBits == 8) || (gBits == 8 && bBits == 8))
                {
                    format = TextureFormat.G8B8;
                }
            }
        }
        else if ((ddsHeader.DdsPF.Flags & DdsInfo.DDSF_BUMPDUDV) != 0)
        {
            if (ddsHeader.DdsPF.RgbBitCount == 16)
            {
                format = TextureFormat.Y16X16;
            }
            else if (ddsHeader.DdsPF.RgbBitCount == 32)
            {
                format = TextureFormat.A8R8G8B8;
            }
        }

        if (format == default)
        {
            (format, remap) = ddsHeader.DdsPF.RgbBitCount switch
            {
                8 => (TextureFormat.B8, remap),
                16 => (TextureFormat.X16, remap),
                32 => (TextureFormat.A8R8G8B8, remap),
                64 => (TextureFormat.W16Z16Y16X16Float, remap),
                128 => (TextureFormat.W32Z32Y32X32Float, remap),
                _ => default
            };
        }
    }

    /// <summary>
    /// Computes the texture remap of the specified DDS pixel format.
    /// </summary>
    /// <param name="ddsPF">The DDS pixel format.</param>
    /// <returns>The texture remap value.</returns>
    private static uint GetRemapFromBitMask(in DdsPixelFormat ddsPF)
    {
        uint* mask = stackalloc uint[4] { ddsPF.ABitMask, ddsPF.RBitMask, ddsPF.GBitMask, ddsPF.BBitMask };
        int* order = stackalloc int[4] { 0, 0, 0, 0 };

        // Find the biggest bitmask among each RGBA bit flag.
        // The color of the biggest one is on the leftmost side in bit order.
        if ((ddsPF.Flags & 0x01) == 0)
        {
            // No alpha is contained, so we need to figure out the pixel format by looking at the bit mask.
            // We have two cases here, 16-bit texels and 32-bit texels.
            // Alpha bits are usually put at either the leftmost or rightmost side.
            // If other RGB color bits cover the LSB, then we assume alpha bits use them.
            mask[0] = ((ddsPF.RBitMask | ddsPF.GBitMask | ddsPF.BBitMask) & 0x01) << 31;
        }

        // Compute the order of each RGBA element.
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i != j && mask[i] < mask[j])
                {
                    order[i]++;
                }
            }
        }

        uint remap = TextureRemap.MaskRRRR;

        for (int i = 0; i < 4; i++)
        {
            switch (order[i])
            {
                case 0:
                    remap |= TextureRemap.FromAlpha << (2 * i);

                    // If the "no alpha" flag is on, we set REMAP_ONE to remap.
                    if ((ddsPF.Flags & 0x01) == 0)
                    {
                        remap &= ~(0x3u << (2 * i + 8)); // Zero clear on alpha bits field.
                        remap |= TextureRemap.One << (2 * i + 8); // Put REMAP_ONE on alpha bits field.
                    }
                    break;
                case 1:
                    remap |= TextureRemap.FromRed << (2 * i);
                    break;
                case 2:
                    remap |= TextureRemap.FromGreen << (2 * i);
                    break;
                case 3:
                    remap |= TextureRemap.FromBlue << (2 * i);
                    break;
            }
        }

        return remap;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="DdsImage"/>
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged
    /// resources; false to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing) => MemoryUtils.FreeBackingMemory(ref _data, ref _handle);

    /// <summary>
    /// Releases all resources used by this <see cref="DdsImage"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
