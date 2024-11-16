using System;
using System.Text.Json;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));
        await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text).CreateAsync();

        var count = await DB.CountAsync<Item>();
        if (count == 0)
        {
            Console.WriteLine("No Data - Attempting to seed DB");
            var itemData = await File.ReadAllTextAsync("Data/auctions.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);
            await DB.SaveAsync(items);
        }
        else
        {
            Console.WriteLine("DB already has data");
        }
    }
}
