using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace GtfDdsSharp;

/// <summary>
/// Helper methods to efficiently throw exceptions.
/// </summary>
[StackTraceHidden]
internal static class ThrowHelper
{
    /// <summary>
    /// Throws an <see cref="ArgumentException"/> indicating the DDS buffer is too small to permit conversion.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowArgument_DdsOverflowBytes()
        => throw new ArgumentException(SR.Argument_DdsOverflowBytes);

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> indicating the GTF buffer is too small to permit conversion.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowArgument_GtfOverflowBytes()
        => throw new ArgumentException(SR.Argument_GtfOverflowBytes);

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> indicating the offset and length were out of bounds for the
    /// array or count is greater than the number of elements from index to the end of the source collection.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowArgument_InvalidOffLen()
        => throw new ArgumentException(SR.Argument_InvalidOffLen);

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> indicating the specified stream is not readable.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowArgument_StreamNotReadable()
        => throw new ArgumentException(SR.Argument_StreamNotReadable);

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> indicating the specified texture could not be found.
    /// </summary>
    /// <param name="index">The texture index.</param>
    [DoesNotReturn]
    internal static T ThrowArgument_TextureNotFound<T>(int index)
        => throw new ArgumentException(string.Format(SR.Argument_TextureNotFound, index));

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> indicating
    /// the number of DDS images was not in the range [1, 255].
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRange_DdsImageCount()
        => throw new ArgumentOutOfRangeException(SR.ArgumentOutOfRange_DdsImageCount);
    
    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> indicating the enum value was out of legal range.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRange_Enum(string paramName)
        => throw new ArgumentOutOfRangeException(paramName, SR.ArgumentOutOfRange_Enum);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the file was too large.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_FileTooLong2GB()
        => throw new IOException(SR.IO_FileTooLong2GB);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the file was too large.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_FileTooLong4GB()
        => throw new IOException(SR.IO_FileTooLong4GB);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the image data extends past the end of the DDS file.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidDds_EOF()
        => throw new IOException(SR.IO_InvalidDds_EOF);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the DDS header is invalid for GTF conversion.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidDdsHeader_Convert()
        => throw new IOException(SR.IO_InvalidDdsHeader_Convert);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the header data extends past the end of the DDS file.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidDdsHeader_EOF()
        => throw new IOException(SR.IO_InvalidDdsHeader_EOF);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the DDS file has an invalid magic number.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidDdsHeader_Magic()
        => throw new IOException(SR.IO_InvalidDdsHeader_Magic);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the DDS header contains an invalid pixel format size.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidDdsHeader_PFSize()
        => throw new IOException(SR.IO_InvalidDdsHeader_PFSize);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the DDS header contains an invalid header size.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidDdsHeader_Size()
        => throw new IOException(SR.IO_InvalidDdsHeader_Size);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the GTF file size was not aligned to a 128-byte boundary.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfFile_Size()
        => throw new IOException(SR.IO_InvalidGtfFile_Size);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the GTF file header extends past the end of the file.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfHeader_EOF()
        => throw new IOException(SR.IO_InvalidGtfHeader_EOF);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the number of textures was not in the range [1, 255].
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfHeader_NumTexture()
        => throw new IOException(SR.IO_InvalidGtfHeader_NumTexture);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the GTF image size was not aligned to a 128-byte boundary.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfHeader_Size()
        => throw new IOException(SR.IO_InvalidGtfHeader_Size);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the GTF texture attributes extend past the end of the file.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfHeader_TextureEOF()
        => throw new IOException(SR.IO_InvalidGtfHeader_TextureEOF);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the texture data extends past the end of the file.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfTexture_EOF()
        => throw new IOException(SR.IO_InvalidGtfTexture_EOF);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the texture ID was not in the range [0, 255].
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfTexture_Id()
        => throw new IOException(SR.IO_InvalidGtfTexture_Id);

    /// <summary>
    /// Throws an <see cref="IOException"/> indicating the texture's offset was not aligned to a 128-byte boundary.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowIO_InvalidGtfTexture_Offset()
        => throw new IOException(SR.IO_InvalidGtfTexture_Offset);

    /// <summary>
    /// Throws a <see cref="NotSupportedException"/> indicating DX10 DDS files are not supported.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowNotSupported_DX10()
        => throw new NotSupportedException(SR.NotSupported_DX10);

    /// <summary>
    /// Throws a <see cref="NotSupportedException"/> indicating the specified texture format is not supported.
    /// </summary>
    /// <param name="format">The texture format.</param>
    [DoesNotReturn]
    internal static void ThrowNotSupported_TextureFormat(TextureFormat format)
        => throw new NotSupportedException(string.Format(SR.NotSupported_TextureFormat, format));
}
