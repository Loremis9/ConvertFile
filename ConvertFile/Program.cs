using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using ConvertFile;

class Program
{
    static void Main()
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        var xmlPath = Path.Combine(projectDirectory, "source", "aircraft.xml");

        var aircraftService = new AircraftService();
        var displayService = new DisplayService();
        var exportService = new ExportService();

        displayService.DisplayXmlContent(xmlPath);
        var data = aircraftService.LoadAircrafts(xmlPath);

 
        var wantsToFilter = Ask("Souhaitez-vous filtrer les données ? (oui/non)",true).ToLower() == "oui";
        if (wantsToFilter)
        {
            var filter = Ask("Filtrer selon quelle propriété ?", true);
            displayService.DisplayAvailableValues(data, filter);
            var min = Ask($"Valeur minimum pour {filter} (laisser vide pour ignorer)");
            data = aircraftService.Filter(data, filter, min);
        }


        var wantsToSort = Ask("Souhaitez-vous trier les données ? (oui/non)", true).ToLower() == "oui";
        if (wantsToSort)
        {
            var sort = Ask("Trier selon quelle propriété ?");
            data = aircraftService.Sort(data, sort);
        }


        var wantsToGroup = Ask("Souhaitez-vous grouper les données ? (oui/non)", true).ToLower() == "oui";
        if (wantsToGroup)
        {
            var group = Ask("Grouper selon quelle propriété ?");
            var grouped = aircraftService.Group(data, group);
            displayService.DisplayGroupedData(grouped);
        }
        else
        {
            displayService.DisplayData(data);
        }


        var wantsToExport = Ask("Souhaitez-vous exporter les données ? (oui/non)").ToLower() == "oui";
        if (wantsToExport)
        {
            var exportPath = Ask("Nom du fichier d'export (ex: export.json) :");
            var targetDirectory = Path.Combine(projectDirectory, "target");
            

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var fullExportPath = Path.Combine(targetDirectory, exportPath);
            var fields = new[] { "Model", "Manufacturer", "PassengerCapacity", "Weight", "FuelCapacity" };
            

            var selectedFields = new List<string>();
            foreach (var field in fields)
            {
                if (AskYesNo($"Exporter le champ {field} ?"))
                {
                    selectedFields.Add(field);
                }
            }

            exportService.Export(fullExportPath, data, selectedFields);
            Console.WriteLine($"Export terminé dans le fichier {fullExportPath}");
        }
    }

    static string Ask(string question, bool displayProperties = false)
    {
        if (displayProperties)
        {
            Console.WriteLine("Propriétés disponibles : Model, Manufacturer, PassengerCapacity, Weight, FuelCapacity ");
        }
        Console.WriteLine(question);
        return Console.ReadLine()?.Trim() ?? "";
    }

    static bool AskYesNo(string question)
    {
        Console.WriteLine($"{question} (Appuyez sur Entrée pour Oui, n'importe quelle autre touche pour Non)");
        var key = Console.ReadKey();
        Console.WriteLine(); 
        return key.Key == ConsoleKey.Enter;
    }
}

