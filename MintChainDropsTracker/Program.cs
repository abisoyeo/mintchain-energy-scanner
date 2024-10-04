using Newtonsoft.Json;
using System.Net.Http.Headers;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Program entry\n");

        Console.WriteLine("Enter Auth Key: ");
        string bearer = Console.ReadLine();

        Console.WriteLine();

        Console.Write("Enter TreeId Start Search Range: ");
        int startTreeId = int.Parse(Console.ReadLine());

        Console.Write("Enter TreeId Stop Search Range: ");
        int stopTreeId = int.Parse(Console.ReadLine());

        Console.WriteLine();

        Console.Write("Enter Minimum Drop To Search From: ");
        int minDrop = int.Parse(Console.ReadLine());

        Console.WriteLine();

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            Console.Write("Request Sent...");

            while (startTreeId < stopTreeId)
            {
                try
                {
                    var userId = await GetTreeUserIdAsync(client, startTreeId);

                    if (userId.HasValue)
                    {
                        var energyData = await GetEnergyListAsync(client, userId.Value);

                        if (energyData != null && energyData.Value.Amount > minDrop && energyData.Value.Stealable)
                        {
                            Console.WriteLine($"\nTreeId = {startTreeId}\nAmount Stealable = {energyData.Value.Amount}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing TreeId {startTreeId}: {ex.Message}");
                }

                startTreeId++;
            }
        }

        Console.WriteLine("\nEnd of range\nProgram Quitting...");
        Console.ReadLine();


    }

    private static async Task<int?> GetTreeUserIdAsync(HttpClient client, int treeId)
    {
        var response = await client.GetAsync($"https://www.mintchain.io/api/tree/user-info?treeid={treeId}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to get user info for TreeId {treeId}, status code: {response.StatusCode}");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(content);

        if (data.code == 10000)
        {
            return data.result.id;
        }

        Console.WriteLine($"TreeId {treeId}: {data.msg}");
        return null;
    }

    private static async Task<(int Amount, bool Stealable)?> GetEnergyListAsync(HttpClient client, int userId)
    {
        var response = await client.GetAsync($"https://www.mintchain.io/api/tree/steal/energy-list?id={userId}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to get energy list for UserId {userId}, status code: {response.StatusCode}");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(content);

        if (data.code == 10000 && data.result.Count > 0)
        {
            return (data.result[0].amount, data.result[0].stealable);
        }

        return null;
    }
}
