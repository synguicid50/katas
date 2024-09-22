using System.Diagnostics;

namespace Minesweeper
{
    internal class Program
    {
        static void Main()
        {

            Console.Clear();
            Console.CursorVisible = false;
            Console.Title = "Minesweeper";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int gridLengthX = 0;
            int gridLengthY = 0;
            int mineCount = 0;

            ConfigureGrid(ref gridLengthX, ref gridLengthY, ref mineCount);

            Sweep(gridLengthX, gridLengthY, mineCount);

        }

        static void ConfigureGrid(ref int gridLengthX, ref int gridLengthY, ref int mineCount)
        {
            int difficultyLevel = DisplayDiffConfigInput();

            switch (difficultyLevel)
            {
                case 0:
                    gridLengthX = 9;
                    gridLengthY = 9;
                    mineCount = 10;
                    break;
                case 1:
                    gridLengthX = 16;
                    gridLengthY = 16;
                    mineCount = 40;
                    break;
                case 2:
                    gridLengthX = 25;
                    gridLengthY = 25;
                    mineCount = 125;
                    break;
            }
        }

        static int DisplayDiffConfigInput()
        {
            List<string> difficultyLevels = new List<string>()
            {
                "Beginner      ",
                "Intermediate  ",
                "Expert        "
            };

            bool inputReceived = false;
            int arrowPosition = 0;

            while (!inputReceived)
            {
                Console.Clear();
                Console.Write("Choose difficulty:");
                foreach (var level in difficultyLevels)
                {
                    Console.Write($"\n{level}");
                    if (arrowPosition == difficultyLevels.IndexOf(level))
                    {
                        Console.Write("\u2190");
                    }
                }

                ConsoleKey userInput = Console.ReadKey().Key;

                switch (userInput)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if (arrowPosition > 0)
                        {
                            arrowPosition--;
                        }
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (arrowPosition < 2)
                        {
                            arrowPosition++;
                        }
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        inputReceived = true;
                        break;
                }
            }
            return arrowPosition;
        }
        static void Sweep(int gridLengthX, int gridLengthY, int mineCount)
        {
            Console.Clear();

            int tileCount = gridLengthX * gridLengthY;

            string instructions = " Move around in the grid with [WASD] or [\u2190\u2191\u2192\u2193]\n Press [SPACE] to destroy a tile, press [F] to place or remove a flag";

            List<int> mineTiles = new List<int>();
            PlaceMines(mineCount, tileCount, ref mineTiles);

            Dictionary<int, int> tileNumbers = new Dictionary<int, int>();
            PlaceNumbers(gridLengthX, gridLengthY, ref tileNumbers, mineTiles);

            List<int> visibleTiles = new List<int>();
            List<int> flaggedTiles = new List<int>();

            int focusTile = ((gridLengthY - (gridLengthY % 2)) / 2) * gridLengthX + ((gridLengthX - (gridLengthX % 2)) / 2); //place focusTile at the approximate center of grid

            Console.WriteLine(instructions);
            DisplayGrid(gridLengthX, gridLengthY, visibleTiles, mineTiles, tileNumbers, focusTile, flaggedTiles);

            bool gameOver = false;
            bool gameWon = false;

            Stopwatch gameTimer = new Stopwatch();
            gameTimer.Start();

            while (!gameOver)
            {
                ConsoleKey userInput = Console.ReadKey().Key;

                switch (userInput)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if ((focusTile - (focusTile % gridLengthX)) / gridLengthX != 0)
                        {
                            focusTile -= gridLengthX;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if ((focusTile - (focusTile % gridLengthX)) / gridLengthX != gridLengthY - 1)
                        {
                            focusTile += gridLengthX;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        if (focusTile % gridLengthX != 0)
                        {
                            focusTile--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        if (focusTile % gridLengthX != gridLengthX - 1)
                        {
                            focusTile++;
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        if (!flaggedTiles.Contains(focusTile))
                        {
                            visibleTiles.Add(focusTile);
                        }
                        if (tileNumbers.ContainsKey(focusTile) && tileNumbers[focusTile] == 0)
                        {
                            ChainClear(focusTile, ref visibleTiles, gridLengthX, gridLengthY, tileNumbers);
                            ChainClearBorder(ref visibleTiles, gridLengthX, gridLengthY, tileNumbers);
                        }
                        break;
                    case ConsoleKey.F:
                        if (flaggedTiles.Contains(focusTile))
                        {
                            flaggedTiles.Remove(focusTile);
                        }
                        else
                        {
                            flaggedTiles.Add(focusTile);
                        }
                        break;
                }

                Console.Clear();
                Console.WriteLine("\x1b[3J");
                Console.WriteLine(instructions);
                DisplayGrid(gridLengthX, gridLengthY, visibleTiles, mineTiles, tileNumbers, focusTile, flaggedTiles);

                if (visibleTiles.Contains(focusTile) && mineTiles.Contains(focusTile))
                {
                    gameOver = true;
                }
                else if (visibleTiles.Count == (tileCount - mineTiles.Count))
                {
                    gameOver = true;
                    gameWon = true;
                }
            }

            gameTimer.Stop();
            double gameTime = Convert.ToDouble(gameTimer.ElapsedMilliseconds) / 1000;

            bool quitCommand = false;
            while (!quitCommand)
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");

                if (!gameWon)
                {
                    foreach (var tile in mineTiles)
                    {
                        if (!visibleTiles.Contains(tile))
                        {
                            visibleTiles.Add(tile);
                        }
                    }

                    Console.WriteLine($"--- GAME OVER ---\nYou have failed miserably in just {gameTime} seconds\nPress [R] to restart, press [Q] to quit");
                }

                if (gameWon)
                {
                    Console.WriteLine($"--- WELL DONE ---\nYou have completed the game in just {gameTime} seconds\nPress [R] to restart, press [Q] to quit");
                }

                DisplayGrid(gridLengthX, gridLengthY, visibleTiles, mineTiles, tileNumbers, focusTile, flaggedTiles);

                ConsoleKey userRestartInput = Console.ReadKey().Key;

                switch (userRestartInput)
                {
                    case ConsoleKey.R:
                        Sweep(gridLengthX, gridLengthY, mineCount);
                        break;
                    case ConsoleKey.Q:
                        quitCommand = true;
                        break;
                    default:
                        break;
                }
            }
        }
        static void PlaceMines(int mineCount, int tileCount, ref List<int> mineTiles)
        {
            List<int> allTiles = new List<int>();

            for (int i = 0; i < tileCount; i++)
            {
                allTiles.Add(i);
            }

            Random rng = new Random();

            for (int i = 0; i < mineCount; i++)
            {
                int randomIndex = rng.Next(0, allTiles.Count);
                mineTiles.Add(allTiles[randomIndex]);
                allTiles.Remove(allTiles[randomIndex]);
            }
        }

        static void PlaceNumbers(int gridLengthX, int gridLengthY, ref Dictionary<int, int> tileNumbers, List<int> mineTiles)
        {
            int tileCount = gridLengthX * gridLengthY;

            List<int> numerableTiles = new List<int>();
            for (int i = 0; i < tileCount; i++)
            {
                numerableTiles.Add(i);
            }

            foreach (var tile in mineTiles)
            {
                numerableTiles.Remove(tile);
            }

            foreach (var tile in numerableTiles)
            {
                int tileNumber = 0;

                foreach (var adjacentTile in FindAdjacentTiles(tile, gridLengthX, gridLengthY))
                {
                    if (mineTiles.Contains(adjacentTile))
                    {
                        tileNumber++;
                    }
                }

                tileNumbers.Add(tile, tileNumber);
            }
        }

        static List<int> FindAdjacentTiles(int tile, int gridLengthX, int gridLengthY)
        {
            List<int> adjacentTiles = new List<int>();

            bool isLeftBorderTile = tile % gridLengthX == 0;
            bool isRightBorderTile = tile % gridLengthX == gridLengthX - 1;
            bool isTopBorderTile = (tile - (tile % gridLengthX)) / gridLengthX == 0;
            bool isBottomBorderTile = (tile - (tile % gridLengthX)) / gridLengthX == gridLengthY - 1; // :)

            switch ((isLeftBorderTile, isRightBorderTile, isTopBorderTile, isBottomBorderTile))
            {
                case (true, false, true, false):
                    //345
                    adjacentTiles.Add(tile + 1);
                    adjacentTiles.Add((tile + gridLengthX) + 1);
                    adjacentTiles.Add((tile + gridLengthX));
                    break;
                case (false, false, true, false):
                    //34567
                    adjacentTiles.Add(tile + 1);
                    adjacentTiles.Add((tile + gridLengthX) + 1);
                    adjacentTiles.Add((tile + gridLengthX));
                    adjacentTiles.Add((tile + gridLengthX) - 1);
                    adjacentTiles.Add(tile - 1);
                    break;
                case (false, true, true, false):
                    //567
                    adjacentTiles.Add((tile + gridLengthX));
                    adjacentTiles.Add((tile + gridLengthX) - 1);
                    adjacentTiles.Add(tile - 1);
                    break;
                case (false, true, false, false):
                    //01567
                    adjacentTiles.Add((tile - gridLengthX) - 1);
                    adjacentTiles.Add((tile - gridLengthX));
                    adjacentTiles.Add((tile + gridLengthX));
                    adjacentTiles.Add((tile + gridLengthX) - 1);
                    adjacentTiles.Add(tile - 1);
                    break;
                case (false, true, false, true):
                    //017
                    adjacentTiles.Add((tile - gridLengthX) - 1);
                    adjacentTiles.Add((tile - gridLengthX));
                    adjacentTiles.Add(tile - 1);
                    break;
                case (false, false, false, true):
                    //01237
                    adjacentTiles.Add((tile - gridLengthX) - 1);
                    adjacentTiles.Add((tile - gridLengthX));
                    adjacentTiles.Add((tile - gridLengthX) + 1);
                    adjacentTiles.Add(tile + 1);
                    adjacentTiles.Add(tile - 1);
                    break;
                case (true, false, false, true):
                    //123
                    adjacentTiles.Add((tile - gridLengthX));
                    adjacentTiles.Add((tile - gridLengthX) + 1);
                    adjacentTiles.Add(tile + 1);
                    break;
                case (true, false, false, false):
                    //12345
                    adjacentTiles.Add((tile - gridLengthX));
                    adjacentTiles.Add((tile - gridLengthX) + 1);
                    adjacentTiles.Add(tile + 1);
                    adjacentTiles.Add((tile + gridLengthX) + 1);
                    adjacentTiles.Add((tile + gridLengthX));
                    break;
                default:
                    adjacentTiles.Add((tile - gridLengthX) - 1);
                    adjacentTiles.Add((tile - gridLengthX));
                    adjacentTiles.Add((tile - gridLengthX) + 1);
                    adjacentTiles.Add(tile + 1);
                    adjacentTiles.Add((tile + gridLengthX) + 1);
                    adjacentTiles.Add((tile + gridLengthX));
                    adjacentTiles.Add((tile + gridLengthX) - 1);
                    adjacentTiles.Add(tile - 1);
                    break;
            }

            return adjacentTiles;
        }

        static void DisplayGrid(int gridLengthX, int gridLengthY, List<int> visibleTiles, List<int> mineTiles, Dictionary<int, int> tileNumbers, int focusTile, List<int> flaggedTiles)
        {
            Console.Write("╔");
            for (int i = 0; i < gridLengthX; i++)
            {
                Console.Write("═══");
                if (i < (gridLengthX - 1))
                {
                    Console.Write("╤");
                }
            }
            Console.WriteLine("╗");

            //tileIndex = i + gridLengthX * j
            for (int j = 0; j < gridLengthY; j++)
            {
                Console.Write("║");
                for (int i = 0; i < gridLengthX; i++)
                {
                    var tileBackground = (" ", " ");
                    string tileCenter = " ";

                    if (visibleTiles.Contains(i + gridLengthX * j))
                    {
                        if (mineTiles.Contains(i + gridLengthX * j))
                        {
                            tileCenter = "*";
                        }
                        else if (tileNumbers.ContainsKey(i + gridLengthX * j))
                        {
                            tileCenter = (tileNumbers[i + gridLengthX * j] == 0) ? " " : Convert.ToString(tileNumbers[i + gridLengthX * j]);
                        }
                    }
                    else if (flaggedTiles.Contains(i + gridLengthX * j))
                    {
                        tileCenter = "\u2690";
                    }
                    else
                    {
                        tileBackground = ("[", "]");
                    }

                    if (i + gridLengthX * j == focusTile)
                    {
                        tileBackground = ("{", "}");
                        if (!visibleTiles.Contains(i + gridLengthX * j) && !flaggedTiles.Contains(i + gridLengthX * j))
                        {
                            tileCenter = "X";
                        }
                    }

                    Console.Write(tileBackground.Item1 + tileCenter + tileBackground.Item2);

                    if (i < (gridLengthX - 1))
                    {
                        Console.Write("│");
                    }
                }
                Console.WriteLine("║");

                if (j < (gridLengthY - 1))
                {
                    Console.Write("╟");
                    for (int i = 0; i < gridLengthX; i++)
                    {
                        Console.Write("───");
                        if (i < (gridLengthX - 1))
                        {
                            Console.Write("┼");
                        }
                    }
                    Console.WriteLine("╢");
                }
            }

            Console.Write("╚");
            for (int i = 0; i < gridLengthX; i++)
            {
                Console.Write("═══");
                if (i < (gridLengthX - 1))
                {
                    Console.Write("╧");
                }
            }
            Console.WriteLine("╝");
        }

        static void ChainClear(int focusTile, ref List<int> visibleTiles, int gridLengthX, int gridLengthY, Dictionary<int, int> tileNumbers)
        {
            foreach (var adjacentTile in FindAdjacentTiles(focusTile, gridLengthX, gridLengthY))
            {
                if (!visibleTiles.Contains(adjacentTile) && tileNumbers.ContainsKey(adjacentTile) && tileNumbers[adjacentTile] == 0)
                {
                    visibleTiles.Add(adjacentTile);
                    ChainClear(adjacentTile, ref visibleTiles, gridLengthX, gridLengthY, tileNumbers);
                }
            }
        }

        static void ChainClearBorder(ref List<int> visibleTiles, int gridLengthX, int gridLengthY, Dictionary<int, int> tileNumbers)
        {
            if (visibleTiles.Count > 0)
            {
                for (int i = 0; i < visibleTiles.Count; i++)
                {
                    if (tileNumbers.ContainsKey(visibleTiles[i]) && tileNumbers[visibleTiles[i]] == 0)
                    {
                        foreach (var adjacentTile in FindAdjacentTiles(visibleTiles[i], gridLengthX, gridLengthY))
                        {
                            if (!visibleTiles.Contains(adjacentTile) && tileNumbers.ContainsKey(adjacentTile) && tileNumbers[adjacentTile] != 0)
                            {
                                visibleTiles.Add(adjacentTile);
                            }
                        }
                    }
                }
            }
        }
    }
}
