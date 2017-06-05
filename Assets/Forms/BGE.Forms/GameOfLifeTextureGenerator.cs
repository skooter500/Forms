using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class GameOfLifeTextureGenerator : TextureGenerator
    {
        public int intervals = 5;
       
        public float updatesPerSecond = 30.0f;

        [Range(0.01f, 100)]
        public float speed = 2;

        Color[,] current;
        Color[,] next;

        float saturation = 0.9f;
        float brightness = 0.9f;

        public bool wrap = false;

        public void Clear()
        {
            ClearBoard(current);
        }

        private void ClearBoard(Color[,] board)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    board[y, x] = Color.black;
                }
            }
        }

        private void StartingPattern(Color[,] board)
        {
            generation = 0;
            ClearBoard(current);
            for (int col = 0; col < size; col++)
            {
                board[20, col] = Color.HSVToRGB(Random.Range(0, 1), saturation, brightness);
                board[30, col] = Color.HSVToRGB(Random.Range(0, 1), saturation, brightness);
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
            current = new Color[size, size];
            next = new Color[size, size];
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
            if (current[up, left] > 0)
            {
                count++;
            }
            // Top
            if (current[up, col] > 0)
            {
                count++;
            }
            // Top right
            if (current[up, right] > 0)
            {
                count++;
            }
            // Left
            if (current[row, left] > 0)
            {
                count++;
            }
            // Right
            if (current[row, right] > 0)
            {
                count++;
            }
            // Bottom left
            if (current[down, left] > 0)
            {
                count++;
            }
            // Bottom
            if (current[down, col] > 0)
            {
                count++;
            }
            // Bottom right
            if (current[down, right] > 0)
            {
                count++;
            }
            return count;
        }

        bool Alive(Color c)
        {
            return c != Color.black;
        }

        int CountNeighbours(int row, int col)
        {
            int count = 0;

            // Top left
            if ((row > 0) && (col > 0) && (Alive(current[row - 1, col - 1])))
            {
                count++;
            }
            // Top
            if ((row > 0) && Alive(current[row - 1, col]))
            {
                count++;
            }
            // Top right
            if ((row > 0) && (col < (size - 1)) && Alive(current[row - 1, col + 1]))
            {
                count++;
            }
            // Left
            if ((col > 0) && Alive(current[row, col - 1]))
            {
                count++;
            }
            // Right
            if ((col < (size - 1)) && Alive(current[row, col + 1]))
            {
                count++;
            }
            // Bottom left
            if ((col > 0) && (row < (size - 1))
                && Alive(current[row + 1, col - 1]))
            {
                count++;
            }
            // Bottom
            if ((row < (size - 1)) && Alive(current[row + 1, col]))
            {
                count++;
            }
            // Bottom right
            if ((col < (size - 1)) && (row < (size - 1))
                && Alive(current[row + 1, col + 1]))
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
                        current[row, col] = Color.HSVToRGB(Random.Range(0, 1), saturation, brightness);
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
            generation = 1;
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
                        if (current[row, col] > 0)
                        {
                            if (count < 2)
                            {
                                next[row, col] = Color.black;
                            }
                            else if ((count == 2) || (count == 3))
                            {
                                next[row, col] = current[row, col];
                            }
                            else if (count > 3)
                            {
                                next[row, col] = Color.black;
                            }
                        }
                        else
                        {
                            if (count == 3)
                            {
                                next[row, col] = generation;
                            }
                        }
                        // next[row, col] = current[row, col];
                    }
                
                }
                Color[,] temp;
                temp = current;
                current = next;
                next = temp;
                float t = 0.0f;
                generation++;


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
                            texture.SetPixel(x, y, Color.Lerp(
                                texture.GetPixel(x, y)
                                , current[x, y], t));                        
                        }
                    }
                    t += tDelta;
                    texture.Apply();
                    yield return new WaitForSeconds(delay);
                }

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
                current[y, x] = generation;
            }
        }

        public void Off(int x, int y)
        {
            if ((x >= 0) && (x < size) && (y >= 0) && (y < size))
            {
                current[y, x] = 0;
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