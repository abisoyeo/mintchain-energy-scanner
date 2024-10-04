using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Net.Http.Headers;

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
        HttpResponseMessage treeIdResponseMessage = await client.GetAsync($"https://www.mintchain.io/api/tree/user-info?treeid={startTreeId}");

        var treeIdResponseBody = await treeIdResponseMessage.Content.ReadAsStringAsync();
        dynamic treeIdDataItem = JsonConvert.DeserializeObject(treeIdResponseBody);

        if (treeIdDataItem.code == 10000)
        {
            int id = treeIdDataItem.result.id;

            HttpResponseMessage energyResponseMessage = await client.GetAsync($"https://www.mintchain.io/api/tree/steal/energy-list?id={id}");

            var responseBody = await energyResponseMessage.Content.ReadAsStringAsync();

            dynamic dataItem = JsonConvert.DeserializeObject(responseBody);


            if (dataItem.code == 10000 && dataItem.result.Count > 0)
            {
                //Console.WriteLine($"Test to see program is working: TreeId = {id}\n Amount Stealable = {dataItem.result[0].amount}\n End of test");
                // check for the result amount if its > 2000 and stealable = true
                if (dataItem.result[0].amount > minDrop && dataItem.result[0].stealable == true)
                    // if it is, get the id of the tree you appended to the uri and print to the console
                    Console.WriteLine($"\nTreeId = {startTreeId}\nAmount Stealable = {dataItem.result[0].amount}");
            }
        }


        startTreeId++;
    }
}

Console.WriteLine("\nEnd of range\nProgram Quitting...");
Console.ReadLine();




