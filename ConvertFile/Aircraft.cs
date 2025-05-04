namespace ConvertFile;

public record Aircraft(string Model, string Manufacturer, int PassengerCapacity, double Weight, double FuelCapacity);
public record GroupedAircraft(object Key, List<Aircraft> Aircrafts);

