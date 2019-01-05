using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class GameOfLifeTextureGenerator : TextureGenerator
    {
        public Color backGround = Color.black;
        public Color secondaryBackGround = Color.white;
        public Color foreGround = Color.gray;

        public float updatesPerSecond = 30.0f;

        [Range(0.01f, 100)]
        public float speed = 2;

        Color[,] current;
        Color[,] next;

        public bool wrap = false;

        public void Clear()
        {
            ClearBoard(current);
        }

        public static GameOfLifeTextureGenerator Instance;
            
        public float sat = 1.0f;
        public float brightness = 1.0f;

        public bool monochrome = true;

        public Color RandomColor()
        {
            return monochrome ? Color.white : Color.HSVToRGB(Random.Range(0.0f, 1.0f), sat, brightness);
        }

        public void ClearBoard(Color[,] board)
        {
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    board[row, col] = Color.black;
                }
            }
        }

        private delegate void StartingPattern(Color[,] board);

        StartingPattern startingPattern;

        private void GridStartingPattern(Color[,] board)
        {
            generation = 0;
            generationMax = 35;
            ClearBoard(board);

            for (int col = 0; col < size; col++)
            {
                int x1 = (int)(size * 0.2);
                int x2 = (int)(size * 0.8);
                board[x1, col] = RandomColor();
                //board[x2, col] = RandomColor();
                board[col, x1] = RandomColor();
                //board[col, x2] = RandomColor();
            }
        }

        private void Pi(Color[,] board)
        {
            ClearBoard(board);
            float radius = (size - 20) / 2;
            int points = 100;
            float theta = Mathf.PI * 2.0f / points;
            for (int i = 0; i < points; i++)
            {
                float x = radius + (Mathf.Sin(theta * i) * radius);
                float y = radius + (Mathf.Cos(theta * i) * radius);
                board[(int)x, (int)y] = RandomColor();
            }
        }

        private void BoxBox(Color[,] board)
        {
            generation = 0;
            generationMax = 20;
            ClearBoard(board);
            int x1 = (int)(size * 0.2);
            int x2 = (int)(size * 0.8);
            for (int col = x1; col < x2; col++)
            {
                board[x1, col] = RandomColor();
                board[x2, col] = RandomColor();
                board[col, x1] = RandomColor();
                board[col, x2] = RandomColor();
            }
            x1 = (int)(size * 0.4);
            x2 = (int)(size * 0.6);
            for (int col = x1; col < x2; col++)
            {
                board[x1, col] = RandomColor();
                board[x2, col] = RandomColor();
                board[col, x1] = RandomColor();
                board[col, x2] = RandomColor();
            }
        }
    
        private void BoxStartingPattern(Color[,] board)
        {
            generation = 0;
            generationMax = 20;
            ClearBoard(board);
            int x1 = (int)(size * 0.2);
            int x2 = (int)(size * 0.8);
            for (int col = x1; col < x2; col++)
            {
                board[x1, col] = RandomColor();
                board[x2, col] = RandomColor();
                board[col, x1] = RandomColor();
                board[col, x2] = RandomColor();
            }
        }

        private void CrossStartingPattern(Color[,] board)
        {
            generation = 0;
            generationMax = 30;
            ClearBoard(board);

            for (int col = 0; col < size; col++)
            {
                board[col, col] = RandomColor();
                //board[x2, col] = RandomColor();
                board[col, size - (col + 1)] = RandomColor();
                //board[col, x2] = RandomColor();
            }
        }

        public override void GenerateTexture()
        {
            if (texture == null)
            {
                texture = new Texture2D(size, size);
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Mirror;

                secondaryTexture = new Texture2D(size, size);
                secondaryTexture.filterMode = FilterMode.Point;
                secondaryTexture.wrapMode = TextureWrapMode.Mirror;
            }
        }

        void Awake()
        {
            Instance = this;
            GenerateTexture();
            current = new Color[size, size];
            next = new Color[size, size];
            //MakeGosperGun(size / 2, size / 2);
            //MakeTumbler(size / 2, size / 2);   

            startingPattern = new StartingPattern(GridStartingPattern);
            startingPattern(current);

        }

        void Start()
        {

            generation = 0;
            
            //Randomise();
            //StartCoroutine("Spawner");
        }

        private void OnEnable()
        {
            StartCoroutine("UpdateBoard");
        }

        IEnumerator Spawner()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(10.0f, 30.0f));
                int i = Random.Range(0, 1);
                switch (i)
                {
                    case 1:
                            GridStartingPattern(current);
                            break;
                }
                
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

        bool IsAlive(int row, int col)
        {
            if (row < 0 || row >= size || col < 0 || col >= size)
            {
                return false;
            }
            else
            {
                return (current[row, col] != Color.black);
            }
        }

        bool Alive(Color cell)
        {
            return !(cell == Color.black);
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
            if ((Alive(current[up, left])))
            {
                count++;
            }
            // Top
            if (Alive(current[up, col]))
            {
                count++;
            }
            // Top right
            if (Alive(current[up, right]))
            {
                count++;
            }
            // Left
            if (Alive(current[row, left]))
            {
                count++;
            }
            // Right
            if (Alive(current[row, right]))
            {
                count++;
            }
            // Bottom left
            if (Alive(current[down, left]))
            {
                count++;
            }
            // Bottom
            if (Alive(current[down, col]))
            {
                count++;
            }
            // Bottom right
            if (Alive(current[down, right]))
            {
                count++;
            }
            return count;
        }

        int CountNeighbours(int row, int col)
        {

            int count = 0;
            for (int r = row - 1; r <= row + 1; r++)
            {
                for (int c = col - 1; c <= col + 1; c++)
                {
                    if (!(r == row && c == col) && IsAlive(r, c))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void Randomise(Color[,] board)
        {
            generation = 0;
            generationMax = 200;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    float f = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (f < 0.5f)
                    {
                        board[row, col] = RandomColor();
                    }
                    else
                    {
                        board[row, col] = Color.black;
                    }
                }
            }
        }

        [HideInInspector] public int generation = 0;
        [HideInInspector] public int generationMax = 40;

        System.Collections.IEnumerator ResetBoard()
        {
            while (true)
            {
                yield return new WaitForSeconds(speed * 50);
                GridStartingPattern(current);
            }
        }

        System.Collections.IEnumerator UpdateBoard()
        {
            float alpha = 0.0f;
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
                        //int count = (wrap) ? CountNeighboursWrapped(row, col) : CountNeighbours(row, col);
                        int count = CountNeighbours(row, col);
                        //Debug.Log(count);
                        if (Alive(current[row, col]))
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
                                next[row, col] = NewCellColor(current, row, col);
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
                while (t <= 1f)
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            Color from = (next[y, x] == Color.black) ? backGround : next[y, x];
                            Color to = (current[y, x] == Color.black) ? backGround : current[y, x];
                            Color c = Color.Lerp(from, to, t);
                            texture.SetPixel(x, y, c);

                            from = (next[y, x] == Color.black) ? secondaryBackGround: next[y, x];
                            to = (current[y, x] == Color.black) ? secondaryBackGround : current[y, x];
                            c = Color.Lerp(from, to, t);
                            secondaryTexture.SetPixel(x, y, c);
                        }
                    }
                    t += tDelta;
                    if (alpha > 1.0f)
                    {
                        alpha = 1.0f;
                    }
                    else
                    {
                        alpha += delay / 3.0f;
                    }
                    texture.Apply();
                    secondaryTexture.Apply();
                    yield return new WaitForSeconds(delay);
                }

                generation++;
                if (generation >= generationMax)
                {
                    int dice = Random.Range(0, 4);
                    switch (dice)
                    {
                        case 0:
                            startingPattern = new StartingPattern(GridStartingPattern);
                            break;
                        case 1:
                            startingPattern = new StartingPattern(BoxStartingPattern);
                            break;
                        case 2:
                            startingPattern = new StartingPattern(CrossStartingPattern);
                            break;
                        case 3:
                            startingPattern = new StartingPattern(BoxBox);
                            break;
                    }
                    startingPattern(current);
                }
            }
        }
        
        private Color NewCellColor(Color[,] board, int row, int col)
        {
            if (monochrome)
            {
                return Color.white;
            }
            Color average = Color.black;
            float count = 0;
            for (int r = row - 1; r <= row + 1; r++)
            {
                for (int c = col - 1; c <= col + 1; c++)
                {
                    if (Get(board, r, c) != Color.black)
                    {
                        average += Get(board, r, c);
                        count++;
                    }
                } 
            }
            float h, s, b;
            Color.RGBToHSV(average / count, out h, out s, out b);
            return  Color.HSVToRGB(h, sat, brightness);
        }

        public Color Get(Color[,] board, int row, int col)
        {
            if (row >= 0 && row < size && col >= 0 && col < size)
            {
                return board[row, col];
            }
            else
            {
                return Color.black;
            }
        }

        /*
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
            */

        float lastX, lastY;
        public void Update()
        {
            float x = Input.GetAxis("DPadX");
            float y = Input.GetAxis("DPadY");

            if (x == -1 && x != lastX)
            {
                leftD = (leftD + 1) % 5; // 5 patterns
                ellapsed = 0;
            }
            if (leftD > -1 && ellapsed > toPass)
            {
                switch (leftD)
                {
                    case 0:
                        startingPattern = new StartingPattern(GridStartingPattern);
                        break;
                    case 1:
                        startingPattern = new StartingPattern(BoxStartingPattern);
                        break;
                    case 2:
                        startingPattern = new StartingPattern(CrossStartingPattern);
                        break;
                    case 3:
                        startingPattern = new StartingPattern(BoxBox);
                        break;
                    case 4:
                        startingPattern = new StartingPattern(Randomise);
                        break;
                }
                startingPattern(current);
                leftD = -1;
            }
            ellapsed += Time.deltaTime;
            /*
            {
                startingPattern = new StartingPattern(BoxStartingPattern);
                startingPattern(current);
            }
            if (x == 1 && x != lastX)
            {
                startingPattern = new StartingPattern(CrossStartingPattern);
                startingPattern(current);
            }
            if (y == 1 && y != lastY)
            {
                startingPattern = new StartingPattern(BoxStartingPattern);
                startingPattern(current);
            }
            if (y == -1 && y != lastY)
            {
                startingPattern = new StartingPattern(Randomise);
                startingPattern(current);
            }
            */

            lastX = x;
            lastY = y;
        }

        public float ellapsed = 0;
        public float toPass = 0.5f;
        public int leftD = -1;

    }
}