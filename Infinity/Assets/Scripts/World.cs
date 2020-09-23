using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class World
    {
        public static World ActiveWorld = new World();

        private readonly Dictionary<Vector3Int, Chunk> _chunks;

        public BlockType this[Vector3Int pos]
        {
            get => this[pos.x, pos.y, pos.z];
            set => this[pos.x, pos.y, pos.z] = value;
        }

        public BlockType this[int x, int y, int z]
        {
            get
            {
                var chunkPosition = GetChunkIndex(x, y, z);
                var chunk = Chunks[chunkPosition];
                if (chunk == null) return null;
                return chunk[x - chunk.WorldPosition.x, y - chunk.WorldPosition.y, z - chunk.WorldPosition.z];
            }
            set
            {
                var chunkPosition = GetChunkIndex(x, y, z);
                var chunk = Chunks[chunkPosition];
                if (chunk == null)
                {
                    // TODO: Generate chunk
                    return;
                }
                chunk[x - chunk.WorldPosition.x, y - chunk.WorldPosition.y, z - chunk.WorldPosition.z] = value;
            }
        }

        public Vector3Int GetChunkIndex(Vector3Int pos) => GetChunkIndex(pos.x, pos.y, pos.z);
        public Vector3Int GetChunkIndex(int x, int y, int z) =>
            new Vector3Int(FlooredIntDivision(x, Chunk.Size.x), FlooredIntDivision(y, Chunk.Size.y), FlooredIntDivision(z, Chunk.Size.z));

        private int FlooredIntDivision(int a, int b) => (a / b) - Convert.ToInt32(((a < 0) ^ (b < 0)) && (a % b != 0));

        public World()
        {
            _chunks = new Dictionary<Vector3Int, Chunk>();
            Chunks = new ChunkIndexer(this);
        }

        public ChunkIndexer Chunks;

        public struct ChunkIndexer
        {
            public readonly World World;

            public ChunkIndexer(World world)
            {
                World = world;
            }

            public Chunk this[int x, int y, int z]
            {
                get => this[new Vector3Int(x, y, z)];
                set => this[new Vector3Int(x, y, z)] = value;
            }

            public Chunk this[Vector3Int pos]
            {
                get => World._chunks.TryGetValue(pos, out var val) ? val : null;
                set => World._chunks[pos] = value;
            }
        }
    }
}