using System.Xml.Linq;
using ConvertFile.Models;

namespace ConvertFile.Services;

public class AircraftService
{
    public IEnumerable<Aircraft> LoadAircrafts(string path)
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

    public IEnumerable<Aircraft> Filter(IEnumerable<Aircraft> data, string field, string min)
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

    public IEnumerable<Aircraft> Sort(IEnumerable<Aircraft> data, string field) => field switch
    {
        "Model" => data.OrderBy(x => x.Model),
        "Manufacturer" => data.OrderBy(x => x.Manufacturer),
        "PassengerCapacity" => data.OrderBy(x => x.PassengerCapacity),
        "Weight" => data.OrderBy(x => x.Weight),
        "FuelCapacity" => data.OrderBy(x => x.FuelCapacity),
        _ => data
    };

    public IEnumerable<IGrouping<object, Aircraft>> Group(IEnumerable<Aircraft> data, string field) => field switch
    {
        "Model" => data.GroupBy(x => (object)x.Model),
        "Manufacturer" => data.GroupBy(x => (object)x.Manufacturer),
        "PassengerCapacity" => data.GroupBy(x => (object)x.PassengerCapacity),
        "Weight" => data.GroupBy(x => (object)x.Weight),
        "FuelCapacity" => data.GroupBy(x => (object)x.FuelCapacity),
        _ => new[] { data.GroupBy(x => (object)"Tous").First() }
    };
} 