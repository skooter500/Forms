using UnityEngine;
using System.Collections.Generic;

namespace BGE.Forms
{
    class Baby
    {
        public GameObject baby;
        public Boid boid;
        public Vector3 key;
        public float creationTime;
        
        public Baby(GameObject t, float ct)
        {
            baby = t;
            boid = null; // Utilities.FindBoidInHierarchy(baby);
            key = t.transform.position;
            creationTime = ct;
        }
    }
    public class Mother : MonoBehaviour
    {
        public int gridWidth = 4;
        public float density = 0.2f;
        public float playerMaxDistance = 20000;

        public GameObject[] prefabs;
        public GameObject player;
        private WorldGenerator wg;
        // Use this for 

        private Dictionary<Vector3, Baby> wakingBabies = new Dictionary<Vector3, Baby>();
        private Dictionary<Vector3, Baby> sleepingBabies = new Dictionary<Vector3, Baby>();

        Vector3 startPos;

        

        void Awake()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            player = GameObject.FindWithTag("Player");
            wg = FindObjectOfType<WorldGenerator>();

            if (player == null)
            {
                Debug.Log("No player!");
                return;
            }
            System.Random r = new System.Random(42);

            StartCoroutine(GenerateCreaturesAroundPlayer());

            /*
            float cellWidth = playerMaxDistance/gridWidth;
            float c = playerMaxDistance/2.0f;
            int dice = (int)Utilities.RandomRange(r, 0, prefabs.Length);
            for (int row = 0; row < gridWidth; row++)
            {
                for (int col = 0; col < gridWidth; col++)
                {
                    if (r.NextDouble() > density)
                    {
                        break;
                    }
                    
                    Vector3 spawnPos = new Vector3();
                    spawnPos.x = -c + player.transform.position.x + (col*cellWidth);
                    spawnPos.z = -c + player.transform.position.z + (row*cellWidth);

                    if (Vector3.Distance(player.transform.position
                            , spawnPos) > playerMaxDistance)
                    {
                        break;
                    }

                    Vector3 cell = spawnPos/wg.cellSize;
                    if (wg != null)
                    {
                        spawnPos.y = Utilities.RandomRange(r, 200, 1000) + wg.Sample(cell.x, cell.z);
                    }
                    
                    GameObject go = GameObject.Instantiate(prefabs[dice % prefabs.Length]);
                    go.transform.position = spawnPos;
                    wakingBabies[dice] = go;
                    go.transform.parent = this.transform;
                    dice++;
                }
            }
            */
        }

        void Start () {
	        
        }



        private System.Collections.IEnumerator GenerateCreaturesAroundPlayer()
        {
            yield return null;

            // Make sure this happens at once at the start
            int xMove = int.MaxValue;
            int zMove = int.MaxValue;

            float tileSize = playerMaxDistance / gridWidth;

            while (true)
            {
                if (Mathf.Abs(xMove) >= tileSize || Mathf.Abs(zMove) >= tileSize)
                {
                    float updateTime = Time.realtimeSinceStartup;

                    //force integer position and round to nearest tilesize
                    int playerX = (int)(Mathf.Floor((player.transform.position.x) / (tileSize)) * tileSize);
                    int playerZ = (int)(Mathf.Floor((player.transform.position.z) / (tileSize)) * tileSize);
                    List<Vector3> newBabies = new List<Vector3>();
                    int halfGridWidth = gridWidth / 2;
                    for (int col = - halfGridWidth; col <= halfGridWidth; col ++)
                    {
                        for (int row = -halfGridWidth; row <= halfGridWidth; row ++)
                        {
                            Vector3 pos = new Vector3((col * tileSize + playerX),
                                0,
                                (row * tileSize + playerZ));
                            //string tilename = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                            if (!wakingBabies.ContainsKey(pos))
                            {
                                newBabies.Add(pos);
                            }
                            else
                            {
                                wakingBabies[pos].creationTime = updateTime;
                            }
                        }
                    }
                    // Sort in order of distance from the player
                    newBabies.Sort((a, b) => (int)Vector3.SqrMagnitude(player.transform.position - a) - (int)Vector3.SqrMagnitude(player.transform.position - b));
                    foreach (Vector3 pos in newBabies)
                    {
                        GameObject t = MakeABaby(pos);
                        //string tilename = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                        //t.name = tilename;
                        Baby baby = new Baby(t, updateTime);
                        wakingBabies[pos] = baby;
                        yield return WaitFor.Frames(Random.Range(1, 3));
                    }

                    //destroy all tiles not just created or with time updated
                    //and put new tiles and tiles to be kepts in a new hashtable
                    Dictionary<Vector3, Baby> newWakingBabies = new Dictionary<Vector3, Baby>();
                    foreach (Baby baby in wakingBabies.Values)
                    {
                        if (baby.creationTime != updateTime)
                        {
                            Debug.Log("Deleting baby: " + baby.key);
                            Destroy(baby.baby);
                            yield return WaitFor.Frames(Random.Range(1, 3));
                        }
                        else
                        {
                            newWakingBabies[baby.key] = baby;
                        }
                    }
                    //copy new hashtable contents to the working hashtable
                    wakingBabies = newWakingBabies;
                    startPos = player.transform.position;
                }
                yield return null;
                //determine how far the player has moved since last terrain update
                xMove = (int)(player.transform.position.x - startPos.x);
                zMove = (int)(player.transform.position.z - startPos.z);
            }
        }

        int dice = 0;

        int Hash(Vector3 pos)
        {
            float f = pos.x + pos.y + pos.z;
            Debug.Log(pos);
            Debug.Log(f);
            return (int) f; 
        }

        private GameObject MakeABaby(Vector3 pos)
        {
            // This should always generate the same creature at the same point

            //int dice = (int) Utilities.Map(Mathf.PerlinNoise(pos.x, pos.z), 0, 1, 0, prefabs.Length);
            //int dice = 12;
            int dice = Random.Range(0, prefabs.Length);
            Debug.Log(dice);
            GameObject go = GameObject.Instantiate(prefabs[dice]);

            if (wg != null)
            {
                pos.y = Utilities.RandomRange(200, 1000) + wg.SamplePos(pos.x, pos.z);
            }

            go.transform.position = pos;
            go.transform.parent = this.transform;
            dice++;
            return go;
        }

        // Update is called once per frame
        void Update () {
	        
        }
    }
}