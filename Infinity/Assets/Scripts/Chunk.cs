using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MeshFilter))]
    public class Chunk : MonoBehaviour
    {
        public static int Size = 16;

        private readonly int[,,] Map;
        private readonly Dictionary<int, BlockType> BlockPalette = new Dictionary<int, BlockType> { [0] = BlockTypes.Air };

        public BlockType this[int x, int y, int z]
        {
            get
            {
                var id = Map[x, y, z];
                return BlockPalette[id];
            }
            set
            {
                foreach (var pair in BlockPalette)
                {
                    if (pair.Value == value)
                    {
                        Map[x, y, z] = pair.Key;
                        return;
                    }
                }

                var newId = BlockPalette.Count;
                BlockPalette[newId] = value;
                Map[x, y, z] = newId;
            }
        }

        public void OptimizeBlockPalette()
        {
            var newPalette = new Dictionary<int, BlockType>();
            var idReassignments = new Dictionary<int, int>();

            for (int x = 0; x < Map.Length; x++)
            {
                for (int y = 0; y < Map.Length; y++)
                {
                    for (int z = 0; z < Map.Length; z++)
                    {
                        var id = Map[x, y, z];

                        if (newPalette.ContainsKey(id))
                        {
                            if (idReassignments.TryGetValue(id, out var newId)) Map[x, y, z] = newId;
                        }
                        else
                        {
                            var newId = newPalette.Count;
                            newPalette[newId] = BlockPalette[id];
                            if (newId != id) idReassignments[id] = newId;
                        }
                    }
                }
            }
        }

        public Mesh mesh;
        public List<Vector3> verts = new List<Vector3>();
        public List<int> tris = new List<int>();
        public List<Vector2> uv = new List<Vector2>();

        void RegenerateMesh()
        {
            verts.Clear();
            tris.Clear();
            uv.Clear();
            mesh.triangles = tris.ToArray();

            for(int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    for (int z = 0; z < Size; z++)
                    {
                        BlockType block = this[x, y, z];
                        if (block.Id == 0) continue;
                        DrawBlock(new Vector3(x, y, z), block);
                    }
                }
            }

        }

        void DrawBlock(Vector3 start, BlockType block)
        {
            Vector3 offset1, offset2;
        }

        bool NeedDraw(Vector3 pos)
        {
            if (pos[0] < 0 || pos[1] < 0 || pos[2] < 0) return false;
            return true;
        }


    }
}