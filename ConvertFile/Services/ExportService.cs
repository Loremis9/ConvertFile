using System.Text.Json;
using ConvertFile.Models;

namespace ConvertFile.Services;

public class ExportService
{
    public void Export(string path, IEnumerable<Aircraft> data, IEnumerable<string> fields)
    {
        var json = JsonSerializer.Serialize(data.Select(a =>
        {
            var dict = new Dictionary<string, object>();
            foreach (var f in fields)
            {
                if (f == "Model") dict[f] = a.Model;
                if (f == "Manufacturer") dict[f] = a.Manufacturer;
                if (f == "PassengerCapacity") dict[f] = a.PassengerCapacity;
                if (f == "Weight") dict[f] = a.Weight;
                if (f == "FuelCapacity") dict[f] = a.FuelCapacity;
            }
            return dict;
        }));
        File.WriteAllText(path, json);
    }
} 