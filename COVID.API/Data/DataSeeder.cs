using System.Globalization;
using COVID.API.Models;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace COVID.API.Data
{
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CovidDbContext>();

            // Create database if it doesn't exist
            await context.Database.MigrateAsync();

            if (context.CovidDataPoints.Any())
            {
                Console.WriteLine("Database already seeded.");
                return; // DB has been seeded
            }

            Console.WriteLine("Seeding database from CSV files...");

            // NOTE: Place the provided CSV files in a 'CsvData' folder in your project's root directory.
            // Ensure these files are copied to the output directory by setting 'Copy to Output Directory' to 'Copy if newer' in file properties.
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var confirmedPath = Path.Combine(
                basePath,
                "CsvData",
                "time_series_covid19_confirmed_global.csv"
            );
            var deathsPath = Path.Combine(
                basePath,
                "CsvData",
                "time_series_covid19_deaths_global.csv"
            );
            var recoveredPath = Path.Combine(
                basePath,
                "CsvData",
                "time_series_covid19_recovered_global.csv"
            );

            if (
                !File.Exists(confirmedPath)
                || !File.Exists(deathsPath)
                || !File.Exists(recoveredPath)
            )
            {
                Console.WriteLine(
                    "Error: CSV files not found. Please place them in a 'CsvData' folder."
                );
                return;
            }

            var dataPoints = new Dictionary<string, CovidDataPoint>();

            // 1. Process Confirmed Cases
            ProcessFile(confirmedPath, dataPoints, (dp, val) => dp.Confirmed = val);

            // 2. Process Deaths
            ProcessFile(deathsPath, dataPoints, (dp, val) => dp.Deaths = val);

            // 3. Process Recovered Cases
            ProcessFile(recoveredPath, dataPoints, (dp, val) => dp.Recovered = val);

            await context.CovidDataPoints.AddRangeAsync(dataPoints.Values);
            await context.SaveChangesAsync();

            Console.WriteLine("Database seeding complete.");
        }

        private static void ProcessFile(
            string filePath,
            Dictionary<string, CovidDataPoint> dataPoints,
            Action<CovidDataPoint, int> updateAction
        )
        {
            Console.WriteLine($"Processing {Path.GetFileName(filePath)}...");
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();
            var dateHeaders = csv.HeaderRecord.Skip(4).ToList();

            while (csv.Read())
            {
                var province = csv.GetField("Province/State");
                var country = csv.GetField("Country/Region");

                // --- FIX STARTS HERE ---
                // Instead of directly converting, read as string first to handle empty values.
                var latString = csv.GetField("Lat");
                var lonString = csv.GetField("Long");

                // TryParse will attempt the conversion. If it fails, the value remains 0.
                double.TryParse(
                    latString,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double lat
                );
                double.TryParse(
                    lonString,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double lon
                );
                // --- FIX ENDS HERE ---

                foreach (var dateHeader in dateHeaders)
                {
                    if (DateTime.TryParse(dateHeader, CultureInfo.InvariantCulture, out var date))
                    {
                        var value = csv.GetField<int>(dateHeader);
                        var key = $"{country}-{province}-{date:yyyy-MM-dd}";

                        if (!dataPoints.TryGetValue(key, out var dataPoint))
                        {
                            dataPoint = new CovidDataPoint
                            {
                                ProvinceState = province,
                                CountryRegion = country,
                                Lat = lat, // Use the safely parsed latitude
                                Long = lon, // Use the safely parsed longitude
                                Date = date,
                            };
                            dataPoints[key] = dataPoint;
                        }
                        updateAction(dataPoint, value);
                    }
                }
            }
        }
    }
}
