using BinPacking;
using System;
using System.Numerics;

public static class BinPackingFunctions
{
    /// <summary>
    /// Size modes in the case of entries making use of all the space available.
    /// </summary>
    public enum SizeMode : byte
    {
        /// <summary>
        /// The next power of 2 size.
        /// </summary>
        PowerOf2,
        /// <summary>
        /// The next multiple of 2 in size.
        /// </summary>
        MultipleOf2,
        /// <summary>
        /// The next multiple of 4 in size.
        /// </summary>
        MutlipleOf4,
        Minimum
    }

    public static Vector2 GetEstimatedSize<T>(this T packer, ReadOnlySpan<Vector2> sizes, SizeMode mode, Vector2 padding = default) where T : unmanaged, IBinPacker
    {
        float sizesAreaSum = 0f;
        foreach (Vector2 size in sizes)
        {
            float width = size.X + padding.X * 2f;
            float height = size.Y + padding.Y * 2f;
            sizesAreaSum += width * height;
        }

        float dimensionSize = MathF.Sqrt(sizesAreaSum);
        switch (mode)
        {
            case SizeMode.PowerOf2:
                dimensionSize = MathF.Pow(2, MathF.Ceiling(MathF.Log2(dimensionSize)));
                break;
            case SizeMode.MultipleOf2:
                dimensionSize = MathF.Ceiling(dimensionSize / 2f) * 2f;
                break;
            case SizeMode.MutlipleOf4:
                dimensionSize = MathF.Ceiling(dimensionSize / 4f) * 4f;
                break;
            case SizeMode.Minimum:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
        }

        return new Vector2(dimensionSize, dimensionSize);
    }

    public static void Pack<T>(this T packer, ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, float padding = 0f) where T : unmanaged, IBinPacker
    {
        Pack(packer, sizes, positions, maxSize, new Vector2(padding));
    }

    /// <summary>
    /// Packs the given sizes into the span of positions.
    /// <para>An <see cref="ImpossibleFitException"/> may be thrown when the sizes are not
    /// possible to contain within the space of <paramref name="maxSize"/>.
    /// </para>
    /// </summary>
    public static void Pack<T>(this T packer, ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, Vector2 padding = default) where T : unmanaged, IBinPacker
    {
        if (sizes.Length != positions.Length)
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

    public static Vector2 Pack<T>(this T packer, ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 padding = default) where T : unmanaged, IBinPacker
    {
        Vector2 extraPadding = Vector2.Zero;
        do
        {
            Vector2 maxSize = GetEstimatedSize(packer, sizes, SizeMode.PowerOf2, padding + extraPadding);
            try
            {
                packer.Pack(sizes, positions, maxSize, padding);
                return maxSize;
            }
            catch (ImpossibleFitException)
            {
                extraPadding += new Vector2(1f);
                extraPadding *= 2;
            }
        }
        while (true);
    }

    public static Vector2 Pack<T>(this T packer, ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, float padding = 0f) where T : unmanaged, IBinPacker
    {
        return Pack(packer, sizes, positions, new Vector2(padding));
    }
}