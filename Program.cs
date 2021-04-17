using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FilamentInventory
{
    class Program
    {

        /* 
            Title: (title)
            Application Type: FilamentInventory
            Description: Tracks on-hand 3D printing filament inventory and allows
                         the addition, modification, and removal of filament materials and colors.
            Author: Stephen Pickard
            Date Created: 4/9/2021
            Last Modified: 4/17/2021 
        */

        static void Main(string[] args)
        {

            // Cycles through the opening, the functioning, and closing portions of the program.

            SetTheme();

            DisplayWelcomeScreen();
            DisplayMenuScreen();
            DisplayClosingScreen();

        }

        static void SetTheme()
        {

            // Color scheme

            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
        }


        static void DisplayMenuScreen()
        {

            // Main menu screen. If no data file exists, it creates one with a sample entry which can be removed via menus.
            // User selects an option to advance through menus. This continues until the temination case is used.
            // The main menu also displays a table containing any values below a user-set threshold.

            string userOption;
            string dataPath = @"Data/Inventory.txt";
            string sample = "PLA,WHITE,1000,\n";
            bool exitApp = false;
            int warningLevel = GetWarningLevel();

            if (!File.Exists(dataPath))
            {
                File.WriteAllText(dataPath, sample);
            }

            do
            {
                Console.CursorVisible = false;

                DisplayScreenHeader("Main Menu");

                Console.WriteLine("\ta: Check Current Inventory");
                Console.WriteLine("\tb: Add New Inventory");
                Console.WriteLine("\tc: Modify Current Inventory");
                Console.WriteLine("\td: Remove From Inventory");
                Console.WriteLine("\te: Set Warning Level");
                Console.WriteLine("\tf: Exit Application");
                Console.WriteLine("\n\n\tLow inventory notifications:");
                warningLevel = GetWarningLevel();
                DisplayWarningTable(warningLevel);

                Console.WriteLine("\n\n");
                Console.WriteLine("\t\tEnter option: ");
                userOption = Console.ReadLine().ToLower();

                switch (userOption)
                {
                    case "a":
                        DisplayInventoryTable();
                        break;

                    case "b":
                        AddToInventory();
                        break;

                    case "c":
                        ModifyCurrentInventory();
                        break;

                    case "d":
                        RemoveFromInventoryMenu();
                        break;

                    case "e":
                        SetWarningLevel();
                        break;

                    case "f":
                        exitApp = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tInvalid input. Please select another option.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!exitApp);
        }

        static void DisplayWarningTable(int warningLevel)
        {

            // Generates a warning table to display on the home page.
            // The table only writes out lines below the level threshold.

            string dataPath = @"Data/Inventory.txt";
            string[,] warningTable = PopulateDataArray(dataPath);

            Console.WriteLine("Type\tColor\t\tRemaining (g)");
            Console.WriteLine("**************************************");

            for (int i = 0; i < warningTable.GetLength(0); i++)
            {
                
                for (int j = 0; j < warningTable.GetLength(1); j++)
                {
                    if (int.TryParse(warningTable[i, 2], out int level))
                    {
                        if (level <= warningLevel)
                        {
                            Console.Write(warningTable[i, j] + "\t");                          
                        }
                    }
                    
                }
                Console.WriteLine();
            }

        }

        static int GetWarningLevel()
        {

            // Retrieves the warning level. If no file exists, generates a file with a default level of 250.

            string dataPath = @"Data/Sentinel.txt";
            string warningAsString = "250";
            int warningLevel = 250;

            if (File.Exists(dataPath))
            {
                warningLevel = int.Parse(File.ReadAllText(dataPath));
            }
            else
            {
                File.WriteAllText(dataPath, warningAsString);
            }

            return warningLevel;
        }

        static int SetWarningLevel()
        {

            // Changes the current warning level to a user set one.

            string dataPath = @"Data/Sentinel.txt";
            string amountAsString;
            int amount;

            DisplayScreenHeader("Set Warning Level");

            Console.WriteLine("\tEnter an alert level, in grams, to display on the home page: ");
            amountAsString = Console.ReadLine();
            while (!int.TryParse(amountAsString, out amount))
            {
                Console.WriteLine("Invalid input. Please enter the filament's weight in grams: ");
                amountAsString = Console.ReadLine();
            }

            File.WriteAllText(dataPath, amountAsString);
            Console.WriteLine($"The warning level has been changed to {amountAsString} grams.");
            DisplayContinuePrompt();
            return amount;
        }

        static void RemoveFromInventoryMenu()
        {

            // Opening menu to remove items from inventory. Confirms whether the user would like to do this.

            string userChoice;

            DisplayScreenHeader("Remove Filament");

            Console.WriteLine("\tWould you like to remove an item from inventory? [Y/N]: ");
            userChoice = Console.ReadLine().ToUpper();

            while (userChoice != "Y" && userChoice != "N")
            {
                Console.WriteLine("Invalid input. Remove an item from inventory? [Y/N]: ");
                userChoice = Console.ReadLine().ToUpper();
            }


            if (userChoice == "Y")
            {
                RemoveFromInventory();
            }
            else

            {
                Console.WriteLine("Returning to the previous menu.");
                DisplayContinuePrompt();
                return;
            }


        }

        private static void RemoveFromInventory()
        {

            // Gathers required information for the progran to grab the correct line for removal. 
            // If that data does not exist, tells the user so. Otherwise removes data if user confirms.

            bool exitMenu = false;

            do
            {
                string materialType;
                string materialColor;
                string confirm;
                bool lineRemoved;

                DisplayScreenHeader("Remove Inventory Line");
                GenerateTableDisplay();

                Console.WriteLine("\tPlease enter a material type: ");

                materialType = Console.ReadLine().ToUpper();
                while (string.IsNullOrEmpty(materialType))
                {
                    Console.WriteLine("\tField was left blank. Enter a material type [PLA, PETG, ABS, etc.]: ");
                    materialType = Console.ReadLine().ToUpper();
                }

                Console.WriteLine("\tPlease enter the color of material of that type to remove: ");
                materialColor = Console.ReadLine().ToUpper();
                while (string.IsNullOrEmpty(materialType))
                {
                    Console.WriteLine("\tField was left blank. Enter a material type [PLA, PETG, ABS, etc.]: ");
                    materialType = Console.ReadLine().ToUpper();
                }

                Console.WriteLine($"\t{materialColor} {materialType} will be removed completely. This action cannot be undone. Continue? [Y/N]: ");
                confirm = Console.ReadLine().ToUpper();

                while (confirm != "Y" && confirm != "N")
                {
                    Console.WriteLine("\tInvalid input. Confirm item removal? [Y/N]: ");
                    confirm = Console.ReadLine().ToUpper();
                }

                if (confirm == "Y")
                {
                    lineRemoved = RemoveLine(materialType, materialColor);
                    if (lineRemoved)
                    {
                        Console.WriteLine("\tData successfully removed.");
                    }
                    else
                    {
                        Console.WriteLine("\tData to remove not found.");
                    }
                }
                else
                {
                    Console.WriteLine("\tData not removed.");
                }

                Console.WriteLine("\tRemove another line? [Y/N]: ");
                confirm = Console.ReadLine().ToUpper();
                while (confirm != "Y" && confirm != "N")
                {
                    Console.WriteLine("\tInvalid input. Remove another line? [Y/N]: ");
                    confirm = Console.ReadLine().ToUpper();
                }
                if (confirm == "N")
                {
                    exitMenu = true;
                }


            } while (!exitMenu);

        }

        static bool RemoveLine(string materialType, string materialColor)
        {

            // Removes the line from the file. The file is read into an array, then the array is transferred to a string.
            // The loop will skip over a line that contains both the material and color of the material to remove. 
            // If it does this, returns success as true. Otherwise no line was skipped, so returns success as false.
            // The string is then split back into an array and the file rewritten with the new array.

            string dataPath = @"Data/Inventory.txt";
            string removalQuery1 = materialType;
            string removalQuery2 = materialColor;
            string[] fileContents = File.ReadAllLines(dataPath);
            string newFile = "";
            bool success = false;

            foreach (string x in fileContents)
            {
                if (x.Contains(removalQuery1) && x.Contains(removalQuery2))
                {
                    success = true;
                    continue;             
                }
                else
                {
                    newFile += x + "\n";
                }
            }

            string[] newData = newFile.Split("\n");

            File.WriteAllLines(dataPath, newData);

           // string newFile = string.Join(System.Environment.NewLine, fileContents);

            return success;

        }

        static void ModifyCurrentInventory()
        {

            // Confirms that the user would like to modify current inventory.

            bool confirm = false;

            DisplayScreenHeader("Modify Inventory");

            confirm = DisplayConfirmMenu("\tWould you like to modify the current inventory? [Y/N]: ");

            if (confirm)
            {
                ModifyInventory();
            }
            else

            {
                Console.WriteLine("\tReturning to the previous menu.");
                DisplayContinuePrompt();
                return;
            }
        }

        static void ModifyInventory()
        {

            // Gathers information required to pinpoint the line to modify, as well as the desired action.
            // If lineModified returns true then the line was found and changed. Otherwise the data was not found.

            bool exitMenu = false;

            do
            {
                string materialType;
                string materialColor;
                string amount;
                string confirm;
                int totalChange;
                bool addSubtract;
                bool lineModified;

                DisplayScreenHeader("Modify Inventory Count");
                GenerateTableDisplay();

                Console.WriteLine("\tPlease enter a material type: ");

                materialType = Console.ReadLine().ToUpper();
                while (string.IsNullOrEmpty(materialType))
                {
                    Console.WriteLine("\tField was left blank. Enter a material type [PLA, PETG, ABS, etc.]: ");
                    materialType = Console.ReadLine().ToUpper();
                }

                Console.WriteLine("\tPlease enter the color of material to modify: ");
                materialColor = Console.ReadLine().ToUpper();
                while (string.IsNullOrEmpty(materialColor))
                {
                    Console.WriteLine("\tField was left blank. Enter a material color: ");
                    materialType = Console.ReadLine().ToUpper();
                }

                Console.WriteLine($"\t{materialColor} {materialType} will be modified. Continue? [Y/N]: ");
                confirm = Console.ReadLine().ToUpper();

                while (confirm != "Y" && confirm != "N")
                {
                    Console.WriteLine("\tInvalid input. Confirm inventory modification? [Y/N]: ");
                    confirm = Console.ReadLine().ToUpper();
                }

                if (confirm == "Y")
                {
                    Console.WriteLine($"\tWould you like to add or subtract to the {materialColor} {materialType} count? [A/S]: ");
                    confirm = Console.ReadLine().ToUpper();
                    while (confirm != "A" && confirm != "S")
                    {
                        Console.WriteLine("\tInvalid input. Add or subtract? [A/S]: ");
                        confirm = Console.ReadLine().ToUpper();
                    }
                    if (confirm == "A")
                    {
                        addSubtract = true;
                    }
                    else
                    {
                        addSubtract = false;
                    }

                    Console.WriteLine("\tBy how many grams?: ");
                    amount = Console.ReadLine();
                    while (!int.TryParse(amount, out totalChange))
                    {
                        Console.WriteLine("Invalid input. Please enter the filament's weight in grams: ");
                        amount= Console.ReadLine();
                    }

                    lineModified = PerformChanges(addSubtract, totalChange, materialColor, materialType);

                    if (lineModified)
                    {
                        Console.WriteLine("\tData successfully modified.");
                    }
                    else
                    {
                        Console.WriteLine("\tData to modify not found.");
                    }

                }
                else

                {
                    Console.WriteLine("Returning to the previous menu.");
                    DisplayContinuePrompt();
                    return;
                }



                Console.WriteLine("\tModify another item? [Y/N]: ");
                confirm = Console.ReadLine().ToUpper();
                while (confirm != "Y" && confirm != "N")
                {
                    Console.WriteLine("\tInvalid input. Modify another item? [Y/N]: ");
                    confirm = Console.ReadLine().ToUpper();
                }
                if (confirm == "N")
                {
                    exitMenu = true;
                }


            } while (!exitMenu);
        }

        static bool PerformChanges(bool addSubtract, int totalChange, string materialColor, string materialType)
        {

            // Grabs the file into a string array, then searches the array for the values supplied by the user.
            // If those values are found, it uses the sent bool to determine whether to add or subtract the user's value from the row.
            // As it cycles through the array is compiled into a string. 
            // The string is then split into another array with the modified value, then written back to the file.

            string dataPath = @"Data/Inventory.txt";
            string modifyQuery1 = materialType;
            string modifyQuery2 = materialColor;
            string[] fileContents = File.ReadAllLines(dataPath);
            string[] tempArray;
            string tempString;
            int tempValue;
            string newFile = "";
            bool success = false;

            foreach (string x in fileContents)
            {
                if (x.Contains(modifyQuery1) && x.Contains(modifyQuery2))
                {
                    tempArray = x.Split(',');
                    tempValue = int.Parse(tempArray[2]);
                    if (addSubtract == true)
                    {
                        tempValue += totalChange;
                        success = true;
                    }
                    else
                    {
                        tempValue -= totalChange;
                        success = true;
                        if (tempValue < 0)
                        {
                            tempValue = 0;
                        }
                    }
                    tempString = modifyQuery1 + "," + modifyQuery2 + "," + tempValue + ",";
                    newFile += "\n" + tempString;
                }
                else
                {
                    newFile += "\n" + x;
                }
            }

            string[] newData = newFile.Split("\n");

            File.WriteAllLines(dataPath, newData);

            // string newFile = string.Join(System.Environment.NewLine, fileContents);

            return success;
        }

        static void AddToInventory()
        {

            // Confirms that the user would like to add to the current inventory.

            bool confirm = false;

            DisplayScreenHeader("Add New Filament");

            confirm = DisplayConfirmMenu("Would you like to add a new roll of filament? [Y/N]: ");

            if (confirm)
            {
                DisplayAddFilamentMenu();
            }
            else

            {
                Console.WriteLine("Returning to the previous menu.");
                DisplayContinuePrompt();
                return;
            }

        }

        static void DisplayAddFilamentMenu()
        {

            // Gathers information from the user to add to the file.
            // Loops until the user opts to not add any more.

            bool exitMenu = false;

            do
            {

                string materialType;
                string materialColor;
                string weightAsString;
                string confirm;
                int weight;

                DisplayScreenHeader("Add New Filament");

                Console.WriteLine("\tEnter the type of material to add [PLA, PETG, ABS, etc.]: ");
                materialType = Console.ReadLine().ToUpper();
                while (string.IsNullOrEmpty(materialType))
                {
                    Console.WriteLine("\tField was left blank. Enter a material type [PLA, PETG, ABS, etc.]: ");
                    materialType = Console.ReadLine().ToUpper();
                }

                Console.WriteLine("\tEnter the color of the filament: ");
                materialColor = Console.ReadLine().ToUpper();
                while (string.IsNullOrEmpty(materialColor))
                {
                    Console.WriteLine("\tField was left blank. Please enter a material color: ");
                    materialColor = Console.ReadLine().ToUpper();
                }

                Console.WriteLine("\tEnter roll size in grams: ");
                weightAsString = Console.ReadLine();
                while (!int.TryParse(weightAsString, out weight))
                {
                    Console.WriteLine("Invalid input. Please enter the filament's weight in grams: ");
                    weightAsString = Console.ReadLine();
                }

                Console.WriteLine($"\t{weight} grams of {materialColor} {materialType} filament will be added as a new item. Confirm [Y/N] ");
                confirm = Console.ReadLine().ToUpper();

                while (confirm != "Y" && confirm != "N")
                {
                    Console.WriteLine("Invalid input. Confirm current values? [Y/N]: ");
                    confirm = Console.ReadLine().ToUpper();
                }

                if (confirm == "Y")
                {
                    AddNewInventoryLine(materialType, materialColor, weight);
                }
                else
                {
                    Console.WriteLine("Information not stored.");
                }

                Console.WriteLine("Would you like to add another? [Y/N]: ");
                confirm = Console.ReadLine().ToUpper();

                while (confirm != "Y" && confirm != "N")
                {
                    Console.WriteLine("Invalid input. Add another roll? [Y/N]: ");
                    confirm = Console.ReadLine().ToUpper();
                }

                if (confirm == "N")
                {
                    exitMenu = true;
                }


            } while (!exitMenu);

        }

        static void AddNewInventoryLine(string materialType, string materialColor, int weight)
        {

            // Appends the current file with a new line.

            string dataPath = @"Data/Inventory.txt";
            string inputData;

            inputData = materialType + "," + materialColor + "," + weight + ",\n";
            File.AppendAllText(dataPath, inputData);
        }

        static void DisplayInventoryTable()
        {

            // Generates a table of data from file.

            DisplayScreenHeader("Current Filament Inventory");
            GenerateTableDisplay();
            DisplayContinuePrompt();

        }


        static void GenerateTableDisplay()
        {

            // Uses PopulateDataArray to fill the dataTable array, then displays it as a table.

            string filePath = @"Data/Inventory.txt";

            string[,] dataTable = PopulateDataArray(filePath);

            Console.WriteLine("Type\tColor\t\tRemaining (g)");
            Console.WriteLine("**************************************");

            for (int i = 0; i < dataTable.GetLength(0); i++)
            {
                Console.WriteLine();
                for (int j = 0; j < dataTable.GetLength(1); j++)
                {
                    Console.Write(dataTable[i, j] + "\t\t");
                }

            }

            Console.WriteLine();
        }

        static string[,] PopulateDataArray(string fileName)
        {

            // Grabs the current file, then stores it in a multidimensional array that it then returns.

            string fileContents = File.ReadAllText(fileName);
            int rows;
            const int columns = 3;

            fileContents = fileContents.Replace('\n', '\r');

            string[] lines = fileContents.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

            rows = lines.Length;

            string[,] dataTable = new string[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < columns; c++)
                {
                    dataTable[r, c] = line_r[c];
                }


            }

            return dataTable;

        }


        static void DisplayWelcomeScreen()
        {

            // Generates a welcome screen.

            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tFilament Manager");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        static void DisplayScreenHeader(string headerText)
        {

            // Creates a header for various menus.

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }

        static void DisplayContinuePrompt()
        {

            // Pauses the screen when needed.

            Console.WriteLine();
            Console.WriteLine("\tPress any key to continue.");
            Console.ReadKey();
        }

        static void DisplayClosingScreen()
        {

            // Exit screen.

            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using Filament Manager!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        static bool DisplayConfirmMenu(string displayMessage)
        {

            // Used for confirmation menus. Converts Y and N to true and false, then returns the bool.

            string userChoice;
            bool confirm = false;

            Console.WriteLine();
            Console.WriteLine("\t\t" + displayMessage);
            Console.WriteLine();

            userChoice = Console.ReadLine().ToUpper();

            while (userChoice != "Y" && userChoice != "N")
            {
                Console.WriteLine("Invalid input. " + displayMessage);
                userChoice = Console.ReadLine().ToUpper();
            }

            if (userChoice == "Y")
            {
                confirm = true;
            }

            return confirm;

        }

    }
}
