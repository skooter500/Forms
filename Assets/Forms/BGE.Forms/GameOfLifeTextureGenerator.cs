using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class GameOfLifeTextureGenerator : TextureGenerator
    {
        public Color backGround = Color.black;
        public Color foreGround = Color.gray;

        public float updatesPerSecond = 30.0f;

        [Range(0.01f, 100)]
        public float speed = 2;

        bool[,] current;
        bool[,] next;

        public bool wrap = false;

        public void Clear()
        {
            ClearBoard(current);
        }

        private void ClearBoard(bool[,] board)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    board[y, x] = false;
                }
            }
        }

        private void StartingPattern(bool[,] board)
        {
            generation = 0;
            ClearBoard(current);
            for (int col = 0; col < size; col++)
            {
                board[20, col] = true;
                board[30, col] = true;
            }
        }

        public override void GenerateTexture()
        {
            if (texture == null)
            {
                texture = new Texture2D(size, size);
                texture.filterMode = FilterMode.Point;
            }
        }

        void Awake()
        {
            GenerateTexture();
        }

        void Start()
        {

            generation = 0;
            current = new bool[size, size];
            next = new bool[size, size];
            //MakeGosperGun(size / 2, size / 2);
            //MakeTumbler(size / 2, size / 2);        
            //StartingPattern(current);
            Randomise();
            StartCoroutine("UpdateBoard");
            //StartCoroutine("Spawner");
        }

        IEnumerator Spawner()
        {
            while (true)
            {
                /*int i = Random.Range(0, 6);
            int x = Random.Range(5, size - 5);
            switch (i)
            {
                case 0:
                    
                    //MakeGlider(x, 5);
                    break;
                //case 1:
                //    MakeLightWeightSpaceShip(x, 5);
                //    break;
                //case 2:
                //    break;
                case 5:
                    //ClearBoard(current);
                    break;
            }
            */
                yield return new WaitForSeconds(Random.Range(10.0f, 30.0f));
            }
        }

        bool paused = false;

        int ModNeg(int num, int mod)
        {
            int r = num % mod;
            if (r < 0)
            {
                r += mod;
            }
            return r;
        }

        int CountNeighboursWrapped(int row, int col)
        {
            int count = 0;
            int left, right, up, down;

            left = ModNeg(col - 1, size);
            right = ModNeg(col + 1, size);
            up = ModNeg(row - 1, size);
            down = ModNeg(row + 1, size);

            // Top left
            if ((current[up, left]))
            {
                count++;
            }
            // Top
            if (current[up, col])
            {
                count++;
            }
            // Top right
            if (current[up, right])
            {
                count++;
            }
            // Left
            if (current[row, left])
            {
                count++;
            }
            // Right
            if (current[row, right])
            {
                count++;
            }
            // Bottom left
            if (current[down, left])
            {
                count++;
            }
            // Bottom
            if (current[down, col])
            {
                count++;
            }
            // Bottom right
            if (current[down, right])
            {
                count++;
            }
            return count;
        }

        int CountNeighbours(int row, int col)
        {
            int count = 0;

            // Top left
            if ((row > 0) && (col > 0) && (current[row - 1, col - 1]))
            {
                count++;
            }
            // Top
            if ((row > 0) && current[row - 1, col])
            {
                count++;
            }
            // Top right
            if ((row > 0) && (col < (size - 1)) && (current[row - 1, col + 1]))
            {
                count++;
            }
            // Left
            if ((col > 0) && (current[row, col - 1]))
            {
                count++;
            }
            // Right
            if ((col < (size - 1)) && current[row, col + 1])
            {
                count++;
            }
            // Bottom left
            if ((col > 0) && (row < (size - 1))
                && current[row + 1, col - 1])
            {
                count++;
            }
            // Bottom
            if ((row < (size - 1)) && (current[row + 1, col]))
            {
                count++;
            }
            // Bottom right
            if ((col < (size - 1)) && (row < (size - 1))
                && current[row + 1, col + 1])
            {
                count++;
            }
            return count;
        }

        public void Randomise()
        {
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    float f = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (f < 0.5f)
                    {
                        current[row, col] = true;
                    }
                }
            }
        }

        public int generation = 0;

        System.Collections.IEnumerator ResetBoard()
        {
            while (true)
            {
                yield return new WaitForSeconds(speed * 50);
                StartingPattern(current);
            }
        }

        System.Collections.IEnumerator UpdateBoard()
        {        
            while (true)
            {
                if (texture == null)
                {
                    break;
                }
                ClearBoard(next);
                for (int row = 0; row < size; row++)
                {
                    for (int col = 0; col < size; col++)
                    {                    
                        int count = (wrap) ? CountNeighboursWrapped(row, col) : CountNeighbours(row, col);
                        if (current[row, col])
                        {
                            if (count < 2)
                            {
                                next[row, col] = false;
                            }
                            else if ((count == 2) || (count == 3))
                            {
                                next[row, col] = true;
                            }
                            else if (count > 3)
                            {
                                next[row, col] = false;
                            }
                        }
                        else
                        {
                            if (count == 3)
                            {
                                next[row, col] = true;
                            }
                        }
                        // next[row, col] = current[row, col];
                    }
                
                }
                bool[,] temp;
                temp = current;
                current = next;
                next = temp;
                float t = 0.0f;

                /*for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Color to = current[y, x] ? foreGround : backGround;
                    texture.SetPixel(x, y, to);
                }
            }
            texture.Apply();
            yield return new WaitForSeconds(1.0f / speed);
            */

                // Now lerp to the new board            
                float delay = 1.0f / updatesPerSecond;
                float tDelta = 1.0f / (updatesPerSecond / speed);
                while (t <= 1.0f)
                {                
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            Color from = next[y, x] ? foreGround : backGround;
                            Color to = current[y, x] ? foreGround : backGround;
                            texture.SetPixel(x, y, Color.Lerp(from, to, t));                        
                        }
                    }
                    t += tDelta;
                    texture.Apply();
                    yield return new WaitForSeconds(delay);
                }
                generation++;
                if (generation >= 150)
                {
                    StartingPattern(current);
                }
            }
        }

        public void On(int x, int y)
        {
            if ((x >= 0) && (x < size) && (y >= 0) && (y < size))
            {
                current[y, x] = true;
            }
        }

        public void Off(int x, int y)
        {
            if ((x >= 0) && (x < size) && (y >= 0) && (y < size))
            {
                current[y, x] = false;
            }
        }

        public void MakeGliderRow()
        {
            for (int x = 5; x < size; x += 10)
            {
                MakeGlider(x, 5);
            }
        }

        public void MakeGlider(int x, int y)
        {
            On(x + 1, y);
            On(x, y - 1);
            On(x - 1, y + 1);
            On(x, y + 1);
            On(x + 1, y + 1);
        }

        public void MakeGosperGun(int x, int y)
        {
            On(x + 23, y);
            On(x + 24, y);
            On(x + 34, y);
            On(x + 35, y);

            On(x + 22, y + 1);
            On(x + 24, y + 1);
            On(x + 34, y + 1);
            On(x + 35, y + 1);

            On(x + 0, y + 2);
            On(x + 1, y + 2);
            On(x + 9, y + 2);
            On(x + 10, y + 2);
            On(x + 22, y + 2);
            On(x + 23, y + 2);

            On(x + 0, y + 3);
            On(x + 1, y + 3);
            On(x + 8, y + 3);
            On(x + 10, y + 3);

            On(x + 8, y + 4);
            On(x + 9, y + 4);
            On(x + 16, y + 4);
            On(x + 17, y + 4);

            On(x + 16, y + 5);
            On(x + 18, y + 5);

            On(x + 16, y + 6);

            On(x + 35, y + 7);
            On(x + 36, y + 7);

            On(x + 35, y + 8);
            On(x + 37, y + 8);

            On(x + 35, y + 9);

            On(x + 24, y + 12);
            On(x + 25, y + 12);
            On(x + 26, y + 12);

            On(x + 24, y + 13);

            On(x + 25, y + 14);
        }

        public void MakeLightWeightSpaceShip(int x, int y)
        {
            On(x + 1, y);
            On(x + 2, y);
            On(x + 3, y);
            On(x + 4, y);

            On(x, y + 1);
            On(x + 4, y + 1);

            On(x + 4, y + 2);

            On(x, y + 3);
            On(x + 3, y + 3);
        }


        public void MakeTumbler(int x, int y)
        {
            On(x + 1, y);
            On(x + 2, y);
            On(x + 4, y);
            On(x + 5, y);

            On(x + 1, y + 1);
            On(x + 2, y + 1);
            On(x + 4, y + 1);
            On(x + 5, y + 1);

            On(x + 2, y + 2);
            On(x + 4, y + 2);

            On(x, y + 3);
            On(x + 2, y + 3);
            On(x + 4, y + 3);
            On(x + 6, y + 3);

            On(x, y + 4);
            On(x + 2, y + 4);
            On(x + 4, y + 4);
            On(x + 6, y + 4);

            On(x, y + 5);
            On(x + 1, y + 5);
            On(x + 5, y + 5);
            On(x + 6, y + 5);

        }
    }
}