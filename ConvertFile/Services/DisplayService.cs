using System.Xml.Linq;
using ConvertFile.Models;

namespace ConvertFile.Services;

public class DisplayService
{
    public void DisplayXmlContent(string xmlPath)
    {
        Console.WriteLine("Contenu du fichier XML :\n");
        
        // En-tête du tableau
        Console.WriteLine("┌──────────┬──────────────┬──────────────────┬──────────┬──────────────┐");
        Console.WriteLine("│  Model   │ Manufacturer │ PassengerCapacity│  Weight  │ FuelCapacity │");
        Console.WriteLine("├──────────┼──────────────┼──────────────────┼──────────┼──────────────┤");

        var xmlDoc = XDocument.Load(xmlPath);
        foreach (var aircraft in xmlDoc.Root.Elements("Aircraft"))
        {
            Console.WriteLine($"│ {aircraft.Element("Model")?.Value,-8} │ {aircraft.Element("Manufacturer")?.Value,-12} │ {aircraft.Element("PassengerCapacity")?.Value,16} │ {aircraft.Element("Weight")?.Value,8} │ {aircraft.Element("FuelCapacity")?.Value,12} │");
        }
        
        // Pied du tableau
        Console.WriteLine("└──────────┴──────────────┴──────────────────┴──────────┴──────────────┘");
        Console.WriteLine("\n----------------------------------------\n");
    }

    public void DisplayGroupedData(IEnumerable<IGrouping<object, Aircraft>> grouped)
    {
        foreach (var g in grouped)
        {
            Console.WriteLine($"Group: {g.Key}");
            foreach (var a in g)
                Console.WriteLine($"{a.Model}, {a.Manufacturer}, {a.PassengerCapacity}, {a.Weight}, {a.FuelCapacity}");
        }
    }

    public void DisplayData(IEnumerable<Aircraft> data)
    {
        foreach (var a in data)
            Console.WriteLine($"{a.Model}, {a.Manufacturer}, {a.PassengerCapacity}, {a.Weight}, {a.FuelCapacity}");
    }

    public void DisplayAvailableValues(IEnumerable<Aircraft> data, string field)
    {
        Console.WriteLine("\nValeurs disponibles :");
        var availableValues = data.Select(a => field switch
        {
            "Model" => a.Model,
            "Manufacturer" => a.Manufacturer,
            "PassengerCapacity" => a.PassengerCapacity.ToString(),
            "Weight" => a.Weight.ToString(),
            "FuelCapacity" => a.FuelCapacity.ToString(),
            _ => ""
        }).Distinct().OrderBy(v => v);

        foreach (var value in availableValues)
        {
            Console.WriteLine($"  - {value}");
        }
        Console.WriteLine();
    }
} 