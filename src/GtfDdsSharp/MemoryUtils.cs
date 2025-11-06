using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Helper methods to perform low-level memory operations.
/// </summary>
public static unsafe class MemoryUtils
{
    /// <summary>
    /// Rounds up the specified size to the given alignment.
    /// </summary>
    /// <param name="size">The initial size to be aligned.</param>
    /// <param name="alignment">The boundary to align to.</param>
    /// <returns>The aligned size, which is a multiple of <paramref name="alignment"/>.</returns>
    public static uint GetAlignment(uint size, uint alignment) => (size + alignment - 1) & ~(alignment - 1);

    /// <summary>
    /// Rounds up the specified size to be aligned to a GTF texture boundary.
    /// </summary>
    /// <param name="size">The initial size to be aligned.</param>
    /// <returns>The aligned size, which is a multiple of <see cref="GtfTexture.Alignment"/>.</returns>
    public static uint GetGtfAlignment(uint size) => GetAlignment(size, GtfTexture.Alignment);

    /// <summary>
    /// Computes the number of mipmap levels based on the specified dimensions.
    /// </summary>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <param name="depth">The depth of the texture.</param>
    /// <returns>The total number of mipmap levels for the texture.</returns>
    public static uint GetMipmapSize(uint width, uint height, uint depth)
    {
        uint log2Width = (uint)BitOperations.Log2(width);
        uint log2Height = (uint)BitOperations.Log2(height);
        uint log2Depth = (uint)BitOperations.Log2(depth);
        return Math.Max(Math.Max(log2Width, log2Height), log2Depth) + 1;
    }

    /// <summary>
    /// Copies bytes from the source address to the destination address.
    /// </summary>
    /// <param name="dest">The unmanaged pointer corresponding to the destination address to copy to.</param>
    /// <param name="src">The unmanaged pointer corresponding to the source address to copy from.</param>
    /// <param name="size">The number of bytes to copy.</param>
    /// <param name="swap">Whether to swap the source address and destination address.</param>
    public static void MoveMemory(void* dest, void* src, uint size, bool swap)
    {
        if (swap)
        {
            Unsafe.CopyBlockUnaligned(src, dest, size);
        }
        else
        {
            Unsafe.CopyBlockUnaligned(dest, src, size);
        }
    }

    /// <summary>
    /// Copies bytes from the source address to the destination address in reversed 2-byte chunks.
    /// </summary>
    /// <param name="dest">The unmanaged pointer corresponding to the destination address to copy to.</param>
    /// <param name="src">The unmanaged pointer corresponding to the source address to copy from.</param>
    /// <param name="size">The number of bytes to copy.</param>
    /// <param name="swap">Whether to swap the source address and destination address.</param>
    public static void MoveMemoryWithInvertEndian16(void* dest, void* src, uint size, bool swap)
    {
        if (swap)
        {
            MoveMemoryWithInvertEndian16((ushort*)src, (ushort*)dest, size);
        }
        else
        {
            MoveMemoryWithInvertEndian16((ushort*)dest, (ushort*)src, size);
        }
    }

    /// <inheritdoc cref="MoveMemoryWithInvertEndian16(void*, void*, uint, bool)"/>
    public static void MoveMemoryWithInvertEndian16(ushort* dest, ushort* src, uint size)
    {
        for (uint i = 0; i < size / 2; i++)
        {
            Unsafe.WriteUnaligned(dest++, BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ushort>(src++)));
        }

