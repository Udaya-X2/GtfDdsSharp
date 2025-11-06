using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents information on a texture in a GTF file.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GtfTextureInfo
{
    /// <summary>
    /// The format of the texture.
    /// </summary>
    public TextureFormat Format;

    /// <summary>
    /// The mipmap level of the texture.
    /// </summary>
    public byte Mipmap;

    /// <summary>
    /// The dimension of the texture.
    /// </summary>
    public TextureDimension Dimension;

    /// <summary>
    /// Indicates whether the texture is a cubemap.
    /// </summary>
    public bool IsCubemap;

    /// <summary>
    /// The remap value for the texture.
    /// </summary>
    public uint Remap;

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
    /// The location of the texture.
    /// </summary>
    public TextureLocation Location;

    /// <summary>
    /// Padding byte for alignment.
    /// </summary>
    public byte Padding;

    /// <summary>
    /// The pitch of the texture.
    /// </summary>
    public uint Pitch;

    /// <summary>
    /// The offset of the texture.
    /// </summary>
    public uint Offset;
}
