using UnityEngine;
using System.Collections.Generic;

namespace BGE.Forms
{
    public static class WaitFor
    {
        public static System.Collections.IEnumerator Frames(int frameCount)
        {
            if (frameCount <= 0)
            {
                throw new System.ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
            }

            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }
    }

    class Tile
    {
        public GameObject theTile;
        public float creationTime;


        public Tile(GameObject t, float ct)
        {
            theTile = t;
            creationTime = ct;
        }
    }

    class GeneratedMesh
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uv;
        public Color[] colors;
        public int[] triangles;
    }

    public class WorldGenerator : MonoBehaviour {
        public GameObject player;

        public int cellsPerTile = 10;
        public int halfTile = 5;
        public float cellSize = 1.0f;
        Vector3 startPos;

        Dictionary<string,Tile> tiles = new Dictionary<string, Tile>();
        Sampler[] samplers;
        TextureGenerator textureGenerator;

        public bool drawGizmos = true;

        [Range(0.1f, 10.0f)]
        public float textureScaling = 1.0f;

        public Color color = Color.blue;

        public Sampler[] GetSamplers()
        {
            return samplers ?? (samplers = GetComponents<Sampler>());
        }
        
        void OnDrawGizmos()
        {
            if (drawGizmos )
            {
                samplers = GetSamplers();
                textureGenerator = GetComponent<TextureGenerator>();
                if (samplers == null)
                {
                    Debug.Log("Sampler is null! Add a sampler to the NoiseForm");
                }            
                int playerX = (int)(Mathf.Floor((player.transform.position.x) / (cellsPerTile * cellSize)) * cellsPerTile);
                int playerZ = (int)(Mathf.Floor((player.transform.position.z) / (cellsPerTile * cellSize)) * cellsPerTile);

                Gizmos.color = Color.cyan;
                int gizmoTiles = 4; 
                for (int x = -gizmoTiles; x < gizmoTiles; x++)
                {
                    for (int z = -gizmoTiles; z < gizmoTiles; z++)
                    {
                        Vector3 pos = new Vector3((x * cellsPerTile + playerX),
                            0,
                            (z * cellsPerTile + playerZ));
                        pos *= cellSize;
                        pos += transform.position;
                        Mesh gm = GenerateMesh(pos);
                        Gizmos.DrawMesh(gm, pos);
                        /*for (int i = 0; i < gm.vertices.Length; i += 2)
                    {
                        Gizmos.DrawLine(pos + gm.vertices[i], pos + gm.vertices[i + 1]);
                    }*/
                    }
                }
            }
        }

        void Awake()
        {
            samplers = GetSamplers();
            if (samplers == null)
            {
                Debug.Log("Sampler is null! Add a sampler to the NoiseForm");
            }
            Random.seed = 42;
        }


        // Use this for initialization
        void Start () 
        {
            //this.gameObject.transform.position = Vector3.zero;
            startPos = Vector3.zero;

            textureGenerator = GetComponent<TextureGenerator>();
        
            StartCoroutine(GenerateWorldAroundPlayer());
        }

        private System.Collections.IEnumerator GenerateWorldAroundPlayer()
        {
            yield return null;

            // Make sure this happens at once at the start
            int xMove = int.MaxValue;
            int zMove = int.MaxValue;

            while (true)
            {            
                if (Mathf.Abs(xMove) >= cellsPerTile * cellSize || Mathf.Abs(zMove) >= cellsPerTile * cellSize)
                {
                    float updateTime = Time.realtimeSinceStartup;

                    //force integer position and round to nearest tilesize
                    int playerX = (int)(Mathf.Floor((player.transform.position.x) / (cellsPerTile * cellSize)) * cellsPerTile);
                    int playerZ = (int)(Mathf.Floor((player.transform.position.z) / (cellsPerTile * cellSize)) * cellsPerTile);
                    List<Vector3> newTiles = new List<Vector3>();
                    for (int x = -halfTile; x < halfTile; x++)
                    {
                        for (int z = -halfTile; z < halfTile; z++)
                        {
                            Vector3 pos = new Vector3((x * cellsPerTile + playerX),
                                0,
                                (z * cellsPerTile + playerZ));
                            pos *= cellSize;
                            string tilename = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();                        
                            if (!tiles.ContainsKey(tilename))
                            {
                                newTiles.Add(pos);                            
                            }
                            else
                            {
                                (tiles[tilename] as Tile).creationTime = updateTime;
                            }
                        }
                    }
                    // Sort in order of distance from the player
                    newTiles.Sort((a, b) => (int) Vector3.SqrMagnitude(player.transform.position - a) - (int) Vector3.SqrMagnitude(player.transform.position - b));
                    foreach (Vector3 pos in newTiles)
                    {
                        GameObject t = GenerateTile(pos);
                        string tilename = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                        t.name = tilename;
                        Tile tile = new Tile(t, updateTime);
                        tiles[tilename] = tile;
                        yield return WaitFor.Frames(Random.Range(1, 3));
                    }

                    //destroy all tiles not just created or with time updated
                    //and put new tiles and tiles to be kepts in a new hashtable
                    Dictionary<string, Tile> newTerrain = new Dictionary<string, Tile>();
                    foreach (Tile tile in tiles.Values)
                    {
                        if (tile.creationTime != updateTime)
                        {
                            Debug.Log("Deleting tile: " + tile.theTile.name);
                            Destroy(tile.theTile);
                            yield return WaitFor.Frames(Random.Range(1, 3));
                        }
                        else
                        {
                            newTerrain[tile.theTile.name] = tile;
                        }
                    }
                    //copy new hashtable contents to the working hashtable
                    tiles = newTerrain;
                    startPos = player.transform.position;
                }
                yield return null;
                //determine how far the player has moved since last terrain update
                xMove = (int)(player.transform.position.x - startPos.x);
                zMove = (int)(player.transform.position.z - startPos.z);
            }
        }

        public float SamplePos(float x, float y)
        {
            return SampleCell(x / cellSize, y / cellSize);
        }

        public float SampleCell(float x, float y)
        {
            float sample = 0;


            foreach (Sampler sampler in GetSamplers())
            {
                sample = sampler.Operate(sample, x, y);
            }
            return sample;
        }

        Mesh GenerateMesh(Vector3 position)
        {

            int verticesPerSegment = 6;

            int vertexCount = verticesPerSegment * ((int)cellsPerTile) * ((int)cellsPerTile);
        
            int vertex = 0;
            // What cell is x and z for the bottom left of this tile in world space
            Vector3 tileBottomLeft = new Vector3();
            tileBottomLeft.x = -(cellsPerTile * cellSize) / 2;
            tileBottomLeft.z = -(cellsPerTile * cellSize) / 2;
        
            GeneratedMesh gm = new GeneratedMesh();

            gm.vertices = new Vector3[vertexCount];
            gm.normals = new Vector3[vertexCount];
            gm.uv = new Vector2[vertexCount];
            gm.triangles = new int[vertexCount];
            gm.colors = new Color[vertexCount];

            Vector2 texOrigin = position / cellSize;
            texOrigin.x = texOrigin.x % textureGenerator.size;
            texOrigin.y = texOrigin.y % textureGenerator.size;

            float tilesPerTexture = textureGenerator.size / cellsPerTile;

            for (int z = 0; z < cellsPerTile; z++)
            {
                for (int x = 0; x < cellsPerTile; x++)
                {
                    int startVertex = vertex;

                    // Cell bottom left is the 0 indexed position of the cell on a tile
                    // Kinda like local space on the tile       
                    // We neeed these to make the vertices out of
                    Vector3 cellBottomLeft = tileBottomLeft + new Vector3(x * cellSize, 0, z * cellSize);
                    Vector3 cellTopLeft = tileBottomLeft + new Vector3(x * cellSize, 0, ((z + 1) * cellSize));
                    Vector3 cellTopRight = tileBottomLeft + new Vector3((x + 1) * cellSize, 0, (z + 1) * cellSize);
                    Vector3 cellBottomRight = tileBottomLeft + new Vector3((x + 1) * cellSize, 0, z * cellSize);

                    // Add all the samplers together to make the height
                    // Cell is the absolute position of the cell in world space. I.e what gets sampled
                    Vector3 cellLeft = new Vector3(-cellsPerTile / 2, 0, -cellsPerTile / 2);
                    Vector3 cell = (position / cellSize) + cellLeft + new Vector3(x, 0, z);
                    cellBottomLeft.y = SampleCell(cell.x, cell.z);
                    cellTopLeft.y = SampleCell(cell.x, cell.z + 1);
                    cellTopRight.y = SampleCell(cell.x + 1, cell.z + 1);
                    cellBottomRight.y = SampleCell(cell.x + 1, cell.z);

                    // Make the vertices
                    gm.vertices[vertex++] = cellBottomLeft;
                    gm.vertices[vertex++] = cellTopLeft;
                    gm.vertices[vertex++] = cellTopRight;
                    gm.vertices[vertex++] = cellTopRight;
                    gm.vertices[vertex++] = cellBottomRight;
                    gm.vertices[vertex++] = cellBottomLeft;

                    vertex = startVertex;
                    gm.uv[vertex++] = MakeUV(position, x, z);
                    gm.uv[vertex++] = MakeUV(position, x, z + 1);
                    gm.uv[vertex++] = MakeUV(position, x + 1, z + 1);
                    gm.uv[vertex++] = MakeUV(position, x + 1, z + 1);
                    gm.uv[vertex++] = MakeUV(position, x + 1, z);
                    gm.uv[vertex++] = MakeUV(position, x, z);
             

                    // Make the triangles                
                    for (int i = 0; i < 6; i++)
                    {
                        int vertexIndex = startVertex + i;
                        gm.triangles[vertexIndex] = vertexIndex;
                        gm.colors[vertexIndex] = color;

                    }
                }
            }
            Mesh mesh = new Mesh();
            mesh.vertices = gm.vertices;
            mesh.uv = gm.uv;
            mesh.triangles = gm.triangles;
            mesh.colors = gm.colors;
            mesh.RecalculateNormals();

            return mesh;
        }

        private Vector2 MakeUV(Vector3 tilePos, float x, float z)
        {
        
            // Convert the actual position to a cell position
            Vector2 cellPos = new Vector2(((tilePos.x / cellSize) % (textureGenerator.size + 1)) + x
                , ((tilePos.z / cellSize) % (textureGenerator.size + 1)) + z);
            cellPos /= textureScaling;
            Vector2 uv = new Vector2((cellPos.x) / (float) textureGenerator.size, (cellPos.y) / (float) textureGenerator.size);
            return uv;
        }

        GameObject GenerateTile(Vector3 position)
        {
            GameObject tile = new GameObject();
            tile.layer = this.gameObject.layer;
            tile.transform.parent = this.transform;
            MeshRenderer renderer = tile.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            MeshCollider meshCollider = tile.AddComponent<MeshCollider>();        
            tile.transform.position = position + transform.position;

            Rigidbody rigidBody = tile.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;

            MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
            Mesh mesh = GenerateMesh(position);
            meshFilter.mesh = mesh;
            renderer.material.SetTexture("_MainTex", textureGenerator.texture);
            //renderer.material.color = color; //  new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            /*Shader shader = Shader.Find("Diffuse");

        Material material = null;
        if (renderer.material == null)
        {
            material = new Material(shader);
            renderer.material = material;
        }
        */
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;

            return tile;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}