using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        var xmlPath = Path.Combine(projectDirectory, "source", "aircraft.xml");
        
        // Afficher le contenu XML de manière formatée sans les balises
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

        var data = LoadAircrafts(xmlPath);

        // Demander si l'utilisateur souhaite filtrer
        var wantsToFilter = Ask("Souhaitez-vous filtrer les données ? (oui/non)").ToLower() == "oui";
        if (wantsToFilter)
        {
            var filter = Ask("Filtrer selon quelle propriété ?");
            
            // Afficher les valeurs disponibles pour la propriété sélectionnée
            Console.WriteLine("\nValeurs disponibles :");
            var availableValues = data.Select(a => filter switch
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

            var min = Ask($"Valeur minimum pour {filter} (laisser vide pour ignorer)");
            data = Filter(data, filter, min);
        }

        // Demander si l'utilisateur souhaite trier
        var wantsToSort = Ask("Souhaitez-vous trier les données ? (oui/non)").ToLower() == "oui";
        if (wantsToSort)
        {
            var sort = Ask("Trier selon quelle propriété ?");
            data = Sort(data, sort);
        }

        // Demander si l'utilisateur souhaite grouper
        var wantsToGroup = Ask("Souhaitez-vous grouper les données ? (oui/non)").ToLower() == "oui";
        if (wantsToGroup)
        {
            var group = Ask("Grouper selon quelle propriété ?");
            var grouped = Group(data, group);

            foreach (var g in grouped)
            {
                Console.WriteLine($"Group: {g.Key}");
                foreach (var a in g)
                    Console.WriteLine($"{a.Model}, {a.Manufacturer}, {a.PassengerCapacity}, {a.Weight}, {a.FuelCapacity}");
            }
        }
        else
        {
            // Afficher les données sans groupement
            foreach (var a in data)
                Console.WriteLine($"{a.Model}, {a.Manufacturer}, {a.PassengerCapacity}, {a.Weight}, {a.FuelCapacity}");
        }

        // Demander si l'utilisateur souhaite exporter
        var wantsToExport = AskYesNo("Souhaitez-vous exporter les données ?");
        if (wantsToExport)
        {
            var exportPath = Ask("Nom du fichier d'export (ex: export.json) :");
            var targetDirectory = Path.Combine(projectDirectory, "target");
            
            // Créer le dossier target s'il n'existe pas
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var fullExportPath = Path.Combine(targetDirectory, exportPath);
            var fields = new[] { "Model", "Manufacturer", "PassengerCapacity", "Weight", "FuelCapacity" };
            
            // Demander une seule fois les champs à exporter
            var selectedFields = new List<string>();
            foreach (var field in fields)
            {
                if (AskYesNo($"Exporter le champ {field} ?"))
                {
                    selectedFields.Add(field);
                }
            }

            Export(fullExportPath, data, selectedFields);
            Console.WriteLine($"Export terminé dans le fichier {fullExportPath}");
        }
    }

    static string Ask(string question)
    {
        Console.WriteLine(question);
        return Console.ReadLine()?.Trim() ?? "";
    }

    static bool AskYesNo(string question)
    {
        Console.WriteLine($"{question} (Appuyez sur Entrée pour Oui, n'importe quelle autre touche pour Non)");
        var key = Console.ReadKey();
        Console.WriteLine(); // Retour à la ligne après la réponse
        return key.Key == ConsoleKey.Enter;
    }

    static IEnumerable<Aircraft> LoadAircrafts(string path)
    {
        var doc = XDocument.Load(path);
        return doc.Root.Elements("Aircraft").Select(e => new Aircraft(
            e.Element("Model")?.Value,
            e.Element("Manufacturer")?.Value,
            int.Parse(e.Element("PassengerCapacity")?.Value ?? "0"),
            double.Parse(e.Element("Weight")?.Value ?? "0"),
            double.Parse(e.Element("FuelCapacity")?.Value ?? "0")
        ));
    }

    static IEnumerable<Aircraft> Filter(IEnumerable<Aircraft> data, string field, string min)
    {
        if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(min)) return data;

        return field switch
        {
            "PassengerCapacity" => data.Where(x => x.PassengerCapacity >= int.Parse(min)),
            "Weight" => data.Where(x => x.Weight >= double.Parse(min)),
            "FuelCapacity" => data.Where(x => x.FuelCapacity >= double.Parse(min)),
            _ => data.Where(x => (x.Model + x.Manufacturer).Contains(min, StringComparison.OrdinalIgnoreCase))
        };
    }

    static IEnumerable<Aircraft> Sort(IEnumerable<Aircraft> data, string field) => field switch
    {
        "Model" => data.OrderBy(x => x.Model),
        "Manufacturer" => data.OrderBy(x => x.Manufacturer),
        "PassengerCapacity" => data.OrderBy(x => x.PassengerCapacity),
        "Weight" => data.OrderBy(x => x.Weight),
        "FuelCapacity" => data.OrderBy(x => x.FuelCapacity),
        _ => data
    };

    static IEnumerable<IGrouping<object, Aircraft>> Group(IEnumerable<Aircraft> data, string field) => field switch
    {
        "Model" => data.GroupBy(x => (object)x.Model),
        "Manufacturer" => data.GroupBy(x => (object)x.Manufacturer),
        "PassengerCapacity" => data.GroupBy(x => (object)x.PassengerCapacity),
        "Weight" => data.GroupBy(x => (object)x.Weight),
        "FuelCapacity" => data.GroupBy(x => (object)x.FuelCapacity),
        _ => new[] { data.GroupBy(x => (object)"Tous").First() }
    };

    static void Export(string path, IEnumerable<Aircraft> data, IEnumerable<string> fields)
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
record Aircraft(string Model, string Manufacturer, int PassengerCapacity, double Weight, double FuelCapacity);

