using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Represents the file header of a GTF file.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GtfHeader
{
    /// <summary>
    /// The version of the GTF file.
    /// </summary>
    public uint Version;

    /// <summary>
    /// The total size of all textures in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// The number of textures in the GTF file.
    /// </summary>
    public uint NumTexture;
}
