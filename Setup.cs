namespace IRacingSetupCleanup
{
    public class Setup(string carName, string setupName)
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