using System;
using System.Numerics;

namespace BinPacker.Tests
{
    public class PackingTests
    {
        [Test]
        public void Pack4Images()
        {
            RecursivePacker packer = new();
            Vector2[] sizes =
            [
                new(100, 100),
                new(200, 200),
                new(300, 300),
                new(400, 400)
            ];

            Vector2 maxSize = new(1000, 1000);
            Vector2 padding = new(10, 10);
            Vector2[] positions = new Vector2[sizes.Length];
            packer.Pack(sizes, positions, maxSize, padding);

            for (uint i = 0; i < sizes.Length; i++)
            {
                Vector2 size = sizes[i];
                Vector2 position = positions[i];
                Console.WriteLine($"Entry {i}: {position}, {size}");
            }

            for (uint i = 0; i < sizes.Length; i++)
            {
                Vector2 size = sizes[i];
                Vector2 position = positions[i];
                bool intersectingWithAnother = IsIntersecting(position, size, i);
                if (intersectingWithAnother)
                {
                    Assert.Fail($"Input entry {position}, {size} is packed to be intersecting with another entry.");
                }
            }

            bool IsIntersecting(Vector2 position, Vector2 size, uint exceptIndex)
            {
                for (uint i = 0; i < sizes.Length; i++)
                {
                    if (i == exceptIndex)
                    {
                        continue;
                    }

                    Vector2 otherPosition = positions[i];
                    Vector2 otherSize = sizes[i];
                    if (position.X < otherPosition.X + otherSize.X &&
                        position.X + size.X > otherPosition.X &&
                        position.Y < otherPosition.Y + otherSize.Y &&
                        position.Y + size.Y > otherPosition.Y)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Test]
        public void ImpossibleToFit()
        {
            Vector2 maxSize = new(100, 100);
            Vector2 padding = new(0, 0);
            Vector2[] sizes =
            [
                new(5, 30),
                new(100, 25),
                new(50, 50),
                new(50, 50),
            ];

            Assert.Throws<ImpossibleFitException>(() =>
            {
                RecursivePacker packer = new();
                Vector2[] positions = new Vector2[sizes.Length];
                packer.Pack(sizes, positions, maxSize, padding);
                for (uint i = 0; i < sizes.Length; i++)
                {
                    Vector2 size = sizes[i];
                    Vector2 position = positions[i];
                    Console.WriteLine($"Entry {i}: {position}, {size}");
                }
            });
        }

        [Test]
        public void AutoSize()
        {
            Vector2 padding = new(0, 0);
            Vector2[] sizes = new Vector2[7];
            sizes[0] = new(5, 30);
            sizes[1] = new(100, 25);
            sizes[2] = new(50, 50);
            sizes[3] = new(50, 50);
            sizes[4] = new(8, 8);
            sizes[5] = new(8, 8);
            sizes[6] = new(8, 8);

            RecursivePacker packer = new();
            Vector2[] positions = new Vector2[sizes.Length];
            Vector2 maxSize = packer.Pack(sizes, positions, padding);
            Console.WriteLine($"Max size: {maxSize}");
            for (uint i = 0; i < sizes.Length; i++)
            {
                Vector2 size = sizes[i];
                Vector2 position = positions[i];
                Console.WriteLine($"Entry {i}: {position}, {size}");
            }
        }

        [Test]
        public void PerfectPack()
        {
            Span<Vector2> sizes = stackalloc Vector2[4]
            {
                new(32, 32),
                new(32, 32),
                new(32, 32),
                new(32, 32)
            };

            RecursivePacker packer = new();
            Span<Vector2> positions = stackalloc Vector2[4];
            packer.Pack(sizes, positions, new(64, 64), 0);
            for (int i = 0; i < sizes.Length; i++)
            {
                Vector2 size = sizes[i];
                Vector2 position = positions[i];
                Console.WriteLine($"Entry {i}: {position}, {size}");
            }
        }
    }
}
