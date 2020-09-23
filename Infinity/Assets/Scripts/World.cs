using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class World
    {
        private Dictionary<Vector3Int, Chunk> _chunks;

        public BlockType this[Vector3Int pos]
        {
            get => this[pos.x, pos.y, pos.z];
            set => this[pos.x, pos.y, pos.z] = value;
        }

        public BlockType this[int x, int y, int z]
        {
            get => Chunks[GetChunkPosition(x, y, z)][x, y, z];
            set => Chunks[GetChunkPosition(x, y, z)][x, y, z] = value;
        }

        public Vector3Int GetChunkPosition(Vector3Int pos) => GetChunkPosition(pos.x, pos.y, pos.z);
        public Vector3Int GetChunkPosition(int x, int y, int z) => new Vector3Int(x / Chunk.Size.x, y / Chunk.Size.y, z / Chunk.Size.z);

        public readonly ChunkIndexer Chunks;

        public World()
        {
            _chunks = new Dictionary<Vector3Int, Chunk>();
            Chunks = new ChunkIndexer(this);
        }

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