using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class World : MonoBehaviour
    {
        public Chunk ChunkPrefab;
        public ChunkGenerator ChunkGenerator;

        private readonly Dictionary<Vector3Int, Chunk> _loadedChunks;

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

        private int FlooredIntDivision(int a, int b) => (a / b) - Convert.ToInt32((a < 0 ^ b < 0) && a % b != 0);

        public World()
        {
            _loadedChunks = new Dictionary<Vector3Int, Chunk>();
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

            public Chunk this[Vector3Int chunkIndex]
            {
                get => World._loadedChunks.TryGetValue(chunkIndex, out var val) ? val : World.SpawnChunk(chunkIndex);
                set => World._loadedChunks[chunkIndex] = value;
            }
        }

        public Chunk SpawnChunk(Vector3Int chunkIndex, bool draw = false)
        {
            var chunk = Instantiate(ChunkPrefab, chunkIndex * Chunk.Size, Quaternion.identity);

            _loadedChunks[chunkIndex] = chunk;

            return chunk;
        }
    }
}