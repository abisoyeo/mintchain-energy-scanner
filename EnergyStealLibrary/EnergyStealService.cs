using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace EnergyStealLibrary
{
    public class EnergyStealService
    {
        private readonly HttpClient _client;
        private string? errorMessage = null;   // Holds error message if the request fails
        private bool isUnauthError = false;

        public EnergyStealService()
        {
            _client = new HttpClient();
        }

        // Add an optional IProgress<int> parameter to report progress
        public async Task<EnergyStealResult> GetStealableEnergy(string bearerToken, int startTreeId, int stopTreeId, int minDrop, IProgress<int> progress = null)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var result = new EnergyStealResult();
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
                            result.StealableEnergies.Add(new StealableEnergy
                            {
                                TreeId = startTreeId,
                                Amount = energyData.Value.Amount
                            });
                        }
                    }
                    else
                    {
                        result.ErrorMessage += errorMessage;
                        if (isUnauthError)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorMessage += $"Error occurred while processing tree ID {startTreeId}: {ex.Message}";
                }

                startTreeId++;
                processedCount++;

                // Report progress after processing each tree ID
                progress?.Report((processedCount * 100) / totalItems);
            }

            return result;
        }

        private async Task<int?> GetTreeUserIdAsync(int treeId)
        {
            try
            {
                var response = await _client.GetAsync($"https://www.mintchain.io/api/tree/user-info?treeid={treeId}");

                if (!response.IsSuccessStatusCode)
                {
                    errorMessage = $"Failed to get trees. Status code: {response.StatusCode}";
                    if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        isUnauthError = true ;
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(content);

                if (data.code == 10000)
                {
                    return data.result.id;
                }
                else
                {
                    errorMessage = $"Unexpected response code {data.code} for tree ID {treeId}";
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
                return null;
            }
        }

        private async Task<(int Amount, bool Stealable)?> GetEnergyListAsync(int userId)
        {
            try
            {
                var response = await _client.GetAsync($"https://www.mintchain.io/api/tree/steal/energy-list?id={userId}");

                if (!response.IsSuccessStatusCode)
                {
                    errorMessage = $"Failed to get trees. Status code: {response.StatusCode}";
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        isUnauthError = true;
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(content);

                if (data.code == 10000 && data.result.Count > 0)
                {
                    return (data.result[0].amount, data.result[0].stealable);
                }
                else
                {
                    errorMessage = data.result.Count == 0
                        ? $"No energy data found for user ID {userId}."
                        : $"Unexpected response code {data.code} for user ID {userId}";
                    return null;
                }

            }
            catch (Exception ex)
            {

                errorMessage = $"An error occurred: {ex.Message}";
                return null;
            }
        }

        public class EnergyStealResult
        {
            public List<StealableEnergy> StealableEnergies { get; set; } = new();
            public string? ErrorMessage { get; set; }
        }

        public class StealableEnergy
        {
            public int TreeId { get; set; }
            public int Amount { get; set; }
        }
    }
}
