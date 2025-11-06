using System.Numerics;

namespace GtfDdsSharp;

/// <summary>
/// Provides static methods to convert memory from GTF to DDS or vice versa using a layout buffer.
/// </summary>
public static unsafe class TextureConversion
{
    /// <summary>
    /// Creates a layout buffer using the specified GTF texture and DDS file header.
    /// </summary>
    /// <param name="texture">A GTF texture.</param>
    /// <param name="ddsHeader">A DDS file header.</param>
    /// <param name="gtfImageSize">The size of the GTF image produced by converting the DDS image.</param>
    /// <param name="ddsImageSize">The size of the DDS image.</param>
    /// <returns>The layout buffer.</returns>
    public static Layout[] CreateLayoutBuffer(in GtfTextureInfo texture,
                                              in DdsHeader ddsHeader,
                                              out uint gtfImageSize,
                                              out uint ddsImageSize)
    {
        // Calculate layout number.
        int cube = texture.IsCubemap ? 6 : 1;
        int layoutNum = cube * texture.Mipmap;

        // Initialize layout buffer.
        Layout[] layouts = new Layout[layoutNum];
        ConvertLayoutBuffer(layouts, texture, ddsHeader);

        // Compute texture size.
        bool isSwizzle = texture.IsSwizzle();
        ref Layout lastLayout = ref layouts[^1];
        gtfImageSize = isSwizzle
            ? lastLayout.GtfSwizzleOffset + lastLayout.GtfSwizzleSize
            : lastLayout.GtfLinearOffset + lastLayout.GtfLinearSize;
        ddsImageSize = lastLayout.DdsOffset + lastLayout.DdsSize;
        return layouts;
    }

    /// <summary>
    /// Initializes the specified layout buffer using the given GTF texture and DDS file header.
    /// </summary>
    /// <param name="layouts">The layout buffer to initialize.</param>
    /// <param name="texture">A GTF texture.</param>
    /// <param name="ddsHeader">A DDS file header.</param>
    public static void ConvertLayoutBuffer(Layout[] layouts, in GtfTextureInfo texture, in DdsHeader ddsHeader)
    {
        ArgumentNullException.ThrowIfNull(layouts);

        TextureFormat rawFormat = texture.GetRawFormat();
        bool isDxt = rawFormat.IsDxtn();
        bool isComp = rawFormat.IsRawCompressed();
        ushort width = texture.Width;
        ushort height = texture.Height;
        ushort depth = texture.Depth;
        uint gtfPitch = texture.Pitch;
        if (gtfPitch == 0) gtfPitch = rawFormat.GetPitch(width);
        int cube = texture.IsCubemap ? 6 : 1;
        bool isCube = cube == 6;
        bool isVolume = depth > 1;
        bool isMip = texture.Mipmap > 1;
        ushort colorDepth = rawFormat.GetDepth();

        // Convert to different colorDepth
        ushort ddsColorDepth = ddsHeader.DdsPF switch
        {
            { FourCC: DdsInfo.FOURCC_R16F } => 2,
            { RgbBitCount: 24 } => 3,
            _ => 0
        };
        Layout latest = new();
        int i = 0;

        // Cubemap
        for (int n = 0; n < cube; n++)
        {
            bool newFace = true;
            ushort w = width;
            ushort h = height;
            ushort v = depth;
            ushort m = 0;

            // Mipmap
            while (m < texture.Mipmap)
            {
                Layout layout = new()
                {
                    Width = w,
                    Height = h,
                    Depth = v,
                    Pitch = gtfPitch,
                    ColorDepth = colorDepth,
                    DdsDepth = ddsColorDepth,
                    DdsExpand = ddsColorDepth != 0
                };

                // Dxtn
                if (isDxt)
                {
                    layout.DdsSize = (w + 3u) / 4u * ((h + 3u) / 4u) * colorDepth;
                }
                // B8R8_G8R8, R8B8_R8G8
                else if (isComp)
                {
                    layout.DdsSize = (w + 1u) / 2u * h * 4u;
                }
                else
                {
                    layout.DdsSize = (uint)(w * h * colorDepth);
                }

                // Swizzle gtf size
                layout.GtfSwizzleSize = layout.DdsSize;

                if (ddsColorDepth != 0)
                {
                    layout.DdsPitch = (uint)(w * ddsColorDepth);
                    layout.DdsSize = layout.DdsPitch * h;
                }

                // Linear gtf size
                if (isDxt)
                {
                    // Not power of 2 dxtn
                    layout.GtfLinearSize = (h + 3u) / 4u * gtfPitch;
                }
                else
                {
                    layout.GtfLinearSize = h * gtfPitch;
                }

                // Volume
                if (isVolume)
                {
                    layout.DdsSize *= v;
                    layout.GtfSwizzleSize *= v;
                    layout.GtfLinearSize *= v;
                }

                // Offset
                layout.DdsOffset = latest.DdsOffset + latest.DdsSize;
                layout.GtfSwizzleOffset = latest.GtfSwizzleOffset + latest.GtfSwizzleSize;

                // When swizzle cubemap, each face must be aligned on a 128-byte boundary
                if (isCube && newFace)
                {
                    layout.GtfSwizzleOffset = MemoryUtils.GetGtfAlignment(layout.GtfSwizzleOffset);
                    newFace = false;
                }

                layout.GtfLinearOffset = latest.GtfLinearOffset + latest.GtfLinearSize;

                // Update layout
                layouts[i++] = layout;
                latest = layout;

                // Next miplevel
                if (!isMip) break;

                w >>= 1;
                h >>= 1;
                v >>= 1;

                if (w == 0 && h == 0 && v == 0) break;
                if (w == 0) w = 1;
                if (h == 0) h = 1;
                if (v == 0) v = 1;

                m++;
            }
        }
    }

