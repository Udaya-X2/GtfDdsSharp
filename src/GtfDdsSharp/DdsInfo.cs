namespace GtfDdsSharp;

/// <summary>
/// Defines constants related to the DDS file format.
/// </summary>
public static class DdsInfo
{
    /// <summary>
    /// The size of a DDS header in bytes.
    /// </summary>
    public const uint DDS_HEADER_SIZE = 124;

    /// <summary>
    /// The size of a DDS pixel format in bytes.
    /// </summary>
    public const uint DDS_PIXELFORMAT_SIZE = 32;

    /// <summary>
    /// The maximum number of mipmap levels supported.
    /// </summary>
    public const uint DDSF_MAX_MIPMAPS = 16;

    /// <summary>
    /// The maximum volume size supported.
    /// </summary>
    public const uint DDSF_MAX_VOLUME = 512;

    /// <summary>
    /// The maximum number of textures supported.
    /// </summary>
    public const uint DDSF_MAX_TEXTURES = 16;

    /// <summary>
    /// DDS surface capability flag.
    /// </summary>
    public const uint DDSF_CAPS = 0x00000001;

    /// <summary>
    /// DDS height flag.
    /// </summary>
    public const uint DDSF_HEIGHT = 0x00000002;

    /// <summary>
    /// DDS width flag.
    /// </summary>
    public const uint DDSF_WIDTH = 0x00000004;

    /// <summary>
    /// DDS pitch flag.
    /// </summary>
    public const uint DDSF_PITCH = 0x00000008;

    /// <summary>
    /// DDS pixel format flag.
    /// </summary>
    public const uint DDSF_PIXELFORMAT = 0x00001000;

    /// <summary>
    /// DDS mipmap count flag.
    /// </summary>
    public const uint DDSF_MIPMAPCOUNT = 0x00020000;

    /// <summary>
    /// DDS linear size flag.
    /// </summary>
    public const uint DDSF_LINEARSIZE = 0x00080000;

    /// <summary>
    /// DDS depth flag.
    /// </summary>
    public const uint DDSF_DEPTH = 0x00800000;

    /// <summary>
    /// DDS pixel format flag for alpha pixels.
    /// </summary>
    public const uint DDSF_ALPHAPIXELS = 0x00000001;

    /// <summary>
    /// DDS pixel format flag for FOURCC codes.
    /// </summary>
    public const uint DDSF_FOURCC = 0x00000004;

    /// <summary>
    /// DDS pixel format flag for RGB data.
    /// </summary>
    public const uint DDSF_RGB = 0x00000040;

    /// <summary>
    /// DDS pixel format flag for RGBA data.
    /// </summary>
    public const uint DDSF_RGBA = 0x00000041;

    /// <summary>
    /// DDS pixel format flag for luminance data.
    /// </summary>
    public const uint DDSF_LUMINANCE = 0x00020000;

    /// <summary>
    /// DDS pixel format flag for bump mapping.
    /// </summary>
    public const uint DDSF_BUMPDUDV = 0x00080000;

    /// <summary>
    /// DDS surface capability flag for complex surfaces.
    /// </summary>
    public const uint DDSF_COMPLEX = 0x00000008;

    /// <summary>
    /// DDS surface capability flag for textures.
    /// </summary>
    public const uint DDSF_TEXTURE = 0x00001000;

    /// <summary>
    /// DDS surface capability flag for mipmaps.
    /// </summary>
    public const uint DDSF_MIPMAP = 0x00400000;

    /// <summary>
    /// DDS surface capability flag for cubemaps.
    /// </summary>
    public const uint DDSF_CUBEMAP = 0x00000200;

    /// <summary>
    /// DDS cubemap face flag for positive X.
    /// </summary>
    public const uint DDSF_CUBEMAP_POSITIVEX = 0x00000400;

    /// <summary>
    /// DDS cubemap face flag for negative X.
    /// </summary>
    public const uint DDSF_CUBEMAP_NEGATIVEX = 0x00000800;

    /// <summary>
    /// DDS cubemap face flag for positive Y.
    /// </summary>
    public const uint DDSF_CUBEMAP_POSITIVEY = 0x00001000;

    /// <summary>
    /// DDS cubemap face flag for negative Y.
    /// </summary>
    public const uint DDSF_CUBEMAP_NEGATIVEY = 0x00002000;

