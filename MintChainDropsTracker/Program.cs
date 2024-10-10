using EnergyStealLibrary;
using Newtonsoft.Json;
using System.Net.Http.Headers;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Program entry\n");

        Console.WriteLine("Enter Auth Key: ");
        string bearer = Console.ReadLine().Trim();

        Console.WriteLine();

        Console.Write("Enter TreeId Start Search Range: ");
        int startTreeId = int.Parse(Console.ReadLine());

        Console.Write("Enter TreeId Stop Search Range: ");
        int stopTreeId = int.Parse(Console.ReadLine());

        Console.WriteLine();

        Console.Write("Enter Minimum Drop To Search From: ");
        int minDrop = int.Parse(Console.ReadLine());

        Console.WriteLine();

        // Create an instance of your service
        var energyService = new EnergyStealService();

        Console.Write("Request Sent...");

        // Set up progress reporting
        var progress = new Progress<int>(percent =>
        {
            Console.Write($"\rProgress: {percent}%");
        });

        Console.WriteLine("\n");

        // Get results and pass in the progress reporter
        var results = await energyService.GetStealableEnergy(bearer, startTreeId, stopTreeId, minDrop, progress);

        if (!string.IsNullOrEmpty(results.ErrorMessage))
        {
            Console.WriteLine("\nErrors occurred during the process:");
            Console.WriteLine($"\n{results.ErrorMessage}");
        }

        if (results.StealableEnergies.Any())
        {
            foreach (var result in results.StealableEnergies)
            {
                Console.WriteLine($"\nTreeId = {result.TreeId}\nAmount Stealable = {result.Amount}");
            }
        }
        else
        {
            Console.WriteLine("No stealable energy found in the given range.");
            // Or log error
        }

        Console.WriteLine("\nEnd of range\nProgram Quitting...");
        Console.ReadLine();
    }
}
