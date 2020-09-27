using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            public Chunk GetOrSpawn(Vector3Int chunkIndex, bool wake = false)
            {
                var chunk = this[chunkIndex];

                chunk = chunk != null ? chunk : World.SpawnChunk(chunkIndex);

                if (wake) chunk.gameObject.SetActive(true);

                return chunk;
            }

            public IEnumerable<Chunk> GetOrSpawnArea(Vector3Int chunkIndex, int chunkIndexDistance, bool wake = false)
            {
                var _this = this;
                for (int x = -chunkIndexDistance; x <= chunkIndexDistance; x++)
                {
                    for (int y = -chunkIndexDistance; y <= chunkIndexDistance; y++)
                    {
                        Parallel.For(-chunkIndexDistance, chunkIndexDistance + 1, z =>
                        {
                            _this.GetOrSpawn(chunkIndex + new Vector3Int(x, y, z), wake);
                        });
                    }
                }
            }
        }

        private Chunk SpawnChunk(Vector3Int chunkIndex)
        {
            var chunk = Instantiate(ChunkPrefab, chunkIndex * Chunk.Size, Quaternion.identity, transform);

            chunk.World = this;
            _loadedChunks[chunkIndex] = chunk;

            return chunk;
        }

        public override string ToString() => $"World {{ {_loadedChunks.Count} Loaded Chunks, ChunkPrefab = {ChunkPrefab}, ChunkGenerator = {ChunkGenerator} }}";
    }
}