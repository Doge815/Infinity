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

        public Vector3Int GetChunkIndex(Vector3Int worldPosition) => GetChunkIndex(worldPosition.x, worldPosition.y, worldPosition.z);
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
                get => World._loadedChunks.TryGetValue(chunkIndex, out var val) ? val : null;
                set => World._loadedChunks[chunkIndex] = value;
            }

            public Chunk GetOrSpawn(Vector3Int chunkIndex, bool draw = false)
            {
                var chunk = this[chunkIndex];

                return chunk != null ? chunk : World.SpawnChunk(chunkIndex, draw);
            }
        }

        public Chunk SpawnChunk(Vector3Int chunkIndex, bool draw = false)
        {
            var chunk = Instantiate(ChunkPrefab, chunkIndex * Chunk.Size, Quaternion.identity);

            chunk.World = this;
            _loadedChunks[chunkIndex] = chunk;
            chunk.RegenerateMesh();

            return chunk;
        }

        private IEnumerable<Chunk> GetOrSpawnChunksCore(Vector3Int chunkIndex, int chunkIndexDistance)
        {
            for (int x = -chunkIndexDistance; x <= chunkIndexDistance; x++)
            {
                for (int y = -chunkIndexDistance; y <= chunkIndexDistance; y++)
                {
                    for (int z = -chunkIndexDistance; z <= chunkIndexDistance; z++)
                    {
                        yield return Chunks.GetOrSpawn(chunkIndex + new Vector3Int(x, y, z), draw: false);
                    }
                }
            }
        }

        public IEnumerable<Chunk> GetOrSpawnChunks(Vector3Int chunkIndex, int chunkIndexDistance, bool draw = false)
        {
            foreach (var elem in GetOrSpawnChunksCore(chunkIndex, chunkIndexDistance))
            {
                if (draw)
                {
                    elem.RegenerateMesh();
                }

                yield return elem;
            }
        }

        public override string ToString() => $"World {{ {_loadedChunks.Count} Loaded Chunks, ChunkPrefab = {ChunkPrefab}, ChunkGenerator = {ChunkGenerator} }}";
    }
}