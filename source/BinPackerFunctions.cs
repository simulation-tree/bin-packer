using BinPacker;
using System;
using System.Numerics;
using Unmanaged;

public static class BinPackerFunctions
{
    /// <summary>
    /// Packs the given sizes into the span of positions.
    /// <para>An <see cref="ImpossibleFitException"/> may be thrown when the sizes are not
    /// possible to contain within the space of <paramref name="maxSize"/>.
    /// </para>
    /// </summary>
    public static void Pack<T>(this T packer, USpan<Vector2> sizes, USpan<Vector2> positions, Vector2 maxSize, float padding = 0f) where T : unmanaged, IBinPacker
    {
        Pack(packer, sizes, positions, maxSize, new Vector2(padding));
    }

    /// <summary>
    /// Packs the given sizes into the span of positions.
    /// <para>An <see cref="ImpossibleFitException"/> may be thrown when the sizes are not
    /// possible to contain within the space of <paramref name="maxSize"/>.
    /// </para>
    /// </summary>
    public static void Pack<T>(this T packer, USpan<Vector2> sizes, USpan<Vector2> positions, Vector2 maxSize, Vector2 padding = default) where T : unmanaged, IBinPacker
    {
        if (sizes.length != positions.length)
        {
            throw new ArgumentException("Span of positions must be the same length as the span of sizes.");
        }

        float sizesAreaSum = 0f;
        foreach (Vector2 size in sizes)
        {
            float width = size.X + padding.X * 2f;
            float height = size.Y + padding.Y * 2f;
            sizesAreaSum += width * height;
        }

        float maxSizeArea = maxSize.X * maxSize.Y;
        if (sizesAreaSum > maxSizeArea)
        {
            throw new ImpossibleFitException("Not enough space available to pack the given sizes.");
        }

        packer.Pack(sizes, positions, maxSize, padding);
    }

    /// <summary>
    /// Packs the given sizes and returns the smallest possible size to contain them all.
    /// </summary>
    public static Vector2 Pack<T>(this T packer, USpan<Vector2> sizes, USpan<Vector2> positions, Vector2 padding = default) where T : unmanaged, IBinPacker
    {
        Vector2 size = new(4, 4);
        do
        {
            try
            {
                packer.Pack(sizes, positions, size, padding);
                return size;
            }
            catch (ImpossibleFitException)
            {
                size *= 2;
            }
        }
        while (true);
    }

    /// <summary>
    /// Packs the given sizes and returns the smallest possible size to contain them all.
    /// </summary>
    public static Vector2 Pack<T>(this T packer, USpan<Vector2> sizes, USpan<Vector2> positions, float padding = 0f) where T : unmanaged, IBinPacker
    {
        return Pack(packer, sizes, positions, new Vector2(padding));
    }
}