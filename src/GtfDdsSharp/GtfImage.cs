using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Provides properties and methods to read textures from a GTF file.
/// </summary>
public unsafe class GtfImage : IReadOnlyList<GtfTexture>, IDisposable
{
    /// <summary>
    /// The default version of a GTF file header: v202.00.00.
    /// </summary>
    public const uint DefaultVersion = 0x02020000;

    private readonly uint _size;
    private readonly GtfHeader _gtfHeader;
    private readonly GtfTextureAttribute[] _textures;

    private byte* _data;
    private nint _handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfImage"/> class from the specified file.
    /// </summary>
    /// <param name="path">The path to the GTF file.</param>
    public GtfImage(string path)
        : this(ReadFile(path, out uint length), 0, length, MemoryType.Native)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfImage"/> class from the specified stream.
    /// </summary>
    /// <param name="stream">The stream containing the GTF file data.</param>
    public GtfImage(Stream stream)
        : this(ReadStream(stream, out uint length), 0, length, MemoryType.Native)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfImage"/> class from the specified array.
    /// </summary>
    /// <param name="buffer">An array of bytes containing the GTF file data.</param>
    public GtfImage(byte[] buffer)
        : this(MemoryUtils.PinMemory(buffer), 0, (uint)buffer.Length, MemoryType.Handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfImage"/> class from the specified array region.
    /// </summary>
    /// <param name="buffer">An array of bytes containing the GTF file data.</param>
    /// <param name="offset">The zero-based byte offset at which to read bytes.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    public GtfImage(byte[] buffer, int offset, int count)
        : this(MemoryUtils.PinMemory(buffer, offset, count), (uint)offset, (uint)count, MemoryType.Handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfImage"/> class from the specified region of memory.
    /// </summary>
    /// <param name="buffer">The region of memory containing the GTF file data.</param>
    public GtfImage(ReadOnlySpan<byte> buffer)
        : this(MemoryUtils.CopyMemory(buffer), 0, (uint)buffer.Length, MemoryType.Native)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfImage"/> class from the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to the GTF file data.</param>
    /// <param name="length">The maximum number of bytes to read.</param>
    public GtfImage(nint pointer, uint length)
        : this(pointer, 0, length, MemoryType.External)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GtfImage"/> class from the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to the GTF file data.</param>
    /// <param name="offset">The zero-based byte offset at which to read bytes.</param>
    /// <param name="length">The maximum number of bytes to read.</param>
    /// <param name="memoryType">Specifies the backing memory for <paramref name="pointer"/>.</param>
    private GtfImage(nint pointer, uint offset, uint length, MemoryType memoryType)
    {
        ArgumentNullException.ThrowIfNull((void*)pointer);

        // Initialize the backing memory for the file data.
        MemoryUtils.InitializeBackingMemory(pointer, offset, memoryType, out _data, out _handle);
        _size = length;

        try
        {
            // Read the header and texture data from the start of the file.
            _gtfHeader = ReadHeader();
            _textures = ReadTextureAttributes();
        }
        catch
        {
            // Dispose if an error occurs.
            Dispose(true);
            throw;
        }
    }

    /// <summary>
    /// Frees unmanaged memory in the event that the <see cref="GtfImage"/> is not properly disposed.
    /// </summary>
    ~GtfImage()
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
    /// Gets the GTF file header.
    /// </summary>
    public GtfHeader GtfHeader => _gtfHeader;

    /// <summary>
    /// Gets the GTF texture attributes.
    /// </summary>
    public GtfTextureAttribute[] Textures => _textures;

    /// <summary>
    /// Gets whether the <see cref="GtfImage"/> was disposed.
    /// </summary>
    public bool IsClosed => _data is null;

    /// <summary>
    /// Gets the number of textures.
    /// </summary>
    public int Count => _textures.Length;

    /// <summary>
    /// Gets the texture at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the texture to get.</param>
    /// <returns>The texture at the specified index.</returns>
    public GtfTexture this[int index] => new(this, index);

    /// <summary>
    /// Gets the texture attribute at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the texture attribute to get.</param>
    /// <returns>The texture attribute at the specified index.</returns>
    public GtfTextureAttribute GetTextureAttribute(int index)
    {
        GtfTextureAttribute[] textures = _textures;

        for (int i = 0; i < textures.Length; i++)
        {
            ref GtfTextureAttribute texture = ref textures[i];

            if (texture.Id == (uint)index)
            {
                return texture;
            }
        }

        return ThrowHelper.ThrowArgument_TextureNotFound<GtfTextureAttribute>(index);
    }

    /// <inheritdoc/>
    public IEnumerator<GtfTexture> GetEnumerator()
    {
        GtfTextureAttribute[] textures = _textures;

        for (int i = 0; i < textures.Length; i++)
        {
            ref GtfTextureAttribute texture = ref textures[i];
            yield return this[(int)texture.Id];
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Converts the specified GTF file to a DDS file.
    /// </summary>
    /// <param name="gtfPath">The path of the GTF file to read from.</param>
    /// <param name="index">The index of the texture to convert, or <see langword="null"/> to convert all.</param>
    public static void ConvertToDds(string gtfPath, int? index = null)
        => ConvertToDds(gtfPath, Path.ChangeExtension(gtfPath, ".dds"), index);

    /// <summary>
    /// Converts the specified GTF file to a DDS file.
    /// </summary>
    /// <param name="gtfPath">The path of the GTF file to read from.</param>
    /// <param name="ddsPath">The path to write the DDS file to.</param>
    /// <param name="index">The index of the texture to convert, or <see langword="null"/> to convert all.</param>
    public static void ConvertToDds(string gtfPath, string ddsPath, int? index = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(gtfPath);
        ArgumentException.ThrowIfNullOrEmpty(ddsPath);

        using GtfImage image = new(gtfPath);

        // For a single texture GTF file or when an index is specified, convert one texture only.
        if (image.Count == 1 || index != null)
        {
            image[index ?? 0].ConvertToDds(ddsPath);
            return;
        }

        string basePath = Path.ChangeExtension(ddsPath, null);
        string extension = Path.GetExtension(ddsPath);

        // For a packed GTF file, convert all textures.
        foreach (GtfTexture texture in image)
        {
            texture.ConvertToDds($"{basePath}_{texture.Texture.Id:D3}{extension}");
        }
    }

    /// <summary>
    /// Reads the bytes of the specified file to a block of memory.
    /// </summary>
    /// <param name="path">The file to read.</param>
    /// <param name="length">The length of the stream.</param>
    /// <returns>A pointer to the block of memory containing the file bytes.</returns>
    private static nint ReadFile(string path, out uint length)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0);
        return ReadStream(stream, out length);
    }

    /// <summary>
    /// Reads the bytes of the specified stream to a block of memory.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="length">The length of the stream.</param>
    /// <returns>A pointer to the block of memory containing the stream bytes.</returns>
    private static nint ReadStream(Stream stream, out uint length)
        => MemoryUtils.TryReadStream(stream, out length, out nint pointer)
        ? pointer
        : ReadUnseekableStream(stream, out length);

    /// <summary>
    /// Reads the bytes of the specified unseekable stream to a block of memory.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="length">The length of the stream.</param>
    /// <returns>A pointer to the block of memory containing the stream bytes.</returns>
    private static nint ReadUnseekableStream(Stream stream, out uint length)
    {
        // Allocate a block of memory to store the file header.
        byte* pointer = (byte*)NativeMemory.Alloc((uint)sizeof(GtfHeader));

        try
        {
            // Read the file header.
            stream.ReadExactly(new Span<byte>(pointer, sizeof(GtfHeader)));
            GtfHeader gtfHeader = ReadHeader(pointer);

            // Compute the file size.
            uint gtfImageSize = gtfHeader.GetFileSize() - (uint)sizeof(GtfHeader);
            length = (uint)sizeof(GtfHeader) + gtfImageSize;

            // Reallocate the block of memory to store the texture data.
            pointer = (byte*)NativeMemory.Realloc(pointer, length);

            // Read the texture data.
            foreach (Span<byte> buffer in new PointerSpanEnumerator(pointer + sizeof(GtfHeader), gtfImageSize))
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
    /// Reads the header from the start of the GTF file.
    /// </summary>
    /// <returns>The GTF file header.</returns>
    private GtfHeader ReadHeader()
    {
        if (_size % GtfTexture.Alignment != 0) ThrowHelper.ThrowIO_InvalidGtfFile_Size();
        if (_size < (uint)sizeof(GtfHeader)) ThrowHelper.ThrowIO_InvalidGtfHeader_EOF();

        GtfHeader gtfHeader = ReadHeader(_data);

        if (_size < gtfHeader.GetHeaderSize()) ThrowHelper.ThrowIO_InvalidGtfHeader_TextureEOF();

        return gtfHeader;
    }

    /// <summary>
    /// Reads a GTF file header from the specified memory address.
    /// </summary>
    /// <param name="pointer">An unmanaged pointer containing the address to read from.</param>
    /// <returns>The GTF file header.</returns>
    private static GtfHeader ReadHeader(byte* pointer)
    {
        GtfHeader gtfHeader = Unsafe.ReadUnaligned<GtfHeader>(pointer);

        // Read the header in big-endian format.
        if (BitConverter.IsLittleEndian)
        {
            gtfHeader.ReverseEndianness();
        }

        if (gtfHeader.NumTexture is < 1 or > byte.MaxValue) ThrowHelper.ThrowIO_InvalidGtfHeader_NumTexture();
        if (gtfHeader.Size % GtfTexture.Alignment != 0) ThrowHelper.ThrowIO_InvalidGtfHeader_Size();

        return gtfHeader;
    }

    /// <summary>
    /// Reads the texture attributes from the start of the GTF file, following the header.
    /// </summary>
    /// <returns>An array of the texture attributes.</returns>
    private GtfTextureAttribute[] ReadTextureAttributes()
    {
        GtfTextureAttribute[] textures = new GtfTextureAttribute[_gtfHeader.NumTexture];
        GtfTextureAttribute* gtfAttr = (GtfTextureAttribute*)(_data + sizeof(GtfHeader));

        for (int i = 0; i < textures.Length; i++)
        {
            ref GtfTextureAttribute texture = ref textures[i];
            texture = Unsafe.ReadUnaligned<GtfTextureAttribute>(gtfAttr++);

            // Read the texture attribute in big-endian format.
            if (BitConverter.IsLittleEndian)
            {
                texture.ReverseEndianness();
            }

            if (texture.Id > byte.MaxValue) ThrowHelper.ThrowIO_InvalidGtfTexture_Id();
            if (texture.OffsetToTex % GtfTexture.Alignment != 0) ThrowHelper.ThrowIO_InvalidGtfTexture_Offset();
            if (_size < (ulong)texture.OffsetToTex + texture.TextureSize) ThrowHelper.ThrowIO_InvalidGtfTexture_EOF();
        }

        return textures;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="GtfImage"/>
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged
    /// resources; false to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing) => MemoryUtils.FreeBackingMemory(ref _data, ref _handle);

    /// <summary>
    /// Releases all resources used by this <see cref="GtfImage"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
