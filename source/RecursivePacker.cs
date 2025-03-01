using RectpackSharp;
using System.Numerics;
using Unmanaged;

namespace BinPacker
{
    public unsafe readonly struct RecursivePacker : IBinPacker
    {
        void IBinPacker.Pack(USpan<Vector2> sizes, USpan<Vector2> positions, Vector2 maxSize, Vector2 padding)
        {
            USpan<PackingRectangle> boxes = stackalloc PackingRectangle[(int)sizes.Length];
            uint boxesCount = 0;
            for (uint i = 0; i < sizes.Length; i++)
            {
                Vector2 size = sizes[i];
                if (size != default)
                {
                    PackingRectangle box = new();
                    box.Width = (uint)(size.X + padding.X * 2);
                    box.Height = (uint)(size.Y + padding.Y * 2);
                    box.Id = (int)i;
                    boxes[boxesCount++] = box;
                }
            }

            uint maxWidth = (uint)maxSize.X;
            uint maxHeight = (uint)maxSize.Y;
            PackingHints hint = PackingHints.FindBest;
            uint acceptableDensity = 1;
            uint stepSize = 1;
            try
            {
                RectanglePacker.Pack(boxes.GetSpan(boxesCount), out PackingRectangle bounds, hint, acceptableDensity, stepSize, maxWidth, maxHeight);
            }
            catch
            {
                throw new ImpossibleFitException($"Not enough space available to pack the given sizes within the bounds of {maxSize}.");
            }

            for (uint i = 0; i < boxesCount; i++)
            {
                PackingRectangle box = boxes[i];
                uint index = (uint)box.Id;
                positions[index] = new Vector2(box.X + padding.X, box.Y + padding.Y);
            }
        }
    }
}