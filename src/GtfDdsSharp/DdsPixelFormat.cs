using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents the pixel format of a DDS file.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DdsPixelFormat
{
    /// <summary>
    /// The size of the pixel format in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// Flags indicating which members contain valid data.
    /// </summary>
    public uint Flags;

    /// <summary>
    /// The FOURCC code specifying the compression or format.
    /// </summary>
    public uint FourCC;

    /// <summary>
    /// The number of bits per pixel for RGB formats.
    /// </summary>
    public uint RgbBitCount;

    /// <summary>
    /// The bit mask for the red channel.
    /// </summary>
    public uint RBitMask;

    /// <summary>
    /// The bit mask for the green channel.
    /// </summary>
    public uint GBitMask;

    /// <summary>
    /// The bit mask for the blue channel.
    /// </summary>
    public uint BBitMask;

    /// <summary>
    /// The bit mask for the alpha channel.
    /// </summary>
    public uint ABitMask;
}
