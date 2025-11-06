# GtfDdsSharp

[![Build Status](https://github.com/Udaya-X2/GtfDdsSharp/workflows/main/badge.svg)](https://github.com/Udaya-X2/GtfDdsSharp/actions)

A .NET library to convert GTF files to and from DDS files.

## Background

[Graphics Texture Format (GTF)](https://www.psdevwiki.com/ps3/Multimedia_Formats_and_Tools#GTF) and [DirectDraw Surface (DDS)](https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide) are file formats for storing one or more textures, often used in video game development. GTF files are used by PS3 games whereas DDS files are often seen in PC games.

## Installation

This project is available as a [NuGet package](https://www.nuget.org/packages/GtfDdsSharp).

```
dotnet add package GtfDdsSharp
```

## Usage

See [documentation](https://udaya-x2.github.io/GtfDdsSharp) for API reference, samples, and tutorials.

## Code Samples

There are several ways to convert between file formats, including both file system and in-memory conversion.

### Convert a GTF file to a DDS file:
```cs
GtfImage.ConvertToDds(gtfPath, ddsPath);
```

### Convert a DDS file to a GTF file:
```cs
DdsImage.ConvertToGtf(ddsPath, gtfPath);
```

### Convert a GTF file to a DDS file in-memory:
```cs
// Read the GTF file into memory.
byte[] gtfBytes = File.ReadAllBytes(gtfPath);
using GtfImage image = new(gtfBytes);

// Convert the first texture to a DDS file.
GtfTexture texture = image[0];
byte[] ddsBytes = new byte[texture.DdsFileSize];
texture.ConvertToDds(ddsBytes);
```

### Convert a DDS file to a GTF file in-memory:
```cs
// Read the DDS file into memory.
byte[] ddsBytes = File.ReadAllBytes(ddsPath);
using DdsImage image = new(ddsBytes);

// Convert the image to a GTF file.
byte[] gtfBytes = new byte[image.GtfFileSize];
image.ConvertToGtf(gtfBytes);
```

## References

* Ported from `samples/util/gtf/libgtfconv`, a C++ library included in the PS3 SDK.
* DDS test data from [Pfim](https://github.com/Udaya-X2/GtfDdsSharp/tree/67b74aec22bb9967f6cde2a831a705337b56b12d/tests/Pfim.Tests/data), created by Nick Babcock.
* Icon by [Freepik](https://www.freepik.com/icon/replace_8718528).
