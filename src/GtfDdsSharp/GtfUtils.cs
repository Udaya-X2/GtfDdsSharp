using System.Runtime.CompilerServices;

namespace GtfDdsSharp;

/// <summary>
/// Provides extension methods for the various GTF types.
/// </summary>
public static class GtfUtils
{
    /// <summary>
    /// Computes the size of the GTF file in bytes.
    /// </summary>
    /// <param name="gtfHeader">The GTF file header.</param>
    /// <returns>The size of the GTF file in bytes.</returns>
    public static uint GetFileSize(this GtfHeader gtfHeader) => gtfHeader.GetHeaderSize() + gtfHeader.Size;

    /// <summary>
    /// Computes the size of the GTF file's header, including texture attributes, in bytes.
    /// </summary>
    /// <param name="gtfHeader">The GTF file header.</param>
    /// <returns>The size of the GTF file's header, including texture attributes, in bytes.</returns>
    public static uint GetHeaderSize(this GtfHeader gtfHeader) => GetHeaderSize(gtfHeader.NumTexture);

    /// <summary>
    /// Computes the size of the GTF file's header, including texture attributes, in bytes.
    /// </summary>
    /// <param name="numTexture">The number of textures.</param>
    /// <returns>The size of the GTF file's header, including texture attributes, in bytes.</returns>
    public static unsafe uint GetHeaderSize(uint numTexture)
        => MemoryUtils.GetGtfAlignment((uint)sizeof(GtfHeader) + (uint)sizeof(GtfTextureAttribute) * numTexture);

    /// <summary>
    /// Returns the texture's format without any flags.
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <returns>The texture's format without any flags.</returns>
    public static TextureFormat GetRawFormat(this in GtfTextureInfo texture) => texture.Format.GetRawFormat();

    /// <summary>
    /// Returns the texture format without any flags.
    /// </summary>
    /// <param name="format">The texture format.</param>
    /// <returns>The texture format without any flags.</returns>
    public static TextureFormat GetRawFormat(this TextureFormat format)
        => format & ~(TextureFormat.Linear | TextureFormat.Unnormalize);

    /// <summary>
    /// Returns whether the texture uses a swizzled memory layout.
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <returns>Whether the texture uses a swizzled memory layout.</returns>
    public static bool IsSwizzle(this in GtfTextureInfo texture) => texture.Format.IsSwizzle();

    /// <summary>
    /// Returns whether the texture uses a swizzled memory layout.
    /// </summary>
    /// <param name="format">The texture format.</param>
    /// <returns>Whether the texture uses a swizzled memory layout.</returns>
    public static bool IsSwizzle(this TextureFormat format) => (format & TextureFormat.Linear) == 0;

    /// <summary>
    /// Returns whether the texture can use a swizzled memory layout.
    /// </summary>
    /// <param name="tex">The texture.</param>
    /// <returns>Whether the texture can use a swizzled memory layout.</returns>
    public static bool IsSwizzlable(this in GtfTextureInfo tex)
        => !tex.GetRawFormat().IsRawCompressed()
        && IsPow2OrZero(tex.Width)
        && IsPow2OrZero(tex.Height)
        && IsPow2OrZero(tex.Depth);

    /// <summary>
    /// Computes the pitch from the specified texture and width.
    /// </summary>
    /// <param name="rawFormat">The raw texture format.</param>
    /// <param name="width">The texture's width.</param>
    /// <returns>The pitch.</returns>
    public static uint GetPitch(this TextureFormat rawFormat, uint width) => rawFormat switch
    {
        TextureFormat.CompressedDxt1 => (width + 3) / 4 * 8,
        TextureFormat.CompressedDxt23 => (width + 3) / 4 * 16,
        TextureFormat.CompressedDxt45 => (width + 3) / 4 * 16,
        TextureFormat.CompressedB8R8G8R8Raw => (width + 1) / 2 * 4,
        TextureFormat.CompressedR8B8R8G8Raw => (width + 1) / 2 * 4,
        _ => width * rawFormat.GetDepth(),
    };

