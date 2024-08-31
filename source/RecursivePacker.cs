using RectpackSharp;
using System;
using System.Numerics;
using Unmanaged.Collections;

namespace BinPacker
{
    public unsafe readonly struct RecursivePacker : IBinPacker
    {
        void IBinPacker.Pack(ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, Vector2 padding)
        {
            using UnmanagedList<PackingRectangle> boxes = new((uint)sizes.Length);
            for (uint i = 0; i < sizes.Length; i++)
            {
                Vector2 size = sizes[(int)i];
                if (size != default)
                {
                    PackingRectangle box = new();
                    box.Width = (uint)(size.X + padding.X * 2);
                    box.Height = (uint)(size.Y + padding.Y * 2);
                    box.Id = (int)i;
                    boxes.Add(box);
                }
            }

            uint maxWidth = (uint)maxSize.X;
            uint maxHeight = (uint)maxSize.Y;
            PackingHints hint = PackingHints.FindBest;
            uint acceptableDensity = 1;
            uint stepSize = 1;
            try
            {
                RectanglePacker.Pack(boxes.AsSpan(), out PackingRectangle bounds, hint, acceptableDensity, stepSize, maxWidth, maxHeight);
            }
            catch
            {
                throw new ImpossibleFitException($"Not enough space available to pack the given sizes within the bounds of {maxSize}.");
            }

            for (uint i = 0; i < boxes.Count; i++)
            {
                PackingRectangle box = boxes[i];
                uint index = (uint)box.Id;
                positions[(int)index] = new Vector2(box.X + padding.X, box.Y + padding.Y);
            }
        }
    }
}