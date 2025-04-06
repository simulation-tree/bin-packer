using RectpackSharp;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BinPacker
{
    [SkipLocalsInit]
    public readonly struct RecursivePacker : IBinPacker
    {
        bool IBinPacker.TryPack(ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, Vector2 padding)
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

            Trace.WriteLine("2");
            uint maxWidth = (uint)maxSize.X;
            uint maxHeight = (uint)maxSize.Y;
            PackingHints hint = PackingHints.FindBest;
            uint acceptableDensity = 1;
            uint stepSize = 1;
            try
            {
                if (!RectanglePacker.TryPack(boxes.Slice(0, boxesCount), out PackingRectangle bounds, hint, acceptableDensity, stepSize, maxWidth, maxHeight))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw;
            }

            for (int i = 0; i < boxesCount; i++)
            {
                PackingRectangle box = boxes[i];
                positions[box.Id] = new Vector2(box.X + padding.X, box.Y + padding.Y);
            }

            return true;
        }
    }
}