    /// <summary>
    /// DDS cubemap face flag for positive Z.
    /// </summary>
    public const uint DDSF_CUBEMAP_POSITIVEZ = 0x00004000;

    /// <summary>
    /// DDS cubemap face flag for negative Z.
    /// </summary>
    public const uint DDSF_CUBEMAP_NEGATIVEZ = 0x00008000;

    /// <summary>
    /// DDS cubemap flag for all faces.
    /// </summary>
    public const uint DDSF_CUBEMAP_ALL_FACES = 0x0000FC00;

    /// <summary>
    /// DDS volume texture flag.
    /// </summary>
    public const uint DDSF_VOLUME = 0x00200000;

    /// <summary>
    /// FOURCC code for R16F format.
    /// </summary>
    public const uint FOURCC_R16F = 0x6F;

    /// <summary>
    /// FOURCC code for G16R16F format.
    /// </summary>
    public const uint FOURCC_G16R16F = 0x70;

    /// <summary>
    /// FOURCC code for A16B16G16R16F format.
    /// </summary>
    public const uint FOURCC_A16B16G16R16F = 0x71;

    /// <summary>
    /// FOURCC code for R32F format.
    /// </summary>
    public const uint FOURCC_R32F = 0x72;

    /// <summary>
    /// FOURCC code for A32B32G32R32F format.
    /// </summary>
    public const uint FOURCC_A32B32G32R32F = 0x74;

    /// <summary>
    /// FOURCC code for YVYU format.
    /// </summary>
    public const uint FOURCC_YVYU = 'Y' | ('V' << 8) | ('Y' << 16) | ('U' << 24);

    /// <summary>
    /// FOURCC code for YUY2 format.
    /// </summary>
    public const uint FOURCC_YUY2 = 'Y' | ('U' << 8) | ('Y' << 16) | ('2' << 24);

    /// <summary>
    /// FOURCC code for R8G8_B8G8 format.
    /// </summary>
    public const uint FOURCC_R8G8_B8G8 = 'R' | ('G' << 8) | ('B' << 16) | ('G' << 24);

    /// <summary>
    /// FOURCC code for G8R8_G8B8 format.
    /// </summary>
    public const uint FOURCC_G8R8_G8B8 = 'G' | ('R' << 8) | ('G' << 16) | ('B' << 24);

    /// <summary>
    /// FOURCC code for DDS format.
    /// </summary>
    public const uint FOURCC_DDS = 'D' | ('D' << 8) | ('S' << 16) | (' ' << 24);

    /// <summary>
    /// FOURCC code for DXT1 compressed format.
    /// </summary>
    public const uint FOURCC_DXT1 = 'D' | ('X' << 8) | ('T' << 16) | ('1' << 24);

    /// <summary>
    /// FOURCC code for DXT2 compressed format.
    /// </summary>
    public const uint FOURCC_DXT2 = 'D' | ('X' << 8) | ('T' << 16) | ('2' << 24);

    /// <summary>
    /// FOURCC code for DXT3 compressed format.
    /// </summary>
    public const uint FOURCC_DXT3 = 'D' | ('X' << 8) | ('T' << 16) | ('3' << 24);

    /// <summary>
    /// FOURCC code for DXT4 compressed format.
    /// </summary>
    public const uint FOURCC_DXT4 = 'D' | ('X' << 8) | ('T' << 16) | ('4' << 24);

    /// <summary>
    /// FOURCC code for DXT5 compressed format.
    /// </summary>
    public const uint FOURCC_DXT5 = 'D' | ('X' << 8) | ('T' << 16) | ('5' << 24);

    /// <summary>
    /// FOURCC code for DX10 format.
    /// </summary>
    public const uint FOURCC_DX10 = 'D' | ('X' << 8) | ('1' << 16) | ('0' << 24);

    /// <summary>
    /// FOURCC code for RXGB format.
    /// </summary>
    public const uint FOURCC_RXGB = 'R' | ('X' << 8) | ('G' << 16) | ('B' << 24);

    /// <summary>
    /// FOURCC code for ATI1 format.
    /// </summary>
    public const uint FOURCC_ATI1 = 'A' | ('T' << 8) | ('I' << 16) | ('1' << 24);

    /// <summary>
    /// FOURCC code for ATI2 format.
    /// </summary>
    public const uint FOURCC_ATI2 = 'A' | ('T' << 8) | ('I' << 16) | ('2' << 24);

