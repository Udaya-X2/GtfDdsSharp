using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents a layout used for texture conversion.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Layout
{
    /// <summary>
    /// The offset to the DDS data.
    /// </summary>
    public uint DdsOffset;

    /// <summary>
    /// The size of the DDS data in bytes.
    /// </summary>
    public uint DdsSize;

    /// <summary>
    /// The offset to the linear GTF data.
    /// </summary>
    public uint GtfLinearOffset;

    /// <summary>
    /// The size of the linear GTF data in bytes.
    /// </summary>
    public uint GtfLinearSize;

    /// <summary>
    /// The offset to the swizzled GTF data.
    /// </summary>
    public uint GtfSwizzleOffset;

    /// <summary>
    /// The size of the swizzled GTF data in bytes.
    /// </summary>
    public uint GtfSwizzleSize;

    /// <summary>
    /// The width of the texture.
    /// </summary>
    public ushort Width;

    /// <summary>
    /// The height of the texture.
    /// </summary>
    public ushort Height;

    /// <summary>
    /// The depth of the texture.
    /// </summary>
    public ushort Depth;

    /// <summary>
    /// The color depth of the texture.
    /// </summary>
    public ushort ColorDepth;

    /// <summary>
    /// The pitch of the texture.
    /// </summary>
    public uint Pitch;

    /// <summary>
    /// The pitch of the DDS texture.
    /// </summary>
    public uint DdsPitch;

    /// <summary>
    /// The depth of the DDS texture.
    /// </summary>
    public ushort DdsDepth;

    /// <summary>
    /// Indicates whether the DDS texture should be expanded.
    /// </summary>
    public bool DdsExpand;
}
