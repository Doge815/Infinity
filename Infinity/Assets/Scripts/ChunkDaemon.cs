using UnityEngine;

namespace Assets.Scripts
{
    [DefaultExecutionOrder(-90)]
    public class ChunkDaemon : MonoBehaviour
    {
        public GameObject _chunkPrefab;
        public static GameObject ChunkPrefab { get; private set; }

        public void Awake() => ChunkPrefab = _chunkPrefab;

        public static GameObject SpawnChunk(Vector3Int ChunkPosition)
        {
            return Instantiate(ChunkPrefab, ChunkPosition * Chunk.Size, Quaternion.identity);
        }
    }
}