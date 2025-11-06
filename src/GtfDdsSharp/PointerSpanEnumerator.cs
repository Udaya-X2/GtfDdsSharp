namespace GtfDdsSharp;

/// <summary>
/// Provides an enumerator to process chunks of unmanaged memory as a <see cref="Span{T}"/>.
/// </summary>
/// <param name="pointer">A pointer to the starting address of a specified number of byte elements in memory.</param>
/// <param name="length">The number of byte elements to be processed by the <see cref="PointerSpanEnumerator"/>.</param>
public unsafe struct PointerSpanEnumerator(byte* pointer, uint length)
{
    private readonly byte* _pointer = pointer;
    private readonly uint _length = length;
    private uint _position;
    private int _chunkSize;

    /// <inheritdoc cref="PointerSpanEnumerator"/>
    public PointerSpanEnumerator(nint pointer, uint length)
        : this((byte*)pointer, length)
    {
    }

    /// <summary>
    /// Gets the chunk of memory at the current position of the enumerator.
    /// </summary>
    public readonly Span<byte> Current => new(_pointer + _position, _chunkSize);

    /// <summary>
    /// Advances the enumerator to the next chunk of memory.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the enumerator successfully advanced to the next chunk
    /// of memory; <see langword="false"/> if the end of the memory has been passed.
    /// </returns>
    public bool MoveNext()
    {
        _position += (uint)_chunkSize;
        return (_chunkSize = (int)Math.Min(_length - _position, (uint)Array.MaxLength)) != 0;
    }

    /// <summary>
    /// Returns an enumerator to process chunks of unmanaged memory.
    /// </summary>
    /// <returns>An enumerator to process chunks of unmanaged memory.</returns>
    public readonly PointerSpanEnumerator GetEnumerator() => this;
}
