using FinalProjectTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;

namespace FinalProjectTest.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            await ImportLocations(serviceProvider);
            await ImportHotels(serviceProvider);
            await ImportLocationImages(serviceProvider);
            await SeedAdminUser(serviceProvider);
        }

        public static async Task ImportLocations(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Locations.Any(l => l.Category != "Hotel"))
            {
                Console.WriteLine("📂 Landmarks already seeded. Skipping.");
                return;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Full_Data.xlsx");

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var config = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            };

            var dataSet = reader.AsDataSet(config);
            var table = dataSet.Tables[0];
            var records = new List<Location>();

            foreach (DataRow row in table.Rows)
            {
                string category = row["Category"]?.ToString()?.Trim();

                if (!string.IsNullOrWhiteSpace(category) && category.ToLower() != "hotel")
                {
                    records.Add(new Location
                    {
                        Name = row["Name"]?.ToString(),
                        Address = row["Address"]?.ToString(),
                        ShortDescription = row["ShortDescription"]?.ToString(),
                        FullDescription = row["FullDescription"]?.ToString(),
                        VisitingHours = row["VisitingHours"]?.ToString(),
                        DetailURL = row["DetailURL"]?.ToString(),
                        GoogleMapsLink = row["GoogleMapsLink"]?.ToString(),
                        Category = category,
                        ImageURL = "",
                        Attributes = "",
                        Latitude = 0,
                        Longitude = 0
                    });
                }
            }

            await context.Locations.AddRangeAsync(records);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {records.Count} landmarks seeded.");
        }

        public static async Task ImportHotels(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await context.Locations.AnyAsync(l => l.Category == "Hotel"))
            {
                Console.WriteLine("🏨 Hotels already seeded. Skipping.");
                return;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Full_Data.xlsx");

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var config = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            };

            var dataSet = reader.AsDataSet(config);
            var table = dataSet.Tables[0];
            var hotels = new List<Location>();

            foreach (DataRow row in table.Rows)
            {
                string category = row["Category"]?.ToString()?.Trim();

                if (!string.IsNullOrWhiteSpace(category) && category.ToLower() == "hotel")
                {
                    hotels.Add(new Location
                    {
                        Name = row["Name"]?.ToString(),
                        Address = row["Address"]?.ToString(),
                        ShortDescription = row["ShortDescription"]?.ToString() ?? "Book a stay at this hotel.",
                        FullDescription = row["FullDescription"]?.ToString(),
                        VisitingHours = row["VisitingHours"]?.ToString(),
                        DetailURL = row["DetailURL"]?.ToString(),
                        GoogleMapsLink = row["GoogleMapsLink"]?.ToString(),
                        Category = category,
                        ImageURL = "",
                        Attributes = "",
                        Latitude = 0,
                        Longitude = 0
                    });
                }
            }

            await context.Locations.AddRangeAsync(hotels);
            await context.SaveChangesAsync();
            Console.WriteLine($"🏨 {hotels.Count} hotels seeded.");
        }



        public static async Task ImportLocationImages(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await context.LocationImages.AnyAsync())
            {
                Console.WriteLine("🖼️ Location images already seeded. Skipping.");
                return;
            }

            int addedImages = 0;

            // 🔹 Part 1: Landmarks
            var landmarkPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Landmarks.xlsx");
            using var lStream = new FileStream(landmarkPath, FileMode.Open, FileAccess.Read);
            using var lReader = ExcelReaderFactory.CreateReader(lStream);

            var lDataSet = lReader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });

            foreach (DataRow row in lDataSet.Tables[0].Rows)
            {
                var name = row["Name"]?.ToString();
                var gallery = row["Gallery_Images"]?.ToString();

                var location = await context.Locations.FirstOrDefaultAsync(l => l.Name.Trim().ToLower() == name.Trim().ToLower());
                if (location != null && !string.IsNullOrWhiteSpace(gallery))
                {
                    var cleaned = gallery.Replace("[", "").Replace("]", "").Replace("'", "").Trim();
                    var images = cleaned.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var img in images)
                    {
                        context.LocationImages.Add(new LocationImage
                        {
                            LocationID = location.LocationID,
                            ImageURL = img.Trim()
                        });
                        addedImages++;
                    }
                }
            }

            // 🔹 Part 2: Hotels
            var hotelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Cairo_Hotels_Images_all.xlsx");
            using var hStream = new FileStream(hotelPath, FileMode.Open, FileAccess.Read);
            using var hReader = ExcelReaderFactory.CreateReader(hStream);

            var hDataSet = hReader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });

            foreach (DataRow row in hDataSet.Tables[0].Rows)
            {
                var name = row["hotel_name"]?.ToString();
                var location = await context.Locations.FirstOrDefaultAsync(l => l.Name.Trim().ToLower() == name.Trim().ToLower());

                if (location != null)
                {
                    var img1 = row["image_1"]?.ToString();
                    var img2 = row["image_2"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(img1))
                    {
                        context.LocationImages.Add(new LocationImage { LocationID = location.LocationID, ImageURL = img1.Trim() });
                        addedImages++;
                    }

                    if (!string.IsNullOrWhiteSpace(img2))
                    {
                        context.LocationImages.Add(new LocationImage { LocationID = location.LocationID, ImageURL = img2.Trim() });
                        addedImages++;
                    }
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine($"🖼️ {addedImages} total images seeded.");
        }


        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string email = "admin@example.com";
            string password = "Admin@123";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    Console.WriteLine("✅ Admin user created successfully.");
                }
                else
                {
                    Console.WriteLine("❌ Failed to create admin user:");
                    foreach (var error in result.Errors)
                        Console.WriteLine($"- {error.Description}");
                }
            }
            else
            {
                Console.WriteLine("ℹ️ Admin user already exists.");
            }
        }

        private static (double lat, double lng) GetEstimatedCoordinates(Location location)
        {
            var address = location.Address?.ToLower() ?? "";
            var category = location.Category?.ToLower() ?? "";

            if (address.Contains("giza") || category.Contains("pyramid"))
                return (29.9773, 31.1325);
            if (address.Contains("zamalek"))
                return (30.0582, 31.2190);
            if (address.Contains("coptic") || category.Contains("church"))
                return (30.0061, 31.2306);
            if (address.Contains("khan") || category.Contains("market"))
                return (30.0478, 31.2625);
            if (category.Contains("mosque"))
                return (30.0444, 31.2357);
            if (category.Contains("museum"))
                return (30.0459, 31.2243);
            if (category.Contains("hotel"))
                return (30.0500, 31.2333);

            return (30.033, 31.233);
        }

        private static string ClassifyCategory(string name, string shortDesc, string fullDesc)
        {
            var text = $"{name ?? ""} {shortDesc ?? ""} {fullDesc ?? ""}".ToLower();

            if (text.Contains("mosque"))
                return "Mosque";
            if (text.Contains("church"))
                return "Church";
            if (text.Contains("palace"))
                return "Palace";
            if (text.Contains("museum"))
                return "Museum";
            if (text.Contains("shrine"))
                return "Shrine";
            if (text.Contains("castle") || text.Contains("citadel"))
                return "Fortress";
            if (text.Contains("school") || text.Contains("madrasa"))
                return "School";
            if (text.Contains("fountain") || text.Contains("sabil"))
                return "Fountain";
            if (text.Contains("bazaar") || text.Contains("market") || text.Contains("khan") || text.Contains("souq"))
                return "Market";

            return "Historical";
        }
    }
}
