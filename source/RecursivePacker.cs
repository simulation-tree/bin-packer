using RectpackSharp;
using System;
using System.Numerics;

namespace BinPacker
{
    public unsafe readonly struct RecursivePacker : IBinPacker
    {
        void IBinPacker.Pack(ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, Vector2 padding)
        {
            Span<PackingRectangle> boxes = stackalloc PackingRectangle[sizes.Length];
            int boxesCount = 0;
            for (int i = 0; i < sizes.Length; i++)
            {
                Vector2 size = sizes[i];
                if (size != default)
                {
                    PackingRectangle box = new();
                    box.Width = (uint)(size.X + padding.X * 2);
                    box.Height = (uint)(size.Y + padding.Y * 2);
                    box.Id = i;
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
                RectanglePacker.Pack(boxes.Slice(0, boxesCount), out PackingRectangle bounds, hint, acceptableDensity, stepSize, maxWidth, maxHeight);
            }
            catch
            {
                throw new ImpossibleFitException($"Not enough space available to pack the given sizes within the bounds of {maxSize}.");
            }

            for (int i = 0; i < boxesCount; i++)
            {
                PackingRectangle box = boxes[i];
                positions[box.Id] = new Vector2(box.X + padding.X, box.Y + padding.Y);
            }
        }
    }
}