    /// <summary>
    /// D3D format code for R8G8B8.
    /// </summary>
    public const uint D3DFMT_R8G8B8 = 2;

    /// <summary>
    /// D3D format code for A8R8G8B8.
    /// </summary>
    public const uint D3DFMT_A8R8G8B8 = 2;

    /// <summary>
    /// D3D format code for X8R8G8B8.
    /// </summary>
    public const uint D3DFMT_X8R8G8B8 = 2;

    /// <summary>
    /// D3D format code for R5G6B5.
    /// </summary>
    public const uint D3DFMT_R5G6B5 = 2;

    /// <summary>
    /// D3D format code for X1R5G5B5.
    /// </summary>
    public const uint D3DFMT_X1R5G5B5 = 2;

    /// <summary>
    /// D3D format code for A1R5G5B5.
    /// </summary>
    public const uint D3DFMT_A1R5G5B5 = 2;

    /// <summary>
    /// D3D format code for A4R4G4B4.
    /// </summary>
    public const uint D3DFMT_A4R4G4B4 = 2;

    /// <summary>
    /// D3D format code for R3G3B2.
    /// </summary>
    public const uint D3DFMT_R3G3B2 = 2;

    /// <summary>
    /// D3D format code for A8.
    /// </summary>
    public const uint D3DFMT_A8 = 2;

    /// <summary>
    /// D3D format code for A8R3G3B2.
    /// </summary>
    public const uint D3DFMT_A8R3G3B2 = 2;

    /// <summary>
    /// D3D format code for X4R4G4B4.
    /// </summary>
    public const uint D3DFMT_X4R4G4B4 = 3;

    /// <summary>
    /// D3D format code for A2B10G10R10.
    /// </summary>
    public const uint D3DFMT_A2B10G10R10 = 3;

    /// <summary>
    /// D3D format code for A8B8G8R8.
    /// </summary>
    public const uint D3DFMT_A8B8G8R8 = 3;

    /// <summary>
    /// D3D format code for X8B8G8R8.
    /// </summary>
    public const uint D3DFMT_X8B8G8R8 = 3;

    /// <summary>
    /// D3D format code for G16R16.
    /// </summary>
    public const uint D3DFMT_G16R16 = 3;

    /// <summary>
    /// D3D format code for A2R10G10B10.
    /// </summary>
    public const uint D3DFMT_A2R10G10B10 = 3;

    /// <summary>
    /// D3D format code for A16B16G16R16.
    /// </summary>
    public const uint D3DFMT_A16B16G16R16 = 3;

    /// <summary>
    /// D3D format code for A8P8 palette format.
    /// </summary>
    public const uint D3DFMT_A8P8 = 4;

    /// <summary>
    /// D3D format code for P8 palette format.
    /// </summary>
    public const uint D3DFMT_P8 = 4;

    /// <summary>
    /// D3D format code for L8 luminance format.
    /// </summary>
    public const uint D3DFMT_L8 = 5;

    /// <summary>
    /// D3D format code for A8L8 luminance format.
    /// </summary>
    public const uint D3DFMT_A8L8 = 5;

    /// <summary>
    /// D3D format code for A4L4 luminance format.
    /// </summary>
    public const uint D3DFMT_A4L4 = 5;

    /// <summary>
    /// D3D format code for R16F floating point format.
    /// </summary>
    public const uint D3DFMT_R16F = 11;

    /// <summary>
    /// D3D format code for G16R16F floating point format.
    /// </summary>
    public const uint D3DFMT_G16R16F = 11;

    /// <summary>
    /// D3D format code for A16B16G16R16F floating point format.
    /// </summary>
    public const uint D3DFMT_A16B16G16R16F = 11;

    /// <summary>
    /// D3D format code for R32F floating point format.
    /// </summary>
    public const uint D3DFMT_R32F = 11;

    /// <summary>
    /// D3D format code for G32R32F floating point format.
    /// </summary>
    public const uint D3DFMT_G32R32F = 11;

    /// <summary>
    /// D3D format code for A32B32G32R32F floating point format.
    /// </summary>
    public const uint D3DFMT_A32B32G32R32F = 11;

    /// <summary>
    /// DDS header flag for surface capabilities.
    /// </summary>
    public const uint DDSD_CAPS = 0x00000001;

