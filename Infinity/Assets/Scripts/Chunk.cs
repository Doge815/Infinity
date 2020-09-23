using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MeshFilter))]
    public class Chunk : MonoBehaviour
    {
        public static int Size = 16;

        public int[,,] Map;

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
                        BlockInfo block = Map[x, y, z];
                        if (block.ID == 0) continue;
                        DrawBlock((x, y,z));
                    }
                }
            }

        }

        void DrawBlock((int x, int y, int z) Position)
        {

        }


    }
}