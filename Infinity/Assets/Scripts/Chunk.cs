using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        public static int Size = 16;

        private int[,,] Map;
        private Dictionary<int, BlockType> BlockPalette = new Dictionary<int, BlockType> { [0] = BlockTypes.Air };

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

            BlockPalette = newPalette;
        }

        public Mesh mesh;
        public List<Vector3> verts = new List<Vector3>();
        public List<int> tris = new List<int>();
        public List<Vector2> uv = new List<Vector2>();
        public MeshCollider meshCollider;

        public void Start()
        {
            Map = new int[Size, Size, Size];
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            meshCollider = GetComponent<MeshCollider>();
            RegenerateMesh();
        }

        private void RegenerateMesh()
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
                        int block = Map[x, y, z];
                        if (block == 0) continue;
                        DrawBlock(x, y, z, block);
                    }
                }
            }

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;

        }

        void DrawBlock(int x, int y, int z, int block)
        {
            Vector3 pos = new Vector3(x, y, z);
            Vector3 offset1, offset2;

            if(IsInvisible(x, y-1, z))
            {
                offset1 = Vector3.left;
                offset2 = Vector3.back;
                DrawIt(pos, offset1, offset2, block);
            }
            if (IsInvisible(x, y + 1, z))
            {
                offset1 = Vector3.right;
                offset2 = Vector3.back;
                DrawIt(pos, offset1, offset2, block);
            }
            if (IsInvisible(x - 1, y, z))
            {
                offset1 = Vector3.up;
                offset2 = Vector3.back;
                DrawIt(pos, offset1, offset2, block);
            }
            if (IsInvisible(x + 1, y, z))
            {
                offset1 = Vector3.down;
                offset2 = Vector3.back;
                DrawIt(pos, offset1, offset2, block);
            }
            if (IsInvisible(x, y, z-1))
            {
                offset1 = Vector3.left;
                offset2 = Vector3.up;
                DrawIt(pos, offset1, offset2, block);
            }
            if (IsInvisible(x, y, z+1))
            {
                offset1 = Vector3.right;
                offset2 = Vector3.up;
                DrawIt(pos, offset1, offset2, block);
            }
        }

        void DrawIt(Vector3 pos, Vector3 o1, Vector3 o2, int block)
        {
            verts.Add(pos);
            verts.Add(pos + o1);
            verts.Add(pos + o2);
            verts.Add(pos + o1 + o2);

            int index = verts.Count;
            tris.Add(index + 0);
            tris.Add(index + 1);
            tris.Add(index + 2);
            tris.Add(index + 3);
            tris.Add(index + 2);
            tris.Add(index + 1);
        }

        bool IsInvisible(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= Size || y >= Size || z >= Size) return true;
            return Map[x,y,z] == 0;
        }
    }
}