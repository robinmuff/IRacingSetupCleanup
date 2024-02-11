using IRacingSetupCleanup.Classes;

class Program
{
    const int LINES_BETWEEN_MENU = 3;
    const ConsoleColor SELECTED_COLOR = ConsoleColor.Green;
    static readonly List<Tuple<string, Task>> OPTIONS =
    [
        new Tuple<string, Task>("List setups", new Task(ListSetups)),
        new Tuple<string, Task>("Delete old setups", new Task(DeleteSetups)),
        new Tuple<string, Task>("Delete empty subfolders", new Task(DeleteEmptySubfolders))
    ];
    static readonly string folderPath = @$"C:\Users\{Environment.UserName}\Documents\iRacing\setups\";

    static void Main(string[] args)
    {
        string consoleInput = "";

        while (consoleInput != "x")
        {
            ShowMenu();

            consoleInput = Console.ReadLine() ?? "";

            if (!(IsNumber(consoleInput, out int result) && IsInRange(result))) { Console.WriteLine("Invalid Input"); CreateMenuDistance(); continue; }

            ShowMenu(result, removeLastMenu: true);

            OPTIONS[result].Item2.RunSynchronously();

            CreateMenuDistance();
        }
    }

    // -> Features
    static void ListSetups()
    {
        var setupFiles = Directory
            .GetFiles(folderPath, "*.sto", SearchOption.AllDirectories)
            .Select(filePath => new Setup(filePath.Replace(folderPath, "").Split("\\")[0], Path.GetFileNameWithoutExtension(filePath)))
            .ToList();

        int spaceOfNameText = setupFiles.Max(setup => setup.CarName.Length);

        Console.WriteLine("--- LIST OF SETUPS ---");
        foreach (var setup in setupFiles) setup.ConsoleWriteLine(spaceOfNameText);
    }
    static void DeleteSetups()
    {
        Console.Write("Enter setup identifier to delete (e.g., '24S1'). Warning: Matching setups will be removed.");
        string input = Console.ReadLine() ?? "";

        List<string> filesToDelete = Directory
            .GetFiles(folderPath, "*.sto", SearchOption.AllDirectories)
            .Where(item => !item.Contains(input))
            .ToList();

        if (filesToDelete.Count == 0) { Console.WriteLine("NO FILES FOUND"); return; }

        Console.WriteLine("--- FILES TO BE DELETED ---");
        foreach (string file in filesToDelete)
        {
            Console.WriteLine(file);
        }

        Console.Write("CONFIRM WITH 'y': ");
        input = Console.ReadLine() ?? "";

        if (input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
        {
            foreach (string file in filesToDelete)
            {
                File.Delete(file);
                Console.WriteLine($"Deleted file: {file}");
            }
        }
        else
        {
            Console.WriteLine("Deletion canceled.");
        }
    }
    static void DeleteEmptySubfolders()
    {
        List<string> subfoldersToDelete = [];

        try
        {
            // Get subdirectories in the first layer
            string[] firstLayerSubfolders = Directory.GetDirectories(folderPath);

            foreach (string firstLayerSubfolder in firstLayerSubfolders)
            {
                // Get subdirectories in the first layer of the current subfolder
                string[] secondLayerSubfolders = Directory.GetDirectories(firstLayerSubfolder);

                foreach (string secondLayerSubfolder in secondLayerSubfolders)
                {
                    // Check if the second layer subfolder is empty
                    if (Directory.GetFiles(secondLayerSubfolder).Length == 0 && Directory.GetDirectories(secondLayerSubfolder).Length == 0)
                    {
                        subfoldersToDelete.Add(secondLayerSubfolder);
                    }
                }
            }

            if (subfoldersToDelete.Count == 0) { Console.WriteLine("NO FOLDERS FOUND"); return; }

            Console.WriteLine("--- FOLDERS TO BE DELETED ---");
            foreach (string folder in subfoldersToDelete) Console.WriteLine(folder);

            Console.Write("CONFIRM WITH 'y': ");
            string confirmInput = Console.ReadLine() ?? "";

            if (confirmInput.Equals("y", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (string subfolder in subfoldersToDelete)
                {
                    Directory.Delete(subfolder);
                    Console.WriteLine($"Deleted folder: {subfolder}");
                }
            }
            else
            {
                Console.WriteLine("Deletion canceled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // -> Helper
    // Menu
    static bool IsNumber(string input, out int result)
    {
        return int.TryParse(input, out result);
    }
    static bool IsInRange(int input) => input >= 0 && input < OPTIONS.Count;
    static void RemoveLastMenu()
    {
        // Remove the menu + (header + footer + current = 3)
        for (int i = 0; i < OPTIONS.Count + 3; i++)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1); // Move cursor up one line
            ClearCurrentConsoleLine(); // Clear the line
        }
    }
    static void ShowMenu(int selected = -1, bool removeLastMenu = false)
    {
        if (removeLastMenu) RemoveLastMenu();

        int longestOption = OPTIONS.Max(item => item.Item1.Length);

        // Longest text + distance for left and right design (8)
        string menuOutline = new('-', longestOption + 8);

        Console.WriteLine(menuOutline);
        foreach (string option in OPTIONS.Select(item => item.Item1))
        {
            int currentIndex = OPTIONS.Select(item => item.Item1).ToList().IndexOf(option);
            bool isSelected = currentIndex == selected;

            Console.Write("| ");

            if (isSelected) Console.ForegroundColor = SELECTED_COLOR;
            Console.Write($"({currentIndex}) {option.PadRight(longestOption)}");
            if (isSelected) Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(" |");
        }
        Console.WriteLine(menuOutline);
    }
    static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
    static void CreateMenuDistance() 
    {
        for (int i = 0; i < LINES_BETWEEN_MENU; i++) Console.WriteLine();
    }
}