    /// <summary>
    /// Converts the specified memory from GTF to DDS or vice versa using the given layout buffer and GTF texture.
    /// </summary>
    /// <param name="gtfImage">A pointer to the GTF memory.</param>
    /// <param name="gtfSize">The size of the GTF memory in bytes.</param>
    /// <param name="ddsImage">A pointer to the DDS memory.</param>
    /// <param name="ddsSize">The size of the DDS memory in bytes.</param>
    /// <param name="layouts">The layout buffer used to facilitate conversion.</param>
    /// <param name="texture">The texture corresponding to the GTF memory.</param>
    /// <param name="gtfToDds">Whether to convert from GTF to DDS or vice versa.</param>
    public static void ConvertBufferByLayout(byte* gtfImage,
                                             uint gtfSize,
                                             byte* ddsImage,
                                             uint ddsSize,
                                             Layout[] layouts,
                                             in GtfTextureInfo texture,
                                             bool gtfToDds)
    {
        ArgumentNullException.ThrowIfNull(gtfImage);
        ArgumentNullException.ThrowIfNull(ddsImage);
        ArgumentNullException.ThrowIfNull(layouts);

        bool isSwizzle = texture.IsSwizzle();
        TextureFormat rawFormat = texture.GetRawFormat();
        bool isDxt = rawFormat.IsDxtn();
        bool isComp = false;
        InvertFlag invertFlag = 0;
        uint dxtPitch = 0;
        uint blockSize = 0;

        if (isDxt)
        {
            dxtPitch = rawFormat.GetPitch(texture.Width);
            blockSize = rawFormat == TextureFormat.CompressedDxt1 ? 8u : 16u;
        }
        else
        {
            isComp = rawFormat.IsRawCompressed();
            invertFlag = rawFormat.GetInvertFlag();
        }

        foreach (Layout layout in layouts)
        {
            // Cache layout dimensions
            uint width = layout.Width;
            uint height = layout.Height;
            uint depth = layout.Depth;

            if (width == 0 || height == 0 || depth == 0) continue;

            // Get base pointers
            ulong ddsOffset = layout.DdsOffset;
            ulong gtfOffset = isSwizzle ? layout.GtfSwizzleOffset : layout.GtfLinearOffset;
            byte* ddsPtr = ddsImage + (uint)ddsOffset;
            byte* gtfPtr = gtfImage + (uint)gtfOffset;

            // Dxtn format
            if (isDxt)
            {
                uint blockWidth = (width + 3u) / 4u;
                uint blockHeight = (height + 3u) / 4u;
                uint blockDepth = (depth + 3u) / 4u;
                uint blockPitch = blockWidth * blockSize;
                uint imageSize = blockWidth * blockHeight * blockSize;

                if (isSwizzle)
                {
                    // Power of 2 dxtn
                    if (texture.Dimension == TextureDimension.ThreeDimensional)
                    {
                        uint depthBlockNum = depth % 4u;
                        if (depthBlockNum == 0) depthBlockNum = 4;

                        // Bounds check
                        ulong maxGtfOffset = gtfOffset + imageSize * depthBlockNum * blockDepth;
                        ulong maxDdsOffset = ddsOffset + imageSize * ((blockDepth - 1) * 4 + (depthBlockNum - 1));
                        maxDdsOffset += blockSize * (blockWidth - 1 + (blockHeight - 1) * blockWidth);

                        if (maxGtfOffset + blockSize > gtfSize) ThrowHelper.ThrowArgument_GtfOverflowBytes();
                        if (maxDdsOffset + blockSize > gtfSize) ThrowHelper.ThrowArgument_DdsOverflowBytes();

                        // VTC
                        for (uint z = 0; z < blockDepth; z++)
                        {
                            for (uint y = 0; y < blockHeight; y++)
                            {
                                for (uint x = 0; x < blockWidth; x++)
                                {
                                    for (uint dScan = 0; dScan < depthBlockNum; dScan++)
                                    {
                                        byte* ddsPtr2 = ddsPtr + imageSize * (z * 4 + dScan);
                                        ddsPtr2 += blockSize * (x + y * blockWidth);
                                        MemoryUtils.MoveMemory(gtfPtr, ddsPtr2, blockSize, gtfToDds);
                                        gtfPtr += blockSize;
                                    }
                                }
                            }
                        }
                    }
                    // Not VTC & power of 2
                    else
                    {
                        // Bounds check
                        if (gtfOffset + imageSize > gtfSize) ThrowHelper.ThrowArgument_GtfOverflowBytes();
                        if (ddsOffset + imageSize > ddsSize) ThrowHelper.ThrowArgument_DdsOverflowBytes();

                        MemoryUtils.MoveMemory(gtfPtr, ddsPtr, imageSize, gtfToDds);
                    }
                }
                // Not power of 2 dxtn
                else
                {
                    // Bounds check
                    ulong maxGtfOffset = gtfOffset + dxtPitch * blockHeight * (depth - 1);
                    maxGtfOffset += dxtPitch * (blockHeight - 1);
                    ulong maxDdsOffset = ddsOffset + imageSize * (depth - 1) + blockPitch * (blockHeight - 1);

                    if (maxGtfOffset + blockPitch > gtfSize) ThrowHelper.ThrowArgument_GtfOverflowBytes();
                    if (maxDdsOffset + blockPitch > ddsSize) ThrowHelper.ThrowArgument_DdsOverflowBytes();

                    for (uint d = 0; d < depth; d++)
                    {
                        for (uint blockLine = 0; blockLine < blockHeight; blockLine++)
                        {
                            byte* gtfPtr2 = gtfPtr + dxtPitch * blockHeight * d + dxtPitch * blockLine;
                            byte* ddsPtr2 = ddsPtr + imageSize * d + blockPitch * blockLine;
                            MemoryUtils.MoveMemory(gtfPtr2, ddsPtr2, blockPitch, gtfToDds);
                        }
                    }
                }
            }
            // Not dxtn format
            else
            {
                // Cache layout fields
                uint colorDepth = layout.ColorDepth;
                uint pitch = layout.Pitch;
                uint ddsPitch = layout.DdsPitch;
                uint ddsDepth = layout.DdsDepth;
                uint copySize = invertFlag switch
                {
                    InvertFlag.InvertEndian32 => colorDepth,
                    InvertFlag.InvertEndian16 => colorDepth,
                    InvertFlag.InvertEndian32Even => 4,
                    _ => 0
                };

                if (copySize == 0) continue;

                // 64-bit and 128-bit fat texel swizzled memory layout can be described as "not quite swizzled."
                if (isSwizzle)
                {
                    // A swizzled 128-bit texture is treated as a 32-bit texture that happens to be four times as wide.
                    if (rawFormat == TextureFormat.W32Z32Y32X32Float)
                    {
                        width *= 4;
                        colorDepth = 4;
                    }
                    // A swizzled 64-bit texture is treated as a 32-bit texture that happens to be twice as wide.
                    else if (rawFormat == TextureFormat.W16Z16Y16X16Float)
                    {
                        width *= 2;
                        colorDepth = 4;
                    }
                }
                if (isComp)
                {
                    width = (width + 1) / 2 * 2;
                }
                if (!layout.DdsExpand)
                {
                    ddsDepth = colorDepth;
                    ddsPitch = width * ddsDepth;
                }

                // Cache loop invariants
                uint log2Width = 0;
                uint log2Height = 0;
                uint log2Depth = 0;

                // Compute base-2 logarithms for all dimensions
                if (isSwizzle)
                {
                    log2Width = (uint)BitOperations.Log2(width);
                    log2Height = (uint)BitOperations.Log2(height);
                    log2Depth = (uint)BitOperations.Log2(depth);
                }

                // Bounds check
                ulong maxDdsOffset = ddsOffset + (depth - 1) * ddsPitch * height;
                maxDdsOffset += (height - 1) * ddsPitch + (width - 1) * ddsDepth;
                ulong maxGtfOffset = isSwizzle
                    ? gtfOffset + ((1UL << (int)(log2Width + log2Height + log2Depth)) - 1) * colorDepth
                    : gtfOffset + (depth - 1) * height * pitch + (height - 1) * pitch + (width - 1) * colorDepth;

                if (maxGtfOffset + copySize > gtfSize) ThrowHelper.ThrowArgument_GtfOverflowBytes();
                if (maxDdsOffset + copySize > ddsSize) ThrowHelper.ThrowArgument_DdsOverflowBytes();

                for (uint z = 0; z < depth; z++)
                {
                    for (uint y = 0; y < height; y++)
                    {
                        for (uint x = 0; x < width; x++)
                        {
                            // Get pointers
                            byte* ddsPtr2 = ddsPtr + z * ddsPitch * height + y * ddsPitch + x * ddsDepth;
                            byte* gtfPtr2 = isSwizzle
                                ? gtfPtr + MemoryUtils.ToSwizzle(x, y, z, log2Width, log2Height, log2Depth) * colorDepth
                                : gtfPtr + z * height * pitch + y * pitch + x * colorDepth;

                            switch (invertFlag)
                            {
                                // 32-bit
                                case InvertFlag.InvertEndian32:
                                    MemoryUtils.MoveMemoryWithInvertEndian32(gtfPtr2, ddsPtr2, colorDepth, gtfToDds);
                                    break;
                                // 16-bit
                                case InvertFlag.InvertEndian16:
                                    MemoryUtils.MoveMemoryWithInvertEndian16(gtfPtr2, ddsPtr2, colorDepth, gtfToDds);
                                    break;
                                // B8R8_G8R8, R8B8_R8G8 (even pixel)
                                case InvertFlag.InvertEndian32Even when x % 2 == 0:
                                    MemoryUtils.MoveMemoryWithInvertEndian32(gtfPtr2, ddsPtr2, 4, gtfToDds);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
