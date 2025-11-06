using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents a texture attribute in a GTF file.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GtfTextureAttribute
{
    /// <summary>
    /// The index of the texture attribute.
    /// </summary>
    public uint Id;

    /// <summary>
    /// The offset of the texture data in the file.
    /// </summary>
    public uint OffsetToTex;

    /// <summary>
    /// The size of the texture data in bytes.
    /// </summary>
    public uint TextureSize;

    /// <summary>
    /// The texture information.
    /// </summary>
    public GtfTextureInfo Info;
}