    /// <summary>
    /// Returns the depth of the specified texture.
    /// </summary>
    /// <param name="rawFormat">The raw texture format.</param>
    /// <returns>The depth.</returns>
    public static ushort GetDepth(this TextureFormat rawFormat) => rawFormat switch
    {
        TextureFormat.B8 => 1,
        TextureFormat.A1R5G5B5 => 2,
        TextureFormat.A4R4G4B4 => 2,
        TextureFormat.R5G6B5 => 2,
        TextureFormat.G8B8 => 2,
        TextureFormat.R6G5B5 => 2,
        TextureFormat.Depth16 => 2,
        TextureFormat.Depth16Float => 2,
        TextureFormat.X16 => 2,
        TextureFormat.D1R5G5B5 => 2,
        TextureFormat.R5G5B5A1 => 2,
        TextureFormat.CompressedHilo8 => 2,
        TextureFormat.CompressedHiloS8 => 2,
        TextureFormat.CompressedB8R8G8R8Raw => 2,
        TextureFormat.CompressedR8B8R8G8Raw => 2,
        TextureFormat.A8R8G8B8 => 4,
        TextureFormat.Depth24D8 => 4,
        TextureFormat.Depth24D8Float => 4,
        TextureFormat.Y16X16 => 4,
        TextureFormat.X32Float => 4,
        TextureFormat.D8R8G8B8 => 4,
        TextureFormat.Y16X16Float => 4,
        TextureFormat.W16Z16Y16X16Float => 8,
        TextureFormat.W32Z32Y32X32Float => 16,
        TextureFormat.CompressedDxt1 => 8,
        TextureFormat.CompressedDxt23 => 16,
        TextureFormat.CompressedDxt45 => 16,
        _ => 4,
    };

    /// <summary>
    /// Returns whether the texture is DXT compressed.
    /// </summary>
    /// <param name="rawFormat">The raw texture format.</param>
    /// <returns>Whether the texture is DXT compressed.</returns>
    public static bool IsDxtn(this TextureFormat rawFormat)
        => rawFormat is TextureFormat.CompressedDxt1 or TextureFormat.CompressedDxt23 or TextureFormat.CompressedDxt45;

    /// <summary>
    /// Returns whether the texture is raw compressed.
    /// </summary>
    /// <param name="rawFormat">The raw texture format.</param>
    /// <returns>Whether the texture is raw compressed.</returns>
    public static bool IsRawCompressed(this TextureFormat rawFormat)
        => rawFormat is TextureFormat.CompressedB8R8G8R8Raw or TextureFormat.CompressedR8B8R8G8Raw;

    /// <summary>
    /// Returns a value describing how to invert memory when converting the specified texture.
    /// </summary>
    /// <param name="rawFormat">The raw texture format.</param>
    /// <returns>A value describing how to invert memory when converting the specified texture.</returns>
    public static InvertFlag GetInvertFlag(this TextureFormat rawFormat) => rawFormat switch
    {
        TextureFormat.CompressedB8R8G8R8Raw => InvertFlag.InvertEndian32Even,
        TextureFormat.CompressedR8B8R8G8Raw => InvertFlag.InvertEndian32Even,
        TextureFormat.W32Z32Y32X32Float => InvertFlag.InvertEndian32,
        TextureFormat.X32Float => InvertFlag.InvertEndian32,
        TextureFormat.X16 => InvertFlag.InvertEndian16,
        TextureFormat.Y16X16 => InvertFlag.InvertEndian16,
        TextureFormat.Y16X16Float => InvertFlag.InvertEndian16,
        TextureFormat.W16Z16Y16X16Float => InvertFlag.InvertEndian16,
        _ => rawFormat.GetDepth() switch
        {
            2 => InvertFlag.InvertEndian16,
            4 => InvertFlag.InvertEndian32,
            _ => rawFormat.IsDxtn() switch
            {
                false => InvertFlag.InvertEndian32,
                true => InvertFlag.None
            }
        }
    };

    /// <summary>
    /// Evaluates whether the specified <see langword="uint"/> value is a power of two or zero.
    /// </summary>
    /// <param name="value">An unsigned integer value.</param>
    /// <returns>
    /// <see langword="true"/> if the specified value is a power of two or zero; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPow2OrZero(uint value) => (value & (value - 1)) == 0;
}
