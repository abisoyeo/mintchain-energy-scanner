using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace EnergyStealLibrary
{
    public class EnergyStealService
    {
        private readonly HttpClient _client;

        public EnergyStealService(string bearerToken)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        // Add an optional IProgress<int> parameter to report progress
        public async Task<List<(int TreeId, int Amount)>> GetStealableEnergy(int startTreeId, int stopTreeId, int minDrop, IProgress<int> progress = null)
        {
            var results = new List<(int TreeId, int Amount)>();
            int totalItems = stopTreeId - startTreeId;
            int processedCount = 0;

            while (startTreeId < stopTreeId)
            {
                try
                {
                    var userId = await GetTreeUserIdAsync(startTreeId);

                    if (userId.HasValue)
                    {
                        var energyData = await GetEnergyListAsync(userId.Value);

                        if (energyData != null && energyData.Value.Amount > minDrop && energyData.Value.Stealable)
                        {
                            results.Add((startTreeId, energyData.Value.Amount));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle error using a logger
                }

                startTreeId++;
                processedCount++;

                // Report progress after processing each tree ID
                progress?.Report((processedCount * 100) / totalItems);
            }

            return results;
        }

        private async Task<int?> GetTreeUserIdAsync(int treeId)
        {
            var response = await _client.GetAsync($"https://www.mintchain.io/api/tree/user-info?treeid={treeId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(content);

            if (data.code == 10000)
            {
                return data.result.id;
            }

            return null;
        }

        private async Task<(int Amount, bool Stealable)?> GetEnergyListAsync(int userId)
        {
            var response = await _client.GetAsync($"https://www.mintchain.io/api/tree/steal/energy-list?id={userId}");

            if (!response.IsSuccessStatusCode)
            {
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
}
