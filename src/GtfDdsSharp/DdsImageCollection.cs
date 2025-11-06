using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents a collection of DDS images. Provides properties and methods related to GTF file conversion.
/// </summary>
public unsafe class DdsImageCollection : IReadOnlyList<DdsImage>, IDisposable
{
    private readonly DdsImage[] _images;
    private readonly GtfTextureAttribute[] _textures;
    private readonly GtfHeader _gtfHeader;
    private readonly uint _gtfFileSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImageCollection"/> class from the specified DDS file paths.
    /// </summary>
    /// <param name="images">The DDS file paths.</param>
    /// <param name="options">Specifies options for DDS to GTF conversion.</param>
    public DdsImageCollection(IEnumerable<string> images, ConvertOptions options = default)
        : this(images.Select(x => new DdsImage(x, options)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DdsImageCollection"/> class from the specified DDS images.
    /// </summary>
    /// <param name="images">The DDS images.</param>
    public DdsImageCollection(params IEnumerable<DdsImage> images)
    {
        _images = [.. images];

        if (_images.Length is < 1 or > byte.MaxValue) ThrowHelper.ThrowArgumentOutOfRange_DdsImageCount();

        // Create a GTF texture attribute for each DDS image.
        _textures = new GtfTextureAttribute[_images.Length];
        ref GtfTextureAttribute texture = ref Unsafe.NullRef<GtfTextureAttribute>();
        uint gtfHeaderSize = GtfUtils.GetHeaderSize((uint)_images.Length);
        uint id = 0;

        foreach (DdsImage image in _images)
        {
            ObjectDisposedException.ThrowIf(image.IsClosed, image);

            texture = ref _textures[id];
            texture.Id = id;

            if (id == 0)
            {
                texture.OffsetToTex = gtfHeaderSize;
            }
            else
            {
                ref GtfTextureAttribute prevTex = ref Unsafe.Subtract(ref texture, 1);
                texture.OffsetToTex = MemoryUtils.GetGtfAlignment(prevTex.OffsetToTex + prevTex.TextureSize);
            }

            texture.TextureSize = image.GtfImageSize;
            texture.Info = image.Texture;
            id++;
        }

        // Compute the GTF file size based on the final texture offset.
        _gtfFileSize = MemoryUtils.GetGtfAlignment(texture.OffsetToTex + texture.TextureSize);

        // Initialize the GTF file header.
        _gtfHeader.Version = GtfImage.DefaultVersion;
        _gtfHeader.Size = _gtfFileSize - gtfHeaderSize;
        _gtfHeader.NumTexture = (uint)_images.Length;
    }

    /// <summary>
    /// The DDS images in this collection.
    /// </summary>
    public DdsImage[] Images => _images;

    /// <summary>
    /// The GTF textures corresponding to each DDS image.
    /// </summary>
    public GtfTextureAttribute[] Textures => _textures;

    /// <summary>
    /// The GTF file header used for conversion.
    /// </summary>
    public GtfHeader GtfHeader => _gtfHeader;

    /// <summary>
    /// Gets the size of the GTF image produced by converting the DDS images.
    /// </summary>
    public uint GtfImageSize => _gtfHeader.Size;

    /// <summary>
    /// The size of the GTF file produced by converting the DDS images.
    /// </summary>
    public uint GtfFileSize => _gtfFileSize;

    /// <inheritdoc/>
    public DdsImage this[int index] => _images[index];

    /// <inheritdoc/>
    public int Count => _images.Length;

    /// <summary>
    /// Converts the DDS images to a GTF file and writes it to a byte array.
    /// </summary>
    /// <returns>A byte array containing the GTF file.</returns>
    public byte[] ConvertToGtf()
    {
        if (_gtfFileSize > (uint)Array.MaxLength) ThrowHelper.ThrowIO_FileTooLong2GB();

        byte[] bytes = new byte[_gtfFileSize];
        ConvertToGtf(bytes);
        return bytes;
    }

    /// <summary>
    /// Converts the DDS images to a packed GTF file and writes it to the specified file.
    /// </summary>
    /// <param name="path">The file to write the GTF file to.</param>
    public void ConvertToGtf(string path)
    {
        using FileStream stream = new(path, new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Share = FileShare.Read,
            PreallocationSize = _gtfFileSize,
            BufferSize = 0
        });
        ConvertToGtf(stream);
    }

    /// <summary>
    /// Converts the DDS images to a packed GTF file and writes it to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write the GTF file to.</param>
    public void ConvertToGtf(Stream stream)
    {
        uint length = _gtfFileSize;
        byte* pointer = (byte*)NativeMemory.Alloc(length);

        try
        {
            ConvertToGtf(pointer, length);

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
    /// Converts the DDS images to a packed GTF file and writes it to the specified array.
    /// </summary>
    /// <param name="buffer">The array to write the GTF file to.</param>
    public void ConvertToGtf(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        ConvertToGtf(buffer.AsSpan());
    }

    /// <summary>
    /// Converts the DDS images to a packed GTF file and writes it to the specified array region.
    /// </summary>
    /// <param name="buffer">The array to write the GTF file to.</param>
    /// <param name="offset">The zero-based byte offset at which to write bytes.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    public void ConvertToGtf(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        ConvertToGtf(buffer.AsSpan(offset, count));
    }

    /// <summary>
    /// Converts the DDS images to a packed GTF file and writes it to the specified region of memory.
    /// </summary>
    /// <param name="buffer">The region of memory to write the GTF file to.</param>
    public void ConvertToGtf(Span<byte> buffer)
    {
        fixed (byte* pointer = buffer)
        {
            ConvertToGtf(pointer, (uint)buffer.Length);
        }
    }

    /// <summary>
    /// Converts the DDS images to a packed GTF file and writes it to the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to write the GTF file to.</param>
    /// <param name="length">The maximum number of bytes to write.</param>
    public void ConvertToGtf(nint pointer, uint length) => ConvertToGtf((byte*)pointer, length);

    /// <summary>
    /// Converts the DDS images to a packed GTF file and writes it to the specified memory address.
    /// </summary>
    /// <param name="pointer">The pointer to write the GTF file to.</param>
    /// <param name="length">The maximum number of bytes to write.</param>
    public void ConvertToGtf(byte* pointer, uint length)
    {
        // Initialize the GTF file pointer.
        Unsafe.InitBlockUnaligned(pointer, 0, length);
        Span<GtfTextureAttribute> textures = _textures;
        DdsImage[] images = _images;

        // Convert each image from DDS to GTF.
        for (int i = 0; i < images.Length; i++)
        {
            DdsImage image = images[i];

            ObjectDisposedException.ThrowIf(image.IsClosed, image);

            uint offsetToTex = textures[i].OffsetToTex;
            byte* gtfImage = pointer + offsetToTex;
            byte* ddsImage = (byte*)image.Data + sizeof(DdsHeader);
            TextureConversion.ConvertBufferByLayout(gtfImage,
                                                    length - offsetToTex,
                                                    ddsImage,
                                                    image.Size - (uint)sizeof(DdsHeader),
                                                    image.Layouts,
                                                    image.Texture,
                                                    false);
        }

        GtfHeader gtfHeader = _gtfHeader;
        scoped Span<GtfTextureAttribute> gtfAttr;

        // Convert the header data to big-endian format, if necessary.
        if (BitConverter.IsLittleEndian)
        {
            gtfHeader.ReverseEndianness();
            gtfAttr = stackalloc GtfTextureAttribute[textures.Length];
            textures.CopyTo(gtfAttr);

            foreach (ref GtfTextureAttribute texture in gtfAttr)
            {
                texture.ReverseEndianness();
            }
        }
        else
        {
            gtfAttr = textures;
        }

        // Write the file header and texture attributes at the start of the GTF file.
        Unsafe.WriteUnaligned(pointer, gtfHeader);
        gtfAttr.CopyTo(new Span<GtfTextureAttribute>(pointer + sizeof(GtfHeader), gtfAttr.Length));
    }

    /// <inheritdoc/>
    public IEnumerator<DdsImage> GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _images.GetEnumerator();

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="DdsImageCollection"/>
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged
    /// resources; false to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (DdsImage image in _images)
            {
                image.Dispose();
            }
        }
    }

    /// <summary>
    /// Releases all resources used by this <see cref="DdsImageCollection"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
