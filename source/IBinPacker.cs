using System;
using System.Numerics;

namespace BinPacker
{
    public interface IBinPacker
    {
        bool TryPack(ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, Vector2 padding);
    }
}