    /// <summary>
    /// DDS header flag for pixel format.
    /// </summary>
    public const uint DDSD_PIXELFORMAT = 0x00001000;

    /// <summary>
    /// DDS header flag for width.
    /// </summary>
    public const uint DDSD_WIDTH = 0x00000004;

    /// <summary>
    /// DDS header flag for height.
    /// </summary>
    public const uint DDSD_HEIGHT = 0x00000002;

    /// <summary>
    /// DDS header flag for pitch.
    /// </summary>
    public const uint DDSD_PITCH = 0x00000008;

    /// <summary>
    /// DDS header flag for mipmap count.
    /// </summary>
    public const uint DDSD_MIPMAPCOUNT = 0x00020000;

    /// <summary>
    /// DDS header flag for linear size.
    /// </summary>
    public const uint DDSD_LINEARSIZE = 0x00080000;

    /// <summary>
    /// DDS header flag for depth.
    /// </summary>
    public const uint DDSD_DEPTH = 0x00800000;

    /// <summary>
    /// DDS surface capability flag for alpha.
    /// </summary>
    public const uint DDSCAPS_ALPHA = 0x00000002;

    /// <summary>
    /// DDS surface capability flag for complex surfaces.
    /// </summary>
    public const uint DDSCAPS_COMPLEX = 0x00000008;

    /// <summary>
    /// DDS surface capability flag for textures.
    /// </summary>
    public const uint DDSCAPS_TEXTURE = 0x00001000;

    /// <summary>
    /// DDS surface capability flag for mipmaps.
    /// </summary>
    public const uint DDSCAPS_MIPMAP = 0x00400000;

    /// <summary>
    /// DDS surface capability flag for volume textures.
    /// </summary>
    public const uint DDSCAPS2_VOLUME = 0x00200000;

    /// <summary>
    /// DDS surface capability flag for cubemaps.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP = 0x00000200;

    /// <summary>
    /// DDS cubemap face flag for positive X.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400;

    /// <summary>
    /// DDS cubemap face flag for negative X.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800;

    /// <summary>
    /// DDS cubemap face flag for positive Y.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000;

    /// <summary>
    /// DDS cubemap face flag for negative Y.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000;

    /// <summary>
    /// DDS cubemap face flag for positive Z.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000;

    /// <summary>
    /// DDS cubemap face flag for negative Z.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000;

    /// <summary>
    /// DDS cubemap flag for all faces.
    /// </summary>
    public const uint DDSCAPS2_CUBEMAP_ALL_FACES = 0x0000FC00;

    /// <summary>
    /// DDS pixel format flag for alpha pixels.
    /// </summary>
    public const uint DDPF_ALPHAPIXELS = 0x00000001;

    /// <summary>
    /// DDS pixel format flag for alpha only.
    /// </summary>
    public const uint DDPF_ALPHA = 0x00000002;

    /// <summary>
    /// DDS pixel format flag for FOURCC codes.
    /// </summary>
    public const uint DDPF_FOURCC = 0x00000004;

    /// <summary>
    /// DDS pixel format flag for RGB data.
    /// </summary>
    public const uint DDPF_RGB = 0x00000040;

    /// <summary>
    /// DDS pixel format flag for palette indexed 1.
    /// </summary>
    public const uint DDPF_PALETTEINDEXED1 = 0x00000800;

    /// <summary>
    /// DDS pixel format flag for palette indexed 2.
    /// </summary>
    public const uint DDPF_PALETTEINDEXED2 = 0x00001000;

    /// <summary>
    /// DDS pixel format flag for palette indexed 4.
    /// </summary>
    public const uint DDPF_PALETTEINDEXED4 = 0x00000008;

    /// <summary>
    /// DDS pixel format flag for palette indexed 8.
    /// </summary>
    public const uint DDPF_PALETTEINDEXED8 = 0x00000020;

    /// <summary>
    /// DDS pixel format flag for luminance data.
    /// </summary>
    public const uint DDPF_LUMINANCE = 0x00020000;

    /// <summary>
    /// DDS pixel format flag for premultiplied alpha.
    /// </summary>
    public const uint DDPF_ALPHAPREMULT = 0x00008000;

    /// <summary>
    /// DDS pixel format flag for normal maps.
    /// </summary>
    public const uint DDPF_NORMAL = 0x80000000;
}
