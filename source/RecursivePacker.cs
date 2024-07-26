using System;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

namespace BinPacking
{
    public unsafe readonly struct RecursivePacker : IBinPacker
    {
        void IBinPacker.Pack(ReadOnlySpan<Vector2> sizes, Span<Vector2> positions, Vector2 maxSize, Vector2 padding)
        {
            using UnmanagedList<nint> allocations = UnmanagedList<nint>.Create();
            using UnmanagedList<float> areas = UnmanagedList<float>.Create();
            using UnmanagedArray<uint> indices = new((uint)sizes.Length);
            foreach (Vector2 size in sizes)
            {
                areas.Add(size.X * size.Y);
            }

            //sort the sizes into boxes by sorted in descending order to their area
            using UnmanagedArray<Box> boxes = new(indices.Length);
            uint offset = 0;
            for (uint i = 0; i < sizes.Length; i++)
            {
                uint index = uint.MaxValue;
                float threshold = float.MaxValue;
                for (uint a = areas.Count - 1; a != uint.MaxValue; a--)
                {
                    float area = areas[a];
                    if (area < threshold)
                    {
                        index = a;
                        threshold = area;
                    }
                }

                areas.RemoveAt(index);
                index += offset;
                offset++;
                Vector2 inputSize = sizes[(int)index];
                Vector2 size = inputSize + padding * 2f;
                //boxes[i] = new Box(size);
                //indices[i] = index;
                boxes[(uint)(sizes.Length - i - 1)] = new Box(size);
                indices[(uint)(sizes.Length - i - 1)] = index;
            }

            //start packing
            Node* rootNode = Allocations.Allocate<Node>();
            rootNode[0] = new Node(maxSize);
            allocations.Add((nint)rootNode);

            for (uint i = 0; i < boxes.Length; i++)
            {
                ref Box box = ref boxes.GetRef(i);
                Node* node = Find(rootNode, box.size);
                if (node is null)
                {
                    CleanUp();
                    throw new ImpossibleFitException("Remaining space is unable to contain the next entry fit, the max size isn't great enough.");
                }
            
                box.node = node;
                Split(box);
            }
            
            //finish packing
            for (uint i = 0; i < boxes.Length; i++)
            {
                Box box = boxes[i];
                uint index = indices[i];
                positions[(int)index] = box.node->position + padding;
            }
            
            CleanUp();

            void CleanUp()
            {
                foreach (nint allocation in allocations)
                {
                    void* ptr = (void*)allocation;
                    Allocations.Free(ref ptr);
                }
            }

            Node* Find(Node* rootNode, Vector2 size)
            {
                Node* node = default;
                if (rootNode->used)
                {
                    Node* nextNode = Find(rootNode->up, size);
                    if (nextNode is null)
                    {
                        nextNode = Find(rootNode->right, size);
                    }

                    return nextNode;
                }
                else if (rootNode->size.X >= size.X && rootNode->size.Y >= size.Y)
                {
                    return rootNode;
                }
                else
                {
                    return null;
                }
            }

            void Split(Box box)
            {
                Node* node = box.node;
                Vector2 size = box.size;
                node->used = true;
                Node* up = Allocations.Allocate<Node>();
                Node* right = Allocations.Allocate<Node>();
                allocations.Add((nint)up);
                allocations.Add((nint)right);
                up[0] = new Node(node->position.X + size.X, node->position.Y, node->size.X - size.X, size.Y);
                right[0] = new Node(node->position.X, node->position.Y + size.Y, node->size.X, node->size.Y - size.Y);
                node->up = up;
                node->right = right;
            }
        }

        private struct Node
        {
            public Node* right;
            public Node* up;
            public bool used;
            public Vector2 position;
            public Vector2 size;

            public Node(Vector2 size)
            {
                this.right = null;
                this.up = null;
                this.used = false;
                this.position = Vector2.Zero;
                this.size = size;
            }

            public Node(float x, float y, float width, float height)
            {
                this.right = null;
                this.up = null;
                this.used = false;
                this.position = new Vector2(x, y);
                this.size = new Vector2(width, height);
            }
        }

        private struct Box
        {
            public Vector2 size;
            public float area;
            public Node* node;

            public Box(Vector2 size, Node* node = default)
            {
                this.size = size;
                this.area = size.X * size.Y;
                this.node = node;
            }
        }
    }
}