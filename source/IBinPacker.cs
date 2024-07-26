using System;
using System.Numerics;

namespace BinPacking
{
    public interface IBinPacker
    {
        void Pack(ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, Vector2 padding);
    }
}