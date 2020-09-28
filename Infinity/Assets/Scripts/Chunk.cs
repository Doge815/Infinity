using Assets.Players;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class Chunk : MonoBehaviour
    {
        public static Vector3Int Size = new Vector3Int(16, 16, 16);

        /// <summary>
        /// The position of the chunk in the world, measured from the local block at (0, 0, 0).
        /// </summary>
        public Vector3Int WorldPosition;

        /// <summary>
        /// The block types of all blocks in the chunk.
        /// </summary>
        public BlockType[,,] Map;

        public World World;

        public BlockType this[Vector3Int pos]
        {
            get => this[pos.x, pos.y, pos.z];
            set => this[pos.x, pos.y, pos.z] = value;
        }

        public BlockType this[int x, int y, int z]
        {
            get => Map?[x, y, z];
            set
            {
                if (Map[x, y, z] == value) return;
                RedrawRequired = true;
                Map[x, y, z] = value;
            }
        }

        public BlockType GetBlockTypeSafe(Vector3Int pos) => GetBlockTypeSafe(pos.x, pos.y, pos.z);

        public BlockType GetBlockTypeSafe(int x, int y, int z) =>
            x < 0 || y < 0 || z < 0 || x >= Size.x || y >= Size.y || z >= Size.z
            ? null
            : this[x, y, z];

        // TODO: Use events
        public bool RedrawRequired = false;

        [HideInInspector]
        public Mesh mesh;
        [HideInInspector]
        public List<Vector3> verts = new List<Vector3>();
        [HideInInspector]
        public List<int> tris = new List<int>();
        [HideInInspector]
        public List<Vector2> uv = new List<Vector2>();
        [HideInInspector]
        public MeshFilter meshFilter;
        [HideInInspector]
        public MeshCollider meshCollider;

        public void Start()
        {
            WorldPosition = transform.position.ToVector3Int();

            World.Chunks[World.GetChunkIndex(WorldPosition)] = this;

            Task.Run(() => World.ChunkGenerator.Populate(this));

            mesh = new Mesh();

            meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            meshCollider = GetComponent<MeshCollider>();
        }

        public void Update()
        {
            if (!Application.isPlaying) return;

            if (RedrawRequired)
            {
                RegenerateMesh();
                RedrawRequired = false;
            }

            var cameraDistance = (Camera.main.transform.position.ToVector3Int() - WorldPosition).Absolute();
            var renderDistance = Player.Active != null ? Player.Active.RenderDistance : 2;

            if (cameraDistance.x - (Size.x / 2) > renderDistance * Size.x
                || cameraDistance.y - (Size.y / 2) > renderDistance * Size.y
                || cameraDistance.z - (Size.z / 2) > renderDistance * Size.z)
            {
                gameObject.SetActive(false);
            }
        }

        public void RegenerateMesh()
        {
            verts.Clear();
            tris.Clear();
            uv.Clear();

            for (int side = 0; side < 6; side++) DrawSide(side % 3, side >= 3);

            if (mesh == null) mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            verts.Clear();
            tris.Clear();
            uv.Clear();

            this.Invoke(() =>
            {
                meshFilter.sharedMesh = mesh;
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            });
        }

        private void DrawSide(int sideI, bool isNegated)
        {
            var sideJ = (sideI + 1) % 3;
            var sideK = (sideI + 2) % 3;

            Vector3Int GetIndex(int i, int j, int k) => new Vector3Int
            {
                [sideI] = i,
                [sideJ] = j,
                [sideK] = k,
            };

            for (int height = 0; height <= Size[sideI]; height++)
            {
                var faces = new BlockType[Size[sideJ], Size[sideK]];

                for (int j = 0; j < Size[sideJ]; j++)
                {
                    for (int k = 0; k < Size[sideK]; k++)
                    {
                        faces[j, k] = IsVisible(GetIndex(height + (isNegated ? -1 : 1), j, k)) ? null : GetBlockTypeSafe(GetIndex(height, j, k));
                    }
                }

                while (true)
                {
                    int startJ, startK;

                    for (int j = 0; j < Size[sideJ]; j++)
                    {
                        for (int k = 0; k < Size[sideK]; k++)
                        {
                            if (faces[j, k] != null)
                            {
                                startJ = j;
                                startK = k;

                                goto FindStartJKExit;
                            }
                        }
                    }
                    break;
                FindStartJKExit:

                    int endJ = Size[sideJ], endK = Size[sideK];

                    for (int j = startJ + 1; j < endJ; j++)
                    {
                        if (faces[j, startK] == null)
                        {
                            endJ = j;
                            break;
                        }
                    }

                    for (int k = startK + 1; k < endK; k++)
                    {
                        for (int j = startJ; j < endJ; j++)
                        {
                            if (faces[j, k] == null)
                            {
                                endK = k;
                                break;
                            }
                        }
                    }

                    for (int j = startJ; j < endJ; j++)
                    {
                        for (int k = startK; k < endK; k++)
                        {
                            faces[j, k] = null;
                        }
                    }

                    var start = GetIndex(height, startJ, startK);
                    DrawQuad(start + (GetIndex(isNegated ? -1 : 1, -1, -1).ToVector3() / 2f),
                        GetIndex(0, endJ - startJ, 0), GetIndex(0, 0, endK - startK));
                }
            }
        }

        public void OnDrawGizmosSelected()
        {
            if (RedrawRequired)
            {
                Task.Run(() =>
                {
                    World.ChunkGenerator.Populate(this);
                    RegenerateMesh();
                });
                RedrawRequired = false;
            }

            for (int side = 0; side < 6; side++) DrawSide(side % 3, side >= 3);

            void DrawSide(int sideI, bool isNegated)
            {
                var sideJ = (sideI + 1) % 3;
                var sideK = (sideI + 2) % 3;

                Vector3Int GetIndex(int i, int j, int k) => new Vector3Int
                {
                    [sideI] = i,
                    [sideJ] = j,
                    [sideK] = k,
                };

                for (int height = 0; height <= Size[sideI]; height++)
                {
                    var faces = new BlockType[Size[sideJ], Size[sideK]];

                    for (int j = 0; j < Size[sideJ]; j++)
                    {
                        for (int k = 0; k < Size[sideK]; k++)
                        {
                            faces[j, k] = IsVisible(GetIndex(height + (isNegated ? -1 : 1), j, k)) ? null : GetBlockTypeSafe(GetIndex(height, j, k));

                            if (faces[j, k] == null) continue;
                            var pos = GetIndex(height, j, k);

                            Gizmos.color = new Color(1, 0, 0, 1f);
                            Gizmos.DrawWireCube(
                                WorldPosition + pos + (GetIndex(isNegated ? -1 : 1, 0, 0).ToVector3() / 2f),
                                GetIndex(0, 1, 1));
                            Gizmos.color = Color.black;
                        }
                    }

                    while (true)
                    {
                        int startJ, startK;

                        for (int j = 0; j < Size[sideJ]; j++)
                        {
                            for (int k = 0; k < Size[sideK]; k++)
                            {
                                if (faces[j, k] != null)
                                {
                                    startJ = j;
                                    startK = k;

                                    goto FindStartJKExit;
                                }
                            }
                        }
                        break;
                    FindStartJKExit:

                        int endJ = Size[sideJ], endK = Size[sideK];

                        for (int j = startJ + 1; j < endJ; j++)
                        {
                            if (faces[j, startK] == null)
                            {
                                endJ = j;
                                break;
                            }
                        }

                        for (int k = startK + 1; k < endK; k++)
                        {
                            for (int j = startJ; j < endJ; j++)
                            {
                                if (faces[j, k] == null)
                                {
                                    endK = k;
                                    break;
                                }
                            }
                        }

                        for (int j = startJ; j < endJ; j++)
                        {
                            for (int k = startK; k < endK; k++)
                            {
                                faces[j, k] = null;
                            }
                        }

                        var start = GetIndex(height, startJ, startK);
                        var size = GetIndex(0, endJ - startJ, endK - startK);
                        Gizmos.DrawWireCube(
                            WorldPosition + start + (size.ToVector3() / 2f) + (GetIndex(isNegated ? -1 : 1, -1, -1).ToVector3() / 2f),// - (Vector3.one / 2f),
                            size);
                    }
                }
            }
        }

        private void DrawQuad(Vector3 origin, Vector3 offset1, Vector3 offset2)
        {
            var index = verts.Count;

            verts.Add(origin);
            verts.Add(origin + offset1);
            verts.Add(origin + offset2);
            verts.Add(origin + offset1 + offset2);

            tris.Add(index + 0);
            tris.Add(index + 1);
            tris.Add(index + 2);
            tris.Add(index + 3);
            tris.Add(index + 2);
            tris.Add(index + 1);
        }

        private bool IsVisible(Vector3Int pos) => IsVisible(pos.x, pos.y, pos.z);

        private bool IsVisible(int x, int y, int z)
        {
            return x < 0 || x >= Size.x
                || y < 0 || y >= Size.y
                || z < 0 || z >= Size.z
                || Map?[x, y, z] != null;
        }
    }
}