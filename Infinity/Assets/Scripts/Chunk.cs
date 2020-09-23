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
        private readonly Dictionary<int, BlockType> BlockPalette = new Dictionary<int, BlockType> { [0] = BlockType.Air };

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