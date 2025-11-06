using System.Runtime.InteropServices;

namespace GtfDdsSharp.Tests;

internal unsafe ref struct NativeMemoryHandle
{
    public nint Pointer;
    public int Length;

    public NativeMemoryHandle(int length)
    {
        Length = length;
        Pointer = (nint)NativeMemory.Alloc((uint)length);
    }

    public NativeMemoryHandle(byte[] buffer)
    {
        Length = buffer.Length;
        Pointer = (nint)NativeMemory.Alloc((uint)Length);
        buffer.CopyTo(Span);
    }

    public readonly Span<byte> Span => new((void*)Pointer, Length);

    public void Dispose()
    {
        if (Pointer == 0) return;

        NativeMemory.Free((void*)Pointer);
        Pointer = 0;
        Length = 0;
    }
}
