using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Sudoku Solver";
            Console.CursorVisible = false;

            string[] userInputs = new string[81];

            for (int i = 0; i < userInputs.Length; i++)
            {
                userInputs[i] = " ";
            }

            int inputArrayIndex = 0;

            bool goCommand = false;

            string inputMessage = "--- SUDOKU SOLVER ---\nEnter the numbers, press [SPACE] to leave a space and press [BACKSPACE] to delete a number.";

            //get user inputs

            while (!goCommand)
            {
                if (inputArrayIndex == 81)
                {
                    Console.WriteLine("Press [Enter] to start solving with these numbers...");
                    ConsoleKey userInput = Console.ReadKey().Key;

                    if (userInput == ConsoleKey.Backspace)
                    {
                        userInputs[inputArrayIndex - 1] = "_";
                        inputArrayIndex--;
                    }
                    else if (userInput == ConsoleKey.Enter)
                    {
                        goCommand = true;
                    }
                }

                Console.Clear();
                Console.WriteLine(inputMessage);
                DisplayInputBoard(userInputs);

                if (inputArrayIndex < 81)
                {
                    userInputs[inputArrayIndex] = "_";
                    Console.Clear();
                    Console.WriteLine(inputMessage);
                    DisplayInputBoard(userInputs);

                    ConsoleKey userInput = Console.ReadKey().Key;

                    switch (userInput)
                    {
                        case ConsoleKey.D1:
                            userInputs[inputArrayIndex] = "1";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D2:
                            userInputs[inputArrayIndex] = "2";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D3:
                            userInputs[inputArrayIndex] = "3";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D4:
                            userInputs[inputArrayIndex] = "4";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D5:
                            userInputs[inputArrayIndex] = "5";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D6:
                            userInputs[inputArrayIndex] = "6";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D7:
                            userInputs[inputArrayIndex] = "7";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D8:
                            userInputs[inputArrayIndex] = "8";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.D9:
                            userInputs[inputArrayIndex] = "9";
                            inputArrayIndex++;
                            break;
                        case ConsoleKey.Backspace:
                            if (inputArrayIndex > 0)
                            {
                                userInputs[inputArrayIndex] = " ";
                                userInputs[inputArrayIndex - 1] = " ";
                                inputArrayIndex--;
                            }
                            break;
                        default:
                            userInputs[inputArrayIndex] = " ";
                            inputArrayIndex++;
                            break;
                    }

                    Console.Clear();
                    Console.WriteLine(inputMessage);
                    DisplayInputBoard(userInputs);

                }
            }

            //convert userInputs[] to knownNumbers[]

            //start solving
            var watch = new System.Diagnostics.Stopwatch(); //start execution timer
            watch.Start();

            int[] knownNumbers = new int[81];

            for (int i = 0; i < 81; i++)
            {
                if (userInputs[i] == " ")
                {
                    knownNumbers[i] = 0;
                }
                else
                {
                    knownNumbers[i] = Convert.ToInt16(userInputs[i]);
                }
            }

            int[] possibleNumbers = new int[729]; //a catalog of all possible answers for each subsector

            for (int i = 0; i < 81; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    possibleNumbers[9 * i + j] = j + 1;
                }
            }

            for (int subsector = 0; subsector < knownNumbers.Length; subsector++)
            {
                possibleNumbers = EliminatePossibilities(knownNumbers, possibleNumbers, subsector); //eliminate possibilities before the iterations start
            }

            Console.Clear();
            Console.WriteLine("--- SUDOKU SOLVER ---\n");

            int iteration = 0;

            while (!CheckBoard(knownNumbers) && iteration < 20) //while the puzzle is incomplete 
            {

                for (int subsector = 0; subsector < knownNumbers.Length; subsector++) //goes through every known number
                {
                    //SUBSECTOR POSSIBILITY CHECK
                    //Ideally we want to do this once before the while loop starts and once after a number is found
                    if (knownNumbers[subsector] == 0) //find empty subsectors
                    {

                        //check how many possibilities are there for the subsector

                        int subsectorPossibilityCount = 0;

                        foreach (int possibility in FindSubsectorPossibilities(subsector, possibleNumbers))
                        {
                            if (possibility != 0)
                            {
                                subsectorPossibilityCount++;
                            }
                        }

                        if (subsectorPossibilityCount == 1) //if there is only one possibility for the subsector, then place that number there
                        {
                            foreach (int possibility in FindSubsectorPossibilities(subsector, possibleNumbers))
                            {
                                if (possibility != 0)
                                {
                                    knownNumbers[subsector] = possibility;
                                    Console.WriteLine($"\n(Method0) {possibility} ---> {subsector}");
                                    possibleNumbers = EliminatePossibilities(knownNumbers, possibleNumbers, subsector); //eliminate possibilities considering the new knownNumber
                                }
                            }
                        }


                        //AFTER PLACING A NUMBER, TAKE IT'S SUBSECTOR AND RUN THE POSSIBILITY ELIMINATON PROCESS ONLY FOR THAT NUMBER
                        //AFTER PLACING A NUMBER, TAKE IT'S SUBSECTOR AND RUN THE POSSIBILITY CHECK PROCESS ONLY FOR THAT NUMBER
                        //there is no need the run the process again for all known numbers
                    }
                }

                //Method1
                for (int index = 0; index < 9; index++) //check each sector, row and column (check if there is a number with only one place to go)
                {
                    //doing this before possibility check in each iteration might be faster and/or result in less iterations

                    //check each sector

                    int[] sectorPossibilityWeight = { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //how many of each number are there in sector possibilities {1 to 9}

                    foreach (int subsector in FindSubsectors(index)) //check each subsector in that sector
                    {
                        if (knownNumbers[subsector] == 0) //if subsector is empty
                        {
                            foreach (int possibility in FindSubsectorPossibilities(subsector, possibleNumbers)) //check all possibilities of that subsector
                            {
                                if (possibility != 0)
                                {
                                    sectorPossibilityWeight[possibility - 1]++;
                                }
                            }
                        }
                    }

                    Console.Write($"\nSector{index} | SectorPossibilities: ");
                    for (int i = 0; i < 9; i++)
                    {
                        Console.Write(sectorPossibilityWeight[i]);
                    }
                    Console.WriteLine();

                    for (int i = 0; i < 9; i++) //(i+1) is the number we are looking for, find where it is possible in that sector and place it there
                    {
                        if (sectorPossibilityWeight[i] == 1) //if there is only one possible place for a number
                        {
                            foreach (var subsector in FindSubsectors(index)) //check each subsector for whether the number (i+1) is a possibility
                            {
                                if (FindSubsectorPossibilities(subsector, possibleNumbers).Contains(i + 1))
                                {
                                    knownNumbers[subsector] = i + 1;
                                    Console.WriteLine($"\n(Method1.sector) {i + 1} ---> {subsector}");
                                    possibleNumbers = EliminatePossibilities(knownNumbers, possibleNumbers, subsector); //eliminate possibilities considering the new knownNumber
                                }
                            }
                        }
                    }

                    //check each row

                    int[] rowPossibilityWeight = { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //how many of each number are there in row possibilities {1 to 9}

                    foreach (int subsector in FindRowSubsectors(index)) //check each subsector in that row
                    {
                        if (knownNumbers[subsector] == 0) //if subsector is empty
                        {
                            foreach (int possibility in FindSubsectorPossibilities(subsector, possibleNumbers)) //check all possibilities of that subsector
                            {
                                if (possibility != 0)
                                {
                                    rowPossibilityWeight[possibility - 1]++;
                                }
                            }
                        }
                    }

                    Console.Write($"\nRow{index} | RowPossibilities: ");
                    for (int i = 0; i < 9; i++)
                    {
                        Console.Write(rowPossibilityWeight[i]);
                    }
                    Console.WriteLine();

                    for (int i = 0; i < 9; i++) //(i+1) is the number we are looking for, find where it is possible in that row and place it there
                    {
                        if (rowPossibilityWeight[i] == 1) //if there is only one possible place for a number
                        {
                            foreach (var subsector in FindRowSubsectors(index)) //check each subsector for whether the number (i+1) is a possibility
                            {
                                if (FindSubsectorPossibilities(subsector, possibleNumbers).Contains(i + 1))
                                {
                                    knownNumbers[subsector] = i + 1;
                                    Console.WriteLine($"\n(Method1.row) {i + 1} ---> {subsector}");
                                    possibleNumbers = EliminatePossibilities(knownNumbers, possibleNumbers, subsector); //eliminate possibilities considering the new knownNumber
                                }
                            }
                        }
                    }

                    //check each column

                    int[] columnPossibilityWeight = { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //how many of each number are there in column possibilities {1 to 9}

                    foreach (int subsector in FindColumnSubsectors(index)) //check each subsector in that column
                    {
                        if (knownNumbers[subsector] == 0) //if subsector is empty
                        {
                            foreach (int possibility in FindSubsectorPossibilities(subsector, possibleNumbers)) //check all possibilities of that subsector
                            {
                                if (possibility != 0)
                                {
                                    columnPossibilityWeight[possibility - 1]++;
                                }
                            }
                        }
                    }

                    Console.Write($"\nColumn{index} | ColumnPossibilities: ");
                    for (int i = 0; i < 9; i++)
                    {
                        Console.Write(columnPossibilityWeight[i]);
                    }
                    Console.WriteLine();

                    for (int i = 0; i < 9; i++) //(i+1) is the number we are looking for, find where it is possible in that column and place it there
                    {
                        if (columnPossibilityWeight[i] == 1) //if there is only one possible place for a number
                        {
                            foreach (var subsector in FindColumnSubsectors(index)) //check each subsector for whether the number (i+1) is a possibility
                            {
                                if (FindSubsectorPossibilities(subsector, possibleNumbers).Contains(i + 1))
                                {
                                    knownNumbers[subsector] = i + 1;
                                    Console.WriteLine($"\n(Method1.column) {i + 1} ---> {subsector}");
                                    possibleNumbers = EliminatePossibilities(knownNumbers, possibleNumbers, subsector); //eliminate possibilities considering the new knownNumber
                                }
                            }
                        }
                    }
                }

                iteration++;
                Console.Write($"Iteration {iteration} ");
            }

            if (CheckBoard(knownNumbers)) Console.WriteLine("is successful.");

            watch.Stop();
            Console.WriteLine($"Execution time: {watch.ElapsedMilliseconds} ms");

            DisplayBoard(knownNumbers);

            Console.ReadKey();
        }

        static int FindSector(int subsector)
        {
            int sectorX = 0;
            int sectorY = 0;
            int sector;

            switch (subsector % 9) //find the x component of the sector
            {
                case 0:
                case 1:
                case 2:
                    sectorX = 0;
                    break;
                case 3:
                case 4:
                case 5:
                    sectorX = 1;
                    break;
                case 6:
                case 7:
                case 8:
                    sectorX = 2;
                    break;
            }

            switch ((subsector - (subsector % 9)) / 9) //find the y component of the sector
            {
                case 0:
                case 1:
                case 2:
                    sectorY = 0;
                    break;
                case 3:
                case 4:
                case 5:
                    sectorY = 1;
                    break;
                case 6:
                case 7:
                case 8:
                    sectorY = 2;
                    break;
            }

            sector = 3 * sectorY + sectorX;

            return sector;
        }
        static int FindVSector(int rootSector, int order)
        {

            //order = 0 return sector with smaller id, order = 1 return sector with larger id

            int[] sectorsV = new int[2];
            int arrayIndex = 0;

            for (int j = -2; j <= 2; j++) //find vertically aligning sectors
            {
                if (rootSector + (3 * j) >= 0 && rootSector + (3 * j) < 9 && j != 0)
                {
                    sectorsV[arrayIndex] = rootSector + (3 * j);
                    arrayIndex++;
                }
            }

            if (order == 0 && sectorsV[0] > sectorsV[1])
            {
                return sectorsV[1];
            }
            else if (order == 0 && sectorsV[0] < sectorsV[1])
            {
                return sectorsV[0];
            }
            else if (order == 1 && sectorsV[0] > sectorsV[1])
            {
                return sectorsV[0];
            }
            else if (order == 1 && sectorsV[0] < sectorsV[1])
            {
                return sectorsV[1];
            }
            else
            {
                Console.WriteLine("ERROR: FindVSector");
                return 0;
            }
        }
        static int FindHSector(int rootSector, int order)
        {
            //order = 0 return sector with smaller id, order = 1 return sector with larger id

            int[] sectorsH = new int[2];
            int arrayIndex = 0;

            for (int j = -2; j <= 2; j++) //find horizontally aligning sectors
            {
                if (rootSector + j >= 0 && rootSector + j < 9 && j != 0)
                {
                    if ((rootSector + j) - ((rootSector + j) % 3) == rootSector - (rootSector % 3)) //if both sectors are at the same row
                    {
                        sectorsH[arrayIndex] = rootSector + j;
                        arrayIndex++;
                    }
                }
            }

            if (order == 0 && sectorsH[0] > sectorsH[1])
            {
                return sectorsH[1];
            }
            else if (order == 0 && sectorsH[0] < sectorsH[1])
            {
                return sectorsH[0];
            }
            else if (order == 1 && sectorsH[0] > sectorsH[1])
            {
                return sectorsH[0];
            }
            else if (order == 1 && sectorsH[0] < sectorsH[1])
            {
                return sectorsH[1];
            }
            else
            {
                Console.WriteLine("ERROR: FindHSector");
                return 0;
            }

        }
        static int[] FindSubsectors(int sector)
        {

            int[] subsectors = new int[9];
            int arrayIndex = 0;

            for (int j = 0; j < 19; j += 9)
            {
                for (int k = 0; k < 3; k++)
                {
                    subsectors[arrayIndex++] = j + k + 3 * (sector % 3) + ((sector - (sector % 3)) * 9);
                }
            }

            return subsectors;
        }
        static int[] FindColumnSubsectors(int column)
        {
            int[] columnSubsectors = new int[9];

            for (int i = 0; i < 9; i++)
            {
                columnSubsectors[i] = column + 9 * i;
            }

            return columnSubsectors;
        }
        static int[] FindRowSubsectors(int row)
        {
            int[] rowSubsectors = new int[9];

            for (int i = 0; i < 9; i++)
            {
                rowSubsectors[i] = row * 9 + i;
            }

            return rowSubsectors;
        }
        static int FindRow(int subsector)
        {
            int row = 0;

            while (subsector >= 9)
            {
                subsector -= 9;
                row++;
            }

            return row;
        }
        static int FindColumn(int subsector)
        {
            int column = subsector % 9;

            return column;
        }
        static bool CheckSector(int sector, int number, int[] knownNumbers)
        {
            int[] subsectors = FindSubsectors(sector);
            bool sectorHasNumber = false;

            for (int i = 0; i < subsectors.Length; i++)
            {
                if (knownNumbers[subsectors[i]] == number)
                {
                    sectorHasNumber = true;
                    break;
                }
            }
            return sectorHasNumber;
        }
        static bool CheckRow(int row, int number, int[] knownNumbers)
        {
            int[] rowSubsectors = new int[9];
            bool rowHasNumber = false;

            for (int i = 0; i < 9; i++) //find subsectors of the row
            {
                rowSubsectors[i] = 9 * row + i;
            }

            for (int i = 0; i < 9; i++)
            {
                if (knownNumbers[rowSubsectors[i]] == number)
                {
                    rowHasNumber = true;
                    break;
                }
            }
            return rowHasNumber;
        }
        static bool CheckColumn(int column, int number, int[] knownNumbers)
        {
            int[] columnSubsectors = new int[9];
            bool columnHasNumber = false;

            for (int i = 0; i < 9; i++) //find subsectors of the column
            {
                columnSubsectors[i] = column + 9 * i;
            }

            for (int i = 0; i < 9; i++)
            {
                if (knownNumbers[columnSubsectors[i]] == number)
                {
                    columnHasNumber = true;
                    break;
                }
            }
            return columnHasNumber;
        }
        static void DisplayBoard(int[] knownNumbers)
        {
            string top = "╔═══╤═══╤═══╦═══╤═══╤═══╦═══╤═══╤═══╗"; //layout: top-row-rowLine-row-rowLine-row-sectorLine-row-rowLine-row-rowLine-row-sectorLine-row-rowLine-row-rowLine-row-bottom
            string rowLine = "╟───┼───┼───╫───┼───┼───╫───┼───┼───╢";
            string sectorLine = "╠═══╪═══╪═══╬═══╪═══╪═══╬═══╪═══╪═══╣";
            string bottom = "╚═══╧═══╧═══╩═══╧═══╧═══╩═══╧═══╧═══╝";

            int[] rowLinePlaces = { 0, 1, 3, 4, 6, 7 };
            int[] sectorLinePlaces = { 2, 5 };

            Console.WriteLine(top);
            for (int i = 0; i < 9; i++)
            {
                Console.Write("║");
                for (int j = 0; j < 3; j++)
                {
                    Console.Write($" {knownNumbers[9 * i + 3 * j]} │ {knownNumbers[9 * i + 3 * j + 1]} │ {knownNumbers[9 * i + 3 * j + 2]} ║");
                }
                Console.WriteLine();
                if (rowLinePlaces.Contains(i))
                {
                    Console.WriteLine(rowLine);
                }

                if (sectorLinePlaces.Contains(i))
                {
                    Console.WriteLine(sectorLine);
                }
            }
            Console.WriteLine(bottom);
        }
        static int[] FindSubsectorPossibilities(int subsector, int[] possibleNumbers)
        {
            int[] subsectorPossibilities = new int[9];

            for (int i = 0; i < 9; i++)
            {
                subsectorPossibilities[i] = possibleNumbers[9 * subsector + i];
            }
            return subsectorPossibilities;
        }
        static bool CheckBoard(int[] knownNumbers)
        {

            foreach (int number in knownNumbers)
            {
                if (number == 0)
                {
                    Console.WriteLine($"CheckBoardERROR: Numbers are incomplete");
                    return false;
                }
            }

            for (int i = 0; i < 9; i++) //iterate for each sector
            {
                int[] sectorKnownNumbers = new int[9];
                int arrayIndex = 0;

                foreach (var subsector in FindSubsectors(i))
                {
                    sectorKnownNumbers[arrayIndex++] = knownNumbers[subsector];
                }

                for (int j = 1; j < 10; j++)
                {
                    if (!sectorKnownNumbers.Contains(j)) //if any one of the nine subsectors doesn't have one of the numbers
                    {
                        Console.WriteLine($"CheckBoardERROR: Sector {i} doesn't have {j}");
                        return false;
                    }
                }
            }

            for (int i = 0; i < 9; i++) //iterate for each column
            {
                int[] columnKnownNumbers = new int[9];
                int arrayIndex = 0;

                foreach (var subsector in FindColumnSubsectors(i))
                {
                    columnKnownNumbers[arrayIndex++] = knownNumbers[subsector];
                }

                for (int j = 1; j < 10; j++)
                {
                    if (!columnKnownNumbers.Contains(j)) //if any one of the nine subsectors doesn't have one of the numbers
                    {
                        Console.WriteLine($"CheckBoardERROR: Column {i} doesn't have {j}");
                        return false;
                    }
                }
            }

            for (int i = 0; i < 9; i++) //iterate for each row
            {
                int[] rowKnownNumbers = new int[9];
                int arrayIndex = 0;

                foreach (var subsector in FindRowSubsectors(i))
                {
                    rowKnownNumbers[arrayIndex++] = knownNumbers[subsector];
                }

                for (int j = 1; j < 10; j++)
                {
                    if (!rowKnownNumbers.Contains(j)) //if any one of the nine subsectors doesn't have one of the numbers
                    {
                        Console.WriteLine($"CheckBoardERROR: Row {i} doesn't have {j}");
                        return false;
                    }
                }
            }

            return true; //if none of the loops catch a mistake, then return true
        }
        static int[] EliminatePossibilities(int[] knownNumbers, int[] possibleNumbers, int subsector)
        {
            //SUBSECTOR POSSIBILITY ELIMINATION
            if (knownNumbers[subsector] != 0) //find filled subsectors
            {
                //find all known non-zero numbers and delete their possibilities from possibleNumbers[]

                for (int i = 0; i < 9; i++)
                {
                    possibleNumbers[9 * subsector + i] = 0;
                }

                //find the number's sector and delete that number from all the possibilities of that sector

                foreach (int sameSectorSubsector in FindSubsectors(FindSector(subsector)))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        possibleNumbers[9 * sameSectorSubsector + knownNumbers[subsector] - 1] = 0;
                    }
                }

                //find the number's column and delete that number from all the possibilities of that column

                foreach (int sameColumnSubsector in FindColumnSubsectors(FindColumn(subsector)))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        possibleNumbers[9 * sameColumnSubsector + knownNumbers[subsector] - 1] = 0;
                    }
                }

                //find the number's row and delete that number from all the possibilities of that row

                foreach (int sameRowSubsector in FindRowSubsectors(FindRow(subsector)))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        possibleNumbers[9 * sameRowSubsector + knownNumbers[subsector] - 1] = 0;
                    }
                }

            }

            return possibleNumbers;
        }
        static void DisplayInputBoard(string[] userInputs)
        {
            string top = "╔═══╤═══╤═══╦═══╤═══╤═══╦═══╤═══╤═══╗"; //layout: top-row-rowLine-row-rowLine-row-sectorLine-row-rowLine-row-rowLine-row-sectorLine-row-rowLine-row-rowLine-row-bottom
            string rowLine = "╟───┼───┼───╫───┼───┼───╫───┼───┼───╢";
            string sectorLine = "╠═══╪═══╪═══╬═══╪═══╪═══╬═══╪═══╪═══╣";
            string bottom = "╚═══╧═══╧═══╩═══╧═══╧═══╩═══╧═══╧═══╝";

            int[] rowLinePlaces = { 0, 1, 3, 4, 6, 7 };
            int[] sectorLinePlaces = { 2, 5 };

            Console.WriteLine(top);
            for (int i = 0; i < 9; i++)
            {
                Console.Write("║");
                for (int j = 0; j < 3; j++)
                {
                    Console.Write($" {userInputs[9 * i + 3 * j]} │ {userInputs[9 * i + 3 * j + 1]} │ {userInputs[9 * i + 3 * j + 2]} ║");
                }
                Console.WriteLine();
                if (rowLinePlaces.Contains(i))
                {
                    Console.WriteLine(rowLine);
                }

                if (sectorLinePlaces.Contains(i))
                {
                    Console.WriteLine(sectorLine);
                }
            }
            Console.WriteLine(bottom);
        }
    }
}
