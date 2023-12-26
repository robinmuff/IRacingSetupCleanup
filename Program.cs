using System;
using System.IO;
using System.Linq;

class Program
{
    static readonly string folderPath = @$"C:\Users\{Environment.UserName}\Documents\iRacing\setups\";

    static void Main(string[] args)
    {
        string consoleInput = "";

        while (consoleInput != "0")
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("| (0) Close                   |");
            Console.WriteLine("| (1) List setups             |");
            Console.WriteLine("| (2) Delete old setups       |");
            Console.WriteLine("| (3) Delete empty subfolders |");
            Console.WriteLine("-------------------------------");
            consoleInput = Console.ReadLine() ?? "";

            switch (consoleInput)
            {
                case "1":
                    ListSetups();
                    break;
                case "2":
                    DeleteSetups();
                    break;
                case "3":
                    DeleteEmptySubfolders();
                    break;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        }
    }

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
        Console.Write("Enter the current season (e.g., 24S1): ");
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

        Console.Write("CONFIRM WITH \"y\": ");
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

            Console.Write("CONFIRM WITH \"y\": ");
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

    class Setup(string carName, string setupName)
    {
        public string CarName { get; } = carName;
        public string SetupName { get; } = setupName;

        public void ConsoleWriteLine(int space) 
        {
            string formattedCarName = CarName.Length >= space ? CarName : CarName.PadRight(space);
            Console.WriteLine($"{formattedCarName} -> {SetupName}");
        }
    }
}