        if (size % 2 != 0)
        {
            Unsafe.CopyBlockUnaligned(dest, src, size % 2);
        }
    }

    /// <summary>
    /// Copies bytes from the source address to the destination address in reversed 4-byte chunks.
    /// </summary>
    /// <param name="dest">The unmanaged pointer corresponding to the destination address to copy to.</param>
    /// <param name="src">The unmanaged pointer corresponding to the source address to copy from.</param>
    /// <param name="size">The number of bytes to copy.</param>
    /// <param name="swap">Whether to swap the source address and destination address.</param>
    public static void MoveMemoryWithInvertEndian32(void* dest, void* src, uint size, bool swap)
    {
        if (swap)
        {
            MoveMemoryWithInvertEndian32((uint*)src, (uint*)dest, size);
        }
        else
        {
            MoveMemoryWithInvertEndian32((uint*)dest, (uint*)src, size);
        }
    }

    /// <inheritdoc cref="MoveMemoryWithInvertEndian32(void*, void*, uint, bool)"/>
    public static void MoveMemoryWithInvertEndian32(uint* dest, uint* src, uint size)
    {
        for (uint i = 0; i < size / 4; i++)
        {
            Unsafe.WriteUnaligned(dest++, BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<uint>(src++)));
        }

        if (size % 4 != 0)
        {
            Unsafe.CopyBlockUnaligned(dest, src, size % 4);
        }
    }

    /// <summary>
    /// Reverses a primitive value by performing an endianness swap
    /// of the specified <see cref="DdsHeader"/> value.
    /// </summary>
    /// <param name="ddsHeader">A reference to the value to reverse.</param>
    public static unsafe void ReverseEndianness(this ref DdsHeader ddsHeader)
    {
        ddsHeader.Magic = BinaryPrimitives.ReverseEndianness(ddsHeader.Magic);
        ddsHeader.Size = BinaryPrimitives.ReverseEndianness(ddsHeader.Size);
        ddsHeader.Flags = BinaryPrimitives.ReverseEndianness(ddsHeader.Flags);
        ddsHeader.Height = BinaryPrimitives.ReverseEndianness(ddsHeader.Height);
        ddsHeader.Width = BinaryPrimitives.ReverseEndianness(ddsHeader.Width);
        ddsHeader.PitchOrLinearSize = BinaryPrimitives.ReverseEndianness(ddsHeader.PitchOrLinearSize);
        ddsHeader.Depth = BinaryPrimitives.ReverseEndianness(ddsHeader.Depth);
        ddsHeader.MipmapCount = BinaryPrimitives.ReverseEndianness(ddsHeader.MipmapCount);

        fixed (uint* reserved1 = ddsHeader.Reserved1)
        {
            for (int i = 0; i < 11; i++)
            {
                reserved1[i] = BinaryPrimitives.ReverseEndianness(reserved1[i]);
            }
        }

        ddsHeader.DdsPF.ReverseEndianness();
        ddsHeader.Caps1 = BinaryPrimitives.ReverseEndianness(ddsHeader.Caps1);
        ddsHeader.Caps2 = BinaryPrimitives.ReverseEndianness(ddsHeader.Caps2);

        fixed (uint* reserved2 = ddsHeader.Reserved2)
        {
            for (int i = 0; i < 3; i++)
            {
                reserved2[i] = BinaryPrimitives.ReverseEndianness(reserved2[i]);
            }
        }
    }

    /// <summary>
    /// Reverses a primitive value by performing an endianness swap
    /// of the specified <see cref="DdsPixelFormat"/> value.
    /// </summary>
    /// <param name="ddsPF">A reference to the value to reverse.</param>
    public static unsafe void ReverseEndianness(this ref DdsPixelFormat ddsPF)
    {
        ddsPF.Size = BinaryPrimitives.ReverseEndianness(ddsPF.Size);
        ddsPF.Flags = BinaryPrimitives.ReverseEndianness(ddsPF.Flags);
        ddsPF.FourCC = BinaryPrimitives.ReverseEndianness(ddsPF.FourCC);
        ddsPF.RgbBitCount = BinaryPrimitives.ReverseEndianness(ddsPF.RgbBitCount);
        ddsPF.RBitMask = BinaryPrimitives.ReverseEndianness(ddsPF.RBitMask);
        ddsPF.GBitMask = BinaryPrimitives.ReverseEndianness(ddsPF.GBitMask);
        ddsPF.BBitMask = BinaryPrimitives.ReverseEndianness(ddsPF.BBitMask);
        ddsPF.ABitMask = BinaryPrimitives.ReverseEndianness(ddsPF.ABitMask);
    }

    /// <summary>
    /// Reverses a primitive value by performing an endianness swap
    /// of the specified <see cref="GtfHeader"/> value.
    /// </summary>
    /// <param name="gtfHeader">A reference to the value to reverse.</param>
    public static void ReverseEndianness(this ref GtfHeader gtfHeader)
    {
        gtfHeader.Version = BinaryPrimitives.ReverseEndianness(gtfHeader.Version);
        gtfHeader.Size = BinaryPrimitives.ReverseEndianness(gtfHeader.Size);
        gtfHeader.NumTexture = BinaryPrimitives.ReverseEndianness(gtfHeader.NumTexture);
    }

    /// <summary>
    /// Reverses a primitive value by performing an endianness swap
    /// of the specified <see cref="GtfTextureAttribute"/> value.
    /// </summary>
    /// <param name="texture">A reference to the value to reverse.</param>
    public static void ReverseEndianness(this ref GtfTextureAttribute texture)
    {
        texture.Id = BinaryPrimitives.ReverseEndianness(texture.Id);
        texture.OffsetToTex = BinaryPrimitives.ReverseEndianness(texture.OffsetToTex);
        texture.TextureSize = BinaryPrimitives.ReverseEndianness(texture.TextureSize);
        texture.Info.ReverseEndianness();
    }

    /// <summary>
    /// Reverses a primitive value by performing an endianness swap
    /// of the specified <see cref="GtfTextureInfo"/> value.
    /// </summary>
    /// <param name="texture">A reference to the value to reverse.</param>
    public static void ReverseEndianness(this ref GtfTextureInfo texture)
    {
        texture.Remap = BinaryPrimitives.ReverseEndianness(texture.Remap);
        texture.Width = BinaryPrimitives.ReverseEndianness(texture.Width);
        texture.Height = BinaryPrimitives.ReverseEndianness(texture.Height);
        texture.Depth = BinaryPrimitives.ReverseEndianness(texture.Depth);
        texture.Pitch = BinaryPrimitives.ReverseEndianness(texture.Pitch);
        texture.Offset = BinaryPrimitives.ReverseEndianness(texture.Offset);
    }

    /// <summary>
    /// Computes the swizzle offset from the specified coordinates and base-2 logarithms of each dimension.
    /// </summary>
    /// <param name="x">The x-coordinate, relative to the width.</param>
    /// <param name="y">The y-coordinate, relative to the height.</param>
    /// <param name="z">The z-coordinate, relative to the depth.</param>
    /// <param name="log2Width">The base-2 logarithm of the width.</param>
    /// <param name="log2Height">The base-2 logarithm of the height.</param>
    /// <param name="log2Depth">The base-2 logarithm of the depth.</param>
    /// <returns>The swizzle offset.</returns>
    public static uint ToSwizzle(uint x, uint y, uint z, uint log2Width, uint log2Height, uint log2Depth)
        => log2Depth == 0
        ? ToSwizzle2D(x, y, log2Width, log2Height)
        : ToSwizzle3D(x, y, z, log2Width, log2Height, log2Depth);

    /// <inheritdoc cref="ToSwizzle(uint, uint, uint, uint, uint, uint)"/>
    public static uint ToSwizzle2D(uint x, uint y, uint log2Width, uint log2Height)
    {
        uint offset = 0;
        uint t = 0;

        while (log2Width != 0 || log2Height != 0)
        {
            if (log2Width != 0)
            {
                offset |= (x & 0x01) << (int)t;
                x >>= 1;
                t++;
                log2Width--;
            }
            if (log2Height != 0)
            {
                offset |= (y & 0x01) << (int)t;
                y >>= 1;
                t++;
                log2Height--;
            }
        }

        return offset;
    }

    /// <inheritdoc cref="ToSwizzle(uint, uint, uint, uint, uint, uint)"/>
    public static uint ToSwizzle3D(uint x, uint y, uint z, uint log2Width, uint log2Height, uint log2Depth)
    {
        uint offset = 0;
        uint t = 0;

        while (log2Width != 0 || log2Height != 0 || log2Depth != 0)
        {
            if (log2Width != 0)
            {
                offset |= (x & 0x01) << (int)t;
                x >>= 1;
                t++;
                log2Width--;
            }
            if (log2Height != 0)
            {
                offset |= (y & 0x01) << (int)t;
                y >>= 1;
                t++;
                log2Height--;
            }
            if (log2Depth != 0)
            {
                offset |= (z & 0x01) << (int)t;
                z >>= 1;
                t++;
                log2Depth--;
            }
        }

        return offset;
    }

    /// <summary>
    /// Copies the bytes of the specified region of memory to a block of memory.
    /// </summary>
    /// <param name="buffer">The region of memory to copy.</param>
    /// <returns>A pointer to the block of memory containing the span bytes.</returns>
    internal static nint CopyMemory(ReadOnlySpan<byte> buffer)
    {
        void* pointer = NativeMemory.Alloc((uint)buffer.Length);

        try
        {
            buffer.CopyTo(new Span<byte>(pointer, buffer.Length));
            return (nint)pointer;
        }
        catch
        {
            NativeMemory.Free(pointer);
            throw;
        }
    }

    /// <summary>
    /// Frees the backing memory for the specified pointers.
    /// </summary>
    /// <param name="data">A pointer to the backing memory's data.</param>
    /// <param name="handle">A pointer to the backing memory's handle.</param>
    internal static void FreeBackingMemory(ref byte* data, ref nint handle)
    {
        if (data is null) return;

        if (data == (void*)handle)
        {
            NativeMemory.Free(data);
        }
        else if (handle != 0)
        {
            Unsafe.As<nint, GCHandle>(ref handle).Free();
        }

        data = null;
    }

    /// <summary>
    /// Initializes the backing memory for the specified pointer based on the given memory type.
    /// </summary>
    /// <param name="pointer">A pointer to the memory.</param>
    /// <param name="offset">The zero-based byte offset at which the data pointer should begin.</param>
    /// <param name="memoryType">The type of backing memory.</param>
    /// <param name="data">A pointer to the backing memory's data.</param>
    /// <param name="handle">A pointer to the backing memory's handle.</param>
    internal static void InitializeBackingMemory(nint pointer,
                                                 uint offset,
                                                 MemoryType memoryType,
                                                 out byte* data,
                                                 out nint handle)
    {
        switch (memoryType)
        {
            case MemoryType.Native:
                data = (byte*)pointer;
                handle = pointer;
                break;
            case MemoryType.Handle:
                data = (byte*)Unsafe.As<nint, GCHandle>(ref pointer).AddrOfPinnedObject() + offset;
                handle = pointer;
                break;
            case MemoryType.External:
                data = (byte*)pointer;
                handle = 0;
                break;
            default:
                ThrowHelper.ThrowArgumentOutOfRange_Enum(nameof(memoryType));
                data = null;
                handle = 0;
                break;
        }
    }

    /// <summary>
    /// Allocates a pinned <see cref="GCHandle"/> for the specified array.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, it will be pinned to the handle.</param>
    /// <returns>A new <see cref="GCHandle"/> pinning the specified array.</returns>
    internal static nint PinMemory(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        return (nint)GCHandle.Alloc(buffer, GCHandleType.Pinned);
    }

    /// <summary>
    /// Allocates a pinned <see cref="GCHandle"/> for the specified region of memory.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, it will be pinned to the handle.</param>
    /// <param name="offset">The zero-based byte offset at which to read bytes.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <returns>A new <see cref="GCHandle"/> pinning the specified array.</returns>
    internal static nint PinMemory(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        if ((uint)count > buffer.Length - offset) ThrowHelper.ThrowArgument_InvalidOffLen();

        return (nint)GCHandle.Alloc(buffer, GCHandleType.Pinned);
    }

    /// <summary>
    /// Tries to read the bytes of the specified stream to a block of memory.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="length">The length of the stream.</param>
    /// <param name="pointer">A pointer to the block of memory containing the stream bytes.</param>
    /// <returns>
    /// <see langword="true"/> if the stream was seekable and could be read; otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool TryReadStream(Stream stream, out uint length, out nint pointer)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) ThrowHelper.ThrowArgument_StreamNotReadable();
        if (!stream.CanSeek) goto Fail;

        long streamBytes = stream.Length - stream.Position;

        if (streamBytes < 0) goto Fail;
        if (streamBytes > uint.MaxValue) ThrowHelper.ThrowIO_FileTooLong4GB();

        length = (uint)streamBytes;
        pointer = (nint)NativeMemory.Alloc(length);

        try
        {
            foreach (Span<byte> buffer in new PointerSpanEnumerator(pointer, length))
            {
                stream.ReadExactly(buffer);
            }

            return true;
        }
        catch
        {
            NativeMemory.Free((void*)pointer);
            throw;
        }

    Fail:
        length = 0;
        pointer = 0;
        return false;
    }
}
