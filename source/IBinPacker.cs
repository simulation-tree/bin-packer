using System.Numerics;
using Unmanaged;

namespace BinPacker
{
    public interface IBinPacker
    {
        void Pack(USpan<Vector2> sizes, USpan<Vector2> positions, Vector2 maxSize, Vector2 padding);
    }
}