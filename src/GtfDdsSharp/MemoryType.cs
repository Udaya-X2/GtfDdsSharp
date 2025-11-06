using System.Runtime.InteropServices;

namespace GtfDdsSharp;

/// <summary>
/// Specifies the backing memory for file data.
/// </summary>
internal enum MemoryType
{
    /// <summary>Allocated from <see cref="NativeMemory.Alloc(nuint)"/></summary>
    Native,
    /// <summary>Allocated from <see cref="GCHandle.Alloc(object?, GCHandleType)"/></summary>
    Handle,
    /// <summary>Allocated from an external source passed via pointer.</summary>
    External
}
