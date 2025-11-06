using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents the header of a DDS file.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct DdsHeader
{
    /// <summary>
    /// The magic number identifying a DDS file.
    /// </summary>
    public uint Magic;

    /// <summary>
    /// The size of the DDS header in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// Flags indicating which members contain valid data.
    /// </summary>
    public uint Flags;

    /// <summary>
    /// The height of the DDS texture in pixels.
    /// </summary>
    public uint Height;

    /// <summary>
    /// The width of the DDS texture in pixels.
    /// </summary>
    public uint Width;

    /// <summary>
    /// The pitch or linear size of the DDS texture.
    /// </summary>
    public uint PitchOrLinearSize;

    /// <summary>
    /// The depth of the DDS texture (for volume textures).
    /// </summary>
    public uint Depth;

    /// <summary>
    /// The number of mipmap levels.
    /// </summary>
    public uint MipmapCount;

    /// <summary>
    /// Reserved fields for future use.
    /// </summary>
    public fixed uint Reserved1[11];

    /// <summary>
    /// The pixel format information for the DDS texture.
    /// </summary>
    public DdsPixelFormat DdsPF;

    /// <summary>
    /// DDS surface capabilities flags.
    /// </summary>
    public uint Caps1;

    /// <summary>
    /// Additional DDS surface capabilities flags.
    /// </summary>
    public uint Caps2;

    /// <summary>
    /// Reserved fields for future use.
    /// </summary>
    public fixed uint Reserved2[3];
}
