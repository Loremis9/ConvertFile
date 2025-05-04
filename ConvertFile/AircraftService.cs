using System.Xml.Linq;

namespace ConvertFile;

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

    public IEnumerable<GroupedAircraft> Group(IEnumerable<Aircraft> data, string field)
    {
        Func<Aircraft, object> keySelector = field switch
        {
            "Model" => x => x.Model,
            "Manufacturer" => x => x.Manufacturer,
            "PassengerCapacity" => x => x.PassengerCapacity,
            "Weight" => x => x.Weight,
            "FuelCapacity" => x => x.FuelCapacity,
            _ => x => "Tous"
        };

        return data.GroupBy(keySelector)
                   .Select(g => new GroupedAircraft(g.Key, g.ToList()));
    }
} 