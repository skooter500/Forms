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
        public int[] triangles;

        public GeneratedMesh(int vertexCount, int triangleCount)
        {
            vertices = new Vector3[vertexCount];
            normals = new Vector3[vertexCount];
            uv = new Vector2[vertexCount];
            triangles = new int[triangleCount];
        }
    }

    public class WorldGenerator : MonoBehaviour {

        public static WorldGenerator Instance;
        
        public GameObject player;

        public int quadsPerTile = 10;
        public int halfTile = 5;
        public float cellSize = 1.0f;
        Vector3 startPos;

        Dictionary<string,Tile> tiles = new Dictionary<string, Tile>();
        Sampler[] samplers;
        GameOfLifeTextureGenerator textureGenerator;

        public bool drawGizmos = true;

        [Range(0.001f, 10.0f)]
        public float textureScaling = 1.0f;

        public Color color = Color.blue;

        public float surfaceHeight = 6000;

        public GameObject cannibisPrefab;


        public Material[] groundMaterials;
        public int groundMaterialIndex = 0;
        public Material ceilingMaterial;

        public int ceilingLayer= 20;
        

        public Sampler[] GetSamplers()
        {
            return samplers ?? (samplers = GetComponents<Sampler>());
        }
        
        void OnDrawGizmos()
        {
            if (drawGizmos )
            {
                samplers = GetSamplers();
                textureGenerator = GetComponent<GameOfLifeTextureGenerator>();
                if (samplers == null)
                {
                    Debug.Log("Sampler is null! Add a sampler to the NoiseForm");
                }            
                int playerX = (int)(Mathf.Floor((player.transform.position.x) / (quadsPerTile * cellSize)) * quadsPerTile);
                int playerZ = (int)(Mathf.Floor((player.transform.position.z) / (quadsPerTile * cellSize)) * quadsPerTile);

                Gizmos.color = Color.cyan;
                int gizmoTiles = 4; 
                for (int x = -gizmoTiles; x < gizmoTiles; x++)
                {
                    for (int z = -gizmoTiles; z < gizmoTiles; z++)
                    {
                        Vector3 pos = new Vector3((x * quadsPerTile + playerX),
                            0,
                            (z * quadsPerTile + playerZ));
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

        void GenerateFlora(GameObject tile)
        {
            if (Random.Range(0.0f, 1.0f) < 0.1)
            {
                GameObject can = GameObject.Instantiate<GameObject>(cannibisPrefab);
                float y = SamplePos(tile.transform.position.x, tile.transform.position.z);
                can.transform.position = new Vector3(tile.transform.position.x, y + 250, tile.transform.position.z);
                can.transform.parent = tile.transform;
                float r = 5;
                can.transform.rotation = Quaternion.Euler(
                    Random.Range(-r, r)
                    , Random.Range(0, 360)
                    , Random.Range(-r, r)
                    );
                //float scale = Random.Range(1.5f, 4.0f);
                //can.transform.localScale = new Vector3(scale, scale, scale);
                can.SetActive(true);
                can.isStatic = true;
            }
        }

        void Awake()
        {
            Instance = this;
            samplers = GetSamplers();

        

            if (samplers == null)
            {
                Debug.Log("Sampler is null! Add a sampler to the NoiseForm");
            }
        
            Random.seed = (int)System.DateTime.Now.Ticks;
            foreach (Sampler s in samplers)
            {
                ((PerlinNoiseSampler)s).origin = Random.Range(-1000, 1000);
            }
            

            /*
            ((PerlinNoiseSampler)samplers[0]).origin = 750;
            ((PerlinNoiseSampler)samplers[1]).origin = -747;
            ((PerlinNoiseSampler)samplers[2]).origin = 113;
            */
            player.transform.position = new Vector3(0, SamplePos(0,0) + 500, 0);
            
            //Random.seed = 42;
        }


        // Use this for initialization
        void Start () 
        {
            //this.gameObject.transform.position = Vector3.zero;
            startPos = Vector3.zero;

            textureGenerator = GetComponent<GameOfLifeTextureGenerator>();
        
            StartCoroutine(GenerateWorldAroundPlayer());
        }

        Queue<GameObject> oldGameObjects = new Queue<GameObject>();

        public bool forceCheck = false;

        public void ForceCheck()
        {
            forceCheck = true;
        }

        private System.Collections.IEnumerator GenerateWorldAroundPlayer()
        {
            yield return null;

            // Make sure this happens at once at the start
            int xMove = int.MaxValue;
            int zMove = int.MaxValue;

            player = Camera.main.gameObject;

            while (true)
            {
                if (oldGameObjects.Count > 0)
                {
                    GameObject.Destroy(oldGameObjects.Dequeue());
                }
                if (Mathf.Abs(xMove) >= quadsPerTile * cellSize || Mathf.Abs(zMove) >= quadsPerTile * cellSize || forceCheck)
                {
                    forceCheck = false;
                    float updateTime = Time.realtimeSinceStartup;
                    
                    //force integer position and round to nearest tilesize
                    int playerX = (int)(Mathf.Floor((player.transform.position.x) / (quadsPerTile * cellSize)) * quadsPerTile);
                    int playerZ = (int)(Mathf.Floor((player.transform.position.z) / (quadsPerTile * cellSize)) * quadsPerTile);
                    List<Vector3> newTiles = new List<Vector3>();
                    for (int x = -halfTile; x < halfTile; x++)
                    {
                        for (int z = -halfTile; z < halfTile; z++)
                        {
                            Vector3 pos = new Vector3((x * quadsPerTile + playerX),
                                0,
                                (z * quadsPerTile + playerZ));
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
                        StartCoroutine(ChangeMaterialToOpaque(t, 4));
                        //StaticBatchingUtility.Combine(this.gameObject);
                        yield return WaitFor.Frames(Random.Range(5, 10));
                    }

                    //destroy all tiles not just created or with time updated
                    //and put new tiles and tiles to be kepts in a new hashtable
                    Dictionary<string, Tile> newTerrain = new Dictionary<string, Tile>();
                    foreach (Tile tile in tiles.Values)
                    {
                        if (tile.creationTime != updateTime)
                        {
                            oldGameObjects.Enqueue(tile.theTile);
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

        System.Collections.IEnumerator ChangeMaterialToOpaque(GameObject root, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (root != null)
            {
                foreach (Renderer r in root.GetComponentsInChildren<Renderer>())
                {
                    Utilities.SetupMaterialWithBlendMode(r.material, BlendMode.Opaque);
                }
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

            int triangleCount = verticesPerSegment * ((int)quadsPerTile) * ((int)quadsPerTile);
            int vertexCount = 4 * ((int)quadsPerTile) * ((int)quadsPerTile);

            int vertex = 0;
            // What cell is x and z for the bottom left of this tile in world space
            Vector3 tileBottomLeft = new Vector3();
            tileBottomLeft.x = -(quadsPerTile * cellSize) / 2;
            tileBottomLeft.z = -(quadsPerTile * cellSize) / 2;
        
            GeneratedMesh gm = new GeneratedMesh(vertexCount, triangleCount);
            
            Vector2 texOrigin = position / cellSize;
            texOrigin.x = texOrigin.x % textureGenerator.size;
            texOrigin.y = texOrigin.y % textureGenerator.size;

            float tilesPerTexture = textureGenerator.size / quadsPerTile;
            int triangleVertex = 0;
            for (int z = 0; z < quadsPerTile; z++)
            {
                for (int x = 0; x < quadsPerTile; x++)
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
                    Vector3 cellLeft = new Vector3(-quadsPerTile / 2, 0, -quadsPerTile / 2);
                    Vector3 cell = (position / cellSize) + cellLeft + new Vector3(x, 0, z);
                    cellBottomLeft.y = SampleCell(cell.x, cell.z);
                    cellTopLeft.y = SampleCell(cell.x, cell.z + 1);
                    cellTopRight.y = SampleCell(cell.x + 1, cell.z + 1);
                    cellBottomRight.y = SampleCell(cell.x + 1, cell.z);

                    // Make the vertices
                    startVertex = vertex;
                    gm.vertices[vertex++] = cellBottomLeft;
                    gm.vertices[vertex++] = cellTopLeft;
                    gm.vertices[vertex++] = cellTopRight;
                    gm.vertices[vertex++] = cellBottomRight;
                    
                    vertex = startVertex;
                    gm.uv[vertex++] = MakeUV(position, x, z);
                    gm.uv[vertex++] = MakeUV(position, x, z + 1);
                    gm.uv[vertex++] = MakeUV(position, x + 1, z + 1);
                    gm.uv[vertex++] = MakeUV(position, x + 1, z);

                    gm.triangles[triangleVertex++] = startVertex;
                    gm.triangles[triangleVertex++] = startVertex + 1;
                    gm.triangles[triangleVertex++] = startVertex + 3;
                    gm.triangles[triangleVertex++] = startVertex + 3;
                    gm.triangles[triangleVertex++] = startVertex + 1;
                    gm.triangles[triangleVertex++] = startVertex + 2;                }
            }
            Mesh mesh = new Mesh();
            mesh.vertices = gm.vertices;
            mesh.uv = gm.uv;
            mesh.triangles = gm.triangles;
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
            renderer.enabled = true;
            tile.SetActive(true);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            MeshCollider meshCollider = tile.AddComponent<MeshCollider>();        
            tile.transform.position = position + transform.position;

            Rigidbody rigidBody = tile.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;

            MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
            Mesh mesh = GenerateMesh(position);
            meshFilter.mesh = mesh;
            
            renderer.material = groundMaterials[groundMaterialIndex];
            renderer.material.SetTexture("_MainTex", textureGenerator.texture);
            renderer.material.SetTexture("_EmissionMap", textureGenerator.texture);
            Utilities.SetupMaterialWithBlendMode(renderer.material, BlendMode.Transparent);
            

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

            GameObject surface = MakeSurface(position);
            surface.transform.parent = tile.transform;
            surface.transform.localPosition = new Vector3(0, surfaceHeight, 0);
            tile.isStatic = true;
            surface.isStatic = true;
            //surface.SetActive(false);
            return tile;
        }

        GameObject MakeSurface(Vector3 position)
        {
            

            // Make the surface
            GameObject surface = new GameObject();
            MeshRenderer mr = surface.AddComponent<MeshRenderer>();
            MeshFilter mf = surface.AddComponent<MeshFilter>();
            

            Vector3 tileBottomLeft = new Vector3();
            tileBottomLeft.x = -(quadsPerTile * cellSize) / 2;
            tileBottomLeft.z = -(quadsPerTile * cellSize) / 2;

            Vector3 tileTopRight = new Vector3();
            tileTopRight.x = (quadsPerTile * cellSize) / 2;
            tileTopRight.z = (quadsPerTile * cellSize) / 2;

            GeneratedMesh gm = new GeneratedMesh(6,6);
            
            gm.vertices[0] = tileTopRight;
            gm.vertices[1] = new Vector3(tileBottomLeft.x, 0, tileTopRight.z);
            gm.vertices[2] = tileBottomLeft;
            gm.vertices[3] = new Vector3(tileTopRight.x, 0, tileBottomLeft.z);
            gm.vertices[4] = tileTopRight;
            gm.vertices[5] = tileBottomLeft;

            for (int i = 0; i < 6; i++)
            {
                gm.triangles[i] = i;
            }

            gm.uv[0] = MakeUV(position, quadsPerTile, quadsPerTile);
            gm.uv[1] = MakeUV(position, 0, quadsPerTile);
            gm.uv[2] = MakeUV(position, 0, 0);
            gm.uv[3] = MakeUV(position, quadsPerTile, 0);
            gm.uv[4] = MakeUV(position, quadsPerTile, quadsPerTile);
            gm.uv[5] = MakeUV(position, 0, 0);

            Mesh mesh = new Mesh();
            mesh.vertices = gm.vertices;
            mesh.uv = gm.uv;
            mesh.triangles = gm.triangles;
            mesh.RecalculateNormals();
            
            mf.mesh = mesh;

            mr.castShadows = false;
            mr.receiveShadows = false;
            mr.material = ceilingMaterial;
            mr.material.SetTexture("_MainTex", textureGenerator.texture);
            mr.material.SetTexture("_EmissionMap", textureGenerator.texture);
            //Utilities.SetupMaterialWithBlendMode(mr.material, BlendMode.Transparent);
            
            surface.AddComponent<MeshCollider>().sharedMesh = mesh;
            //Utilities.SetupMaterialWithBlendMode(mr.material, BlendMode.Transparent);
            surface.layer = ceilingLayer;
            surface.name = "surface";
            return surface;
        }

        // Update is called once per frame
        void Update()
        {
            Shader.SetGlobalFloat("_HeightOffset", Time.time/ 10.0f);

            float y = Input.GetAxis("DPadY");

            if (y == 1 && y != lastY)
            {
                topD = (topD + 1) % groundMaterials.Length;
                ellapsed = 0;
            }
            lastY = y;

            if (topD != -1 && ellapsed > toPass)
            {
                SetGroundMaterialASync(topD);                
                topD = -1;
            }
            ellapsed += Time.deltaTime;
        }

        public void SetGroundMaterialASync(int index)
        {
            StartCoroutine(SetGroundMaterial(index));
        }

        System.Collections.IEnumerator SetGroundMaterial(int index)
        {
            groundMaterialIndex = index;
            Debug.Log("Changing ground material to: " + groundMaterials[groundMaterialIndex]);            
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                if (r.name.StartsWith("Tile"))
                {
                    Texture t = null;
                    switch (index)
                    {
                        case 0:
                            t = textureGenerator.texture;
                            break;
                        case 1:
                            t = textureGenerator.secondaryTexture;
                            break;
                        case 2:
                            t = textureGenerator.texture;
                            break;
                        case 3:
                            t = textureGenerator.secondaryTexture;
                            break;
                    }
                    r.material = groundMaterials[groundMaterialIndex];
                    r.material.SetTexture("_MainTex", t);
                    r.material.SetTexture("_EmissionMap", t);
                    yield return null;
                }
            }                   
        }

        public float ellapsed = 0;
        public float toPass = 0.3f;
        public int topD = -1;
        float lastY;


    }
}