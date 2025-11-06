namespace GtfDdsSharp;

/// <summary>
/// Specifies the format of a GTF texture.
/// </summary>
[Flags]
public enum TextureFormat : byte
{
    /// <summary>
    /// Swizzled texture format.
    /// </summary>
    Swizzle = 0x00,
    /// <summary>
    /// Linear texture format.
    /// </summary>
    Linear = 0x20,
    /// <summary>
    /// Unnormalized texture format.
    /// </summary>
    Unnormalize = 0x40,
    /// <summary>
    /// 8-bit BGR texture format.
    /// </summary>
    B8 = 0x81,
    /// <summary>
    /// 16-bit ARGB1555 texture format.
    /// </summary>
    A1R5G5B5 = 0x82,
    /// <summary>
    /// 16-bit ARGB4444 texture format.
    /// </summary>
    A4R4G4B4 = 0x83,
    /// <summary>
    /// 16-bit RGB565 texture format.
    /// </summary>
    R5G6B5 = 0x84,
    /// <summary>
    /// 32-bit ARGB8888 texture format.
    /// </summary>
    A8R8G8B8 = 0x85,
    /// <summary>
    /// Compressed DXT1 texture format.
    /// </summary>
    CompressedDxt1 = 0x86,
    /// <summary>
    /// Compressed DXT2/3 texture format.
    /// </summary>
    CompressedDxt23 = 0x87,
    /// <summary>
    /// Compressed DXT4/5 texture format.
    /// </summary>
    CompressedDxt45 = 0x88,
    /// <summary>
    /// 16-bit G8B8 texture format.
    /// </summary>
    G8B8 = 0x8B,
    /// <summary>
    /// 16-bit R6G5B5 texture format.
    /// </summary>
    R6G5B5 = 0x8F,
    /// <summary>
    /// 24-bit depth with 8-bit stencil texture format.
    /// </summary>
    Depth24D8 = 0x90,
    /// <summary>
    /// 24-bit depth with 8-bit stencil float texture format.
    /// </summary>
    Depth24D8Float = 0x91,
    /// <summary>
    /// 16-bit depth texture format.
    /// </summary>
    Depth16 = 0x92,
    /// <summary>
    /// 16-bit depth float texture format.
    /// </summary>
    Depth16Float = 0x93,
    /// <summary>
    /// 16-bit X texture format.
    /// </summary>
    X16 = 0x94,
    /// <summary>
    /// 32-bit Y16_X16 texture format.
    /// </summary>
    Y16X16 = 0x95,
    /// <summary>
    /// 16-bit R5G5B5A1 texture format.
    /// </summary>
    R5G5B5A1 = 0x97,
    /// <summary>
    /// Compressed HILO8 texture format.
    /// </summary>
    CompressedHilo8 = 0x98,
    /// <summary>
    /// Compressed HILO_S8 texture format.
    /// </summary>
    CompressedHiloS8 = 0x99,
    /// <summary>
    /// 64-bit W16_Z16_Y16_X16 float texture format.
    /// </summary>
    W16Z16Y16X16Float = 0x9A,
    /// <summary>
    /// 128-bit W32_Z32_Y32_X32 float texture format.
    /// </summary>
    W32Z32Y32X32Float = 0x9B,
    /// <summary>
    /// 32-bit X float texture format.
    /// </summary>
    X32Float = 0x9C,
    /// <summary>
    /// 16-bit D1R5G5B5 texture format.
    /// </summary>
    D1R5G5B5 = 0x9D,
    /// <summary>
    /// 32-bit D8R8G8B8 texture format.
    /// </summary>
    D8R8G8B8 = 0x9E,
    /// <summary>
    /// 32-bit Y16_X16 float texture format.
    /// </summary>
    Y16X16Float = 0x9F,
    /// <summary>
    /// Compressed B8R8_G8R8 texture format.
    /// </summary>
    CompressedB8R8G8R8 = 0xAD,
    /// <summary>
    /// Compressed R8B8_R8G8 texture format.
    /// </summary>
    CompressedR8B8R8G8 = 0xAE,
    /// <summary>
    /// Raw compressed B8R8_G8R8 texture format.
    /// </summary>
    CompressedB8R8G8R8Raw = 0x8D,
    /// <summary>
    /// Raw compressed R8B8_R8G8 texture format.
    /// </summary>
    CompressedR8B8R8G8Raw = 0x8E
}
