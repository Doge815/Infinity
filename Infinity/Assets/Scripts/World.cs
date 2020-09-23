using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class World
    {
        public static World ActiveWorld;

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
                var chunkPosition = GetChunkPosition(x, y, z);
                return Chunks[chunkPosition][x - chunkPosition.x, y - chunkPosition.y, z - chunkPosition.z];
            }

            set
            {
                var chunkPosition = GetChunkPosition(x, y, z);
                Chunks[chunkPosition][x - chunkPosition.x, y - chunkPosition.y, z - chunkPosition.z] = value;
            }
        }

        public Vector3Int GetChunkPosition(Vector3Int pos) => GetChunkPosition(pos.x, pos.y, pos.z);
        public Vector3Int GetChunkPosition(int x, int y, int z) =>
            new Vector3Int(FlooredIntDivision(x, Chunk.Size.x), FlooredIntDivision(y, Chunk.Size.y), FlooredIntDivision(z, Chunk.Size.z));

        private int FlooredIntDivision(int a, int b) => (a / b) - Convert.ToInt32(((a < 0) ^ (b < 0)) && (a % b != 0));

        public World()
        {
            _chunks = new Dictionary<Vector3Int, Chunk>();
            Chunks = new ChunkIndexer(this);

            ActiveWorld = this;
        }

        public readonly ChunkIndexer Chunks;

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
                get => World._chunks[pos];
                set => World._chunks[pos] = value;
            }
        }
    }
}