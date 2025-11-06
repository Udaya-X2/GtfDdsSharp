---
uid: Introduction
---

<div class="article">

# Introduction

## Installation

This project is available as a [NuGet package](https://www.nuget.org/packages/GtfDdsSharp).

```
dotnet add package GtfDdsSharp
```

## Background

[Graphics Texture Format (GTF)](https://www.psdevwiki.com/ps3/Multimedia_Formats_and_Tools#GTF) and [DirectDraw Surface (DDS)](https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide) are file formats for storing one or more textures, often used in video game development. GTF files are used by PS3 games whereas DDS files are often seen in PC games.

While there are [some libraries](https://github.com/faraplay/ImasArchiveS/) and binaries that can convert GTF textures, they are either Windows-exclusive or unable to handle common types of GTF input. This library aims to fix that by providing a robust, cross-platform conversion solution.

## Usage

There are several ways to convert between file formats, including both file system and in-memory conversion. Using the [`GtfImage`](../api/GtfDdsSharp.GtfImage.yml) and [`DdsImage`](../api/GtfDdsSharp.DdsImage.yml) classes, you can read a GTF or DDS file into memory and convert it to a DDS or GTF file, respectively.

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

</div>
