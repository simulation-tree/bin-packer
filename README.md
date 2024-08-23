# Bin Packing
Solving the problem of "how do I arrange all of these boxes into a square?" by leveraging
the [rectpack2D](https://github.com/TeamHypersomnia/rectpack2D) algorithm.

### Using
There two main methods to the packer, one requires you to know the size ahead of time,
the other finds it for you:
```cs
RecursivePacker packer = new();
Span<Vector2> sizes = stackalloc Vector2[5];
sizes[0] = new Vector2(32, 32);
sizes[1] = new Vector2(32, 32);
sizes[2] = new Vector2(32, 32);
sizes[3] = new Vector2(32, 16);
sizes[4] = new Vector2(32, 16);
Span<Vector2> positions = stackalloc Vector2[5];

//if you know the max size ahead of time:
packer.Pack(sizes, positions, new Vector2(64, 64), padding);

//let the packer find the smallest size:
Vector2 maxSize = packer.Pack(sizes, positions, padding);
Assert.That(maxSize, Is.EqualTo(new Vector2(64, 64)));
```

> An `ImpossibleFitException` will be thrown if there is no available space left