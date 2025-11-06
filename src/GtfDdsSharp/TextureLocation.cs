namespace GtfDdsSharp;

/// <summary>
/// Specifies the location of a GTF texture.
/// </summary>
public enum TextureLocation : byte
{
    /// <summary>
    /// The texture is located in local memory.
    /// </summary>
    Local = 0,
    /// <summary>
    /// The texture is located in main memory.
    /// </summary>
    Main = 1
}
