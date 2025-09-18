namespace COVID.DataAccessLayer.Models
{
    public class CovidDataPoint
    {
        public int Id { get; set; }
        public string? ProvinceState { get; set; }
        public required string CountryRegion { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public DateTime Date { get; set; }
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int? Recovered { get; set; }
    }
}
