using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using static Inventory_System_API.Models.ComputerCount;

public class ComputerCountService
{
    private readonly DataContex _contex;

    public ComputerCountService(DataContex contex)
    {
        _contex = contex;
    }

    public int GetNumberOfComputersInAD()
    {
        int computerCount = 0;

        using (DirectoryEntry entry = new DirectoryEntry("LDAP://logico.co.sz"))
        {
            using (DirectorySearcher searcher = new DirectorySearcher(entry))
            {
                searcher.Filter = "(objectCategory=computer)";
                searcher.SizeLimit = int.MaxValue;
                searcher.PageSize = 1000;

                SearchResultCollection results = searcher.FindAll();
                computerCount = results.Count;
            }
        }

        return computerCount;
    }

    public int GetNumberOfComputersInDatabase()
    {
        int laptopsInUseCount = _contex.Laptops.Count(l => l.Status == "In Use");
        return laptopsInUseCount ;
    }
    //Laptops in stock
    public int GetNumberOfLaptopsInStock()
    {
        int laptopsInUseCount = _contex.Laptops.Count(l => l.Status == "In Stock");
        return laptopsInUseCount;
    }
    public (int adComputerCount, int dbComputerCount) GetComputerCounts()
    {
        int adComputerCount = GetNumberOfComputersInAD();
        int dbComputerCountLaptops = GetNumberOfComputersInDatabase();

        // If you have the ComputerCount entity and want to save the counts to the database, do it here.
        var computerCount = new ComputerCount
        {
            Date = DateTime.Now,
            AdComputerCount = adComputerCount,
            DbComputerCount = dbComputerCountLaptops,
            LaptopsInUse = GetNumberOfComputersInDatabase(),
            LaptopsInStock = GetNumberOfLaptopsInStock(),
            TabletsInStock = GetNumberOfTabletsInDatabase(),
            TabletsInUse = GetNumberOfTabletsInUse(),
            MobilephonesInStock = GetNumberOfMobileInDatabase(),
            DesktopsInUse = GetNUmberOfDesktopInUse(),
            DesktopsInStock = GetNUmberOfDesktopInStock()

        };

        _contex.ComputerCounts.Add(computerCount);
        _contex.SaveChanges();

        return (adComputerCount, dbComputerCountLaptops);
    }

    public IEnumerable<ComputerCount> GetComputerCountHistory()
    {
        return _contex.ComputerCounts.ToList();
    }
    //number of tablets in stock
    public int GetNumberOfTabletsInDatabase()
    {
        int TabletCount = _contex.Tablets.Count(t => t.Status == "In Stock");
        return TabletCount;
    }
    //Number of Tablets in use
    public int GetNumberOfTabletsInUse()
    {
        int TabletCount = _contex.Tablets.Count(t => t.Status == "In Use");
        return TabletCount;
    }
    //number of mobilephones in stock
    public int GetNumberOfMobileInDatabase()
    {
        int MobileCount = _contex.Mobilephones.Count(t => t.Status == "In Stock");
        return MobileCount;
    }
    //mobilephones in use
    public int GetNumberOfMobileInUse() 
    {
        int MobilCount = _contex.Mobilephones.Count(m => m.Status == "In Use");
            return MobilCount; 
    }
    //Desktops in use
    public int GetNUmberOfDesktopInUse()
    {
        int DesktopCount = _contex.Desktops.Count(d => d.Status == "In Use");
        return DesktopCount;
    }
    //desktop in Stock
    public int GetNUmberOfDesktopInStock()
    {
        int DesktopCount = _contex.Desktops.Count(d => d.Status == "In Stock");
        return DesktopCount ;
    }


    public IEnumerable<DeviceCountDTO> GetDeviceCounts()
    {
        var laptopCount = _contex.Laptops.Count();
        var desktopCount = _contex.Desktops.Count();
        var tabletCount = _contex.Tablets.Count();
        var mobilephoneCount = _contex.Mobilephones.Count();
        return new List<DeviceCountDTO>
            {
                new DeviceCountDTO { Category = "Laptops", Count = laptopCount },
                new DeviceCountDTO { Category = "Desktops", Count = desktopCount },
                new DeviceCountDTO { Category = "Tablets", Count = tabletCount },
                new DeviceCountDTO { Category = "Mobilephones", Count = mobilephoneCount }
            };
    }



    public async Task<List<CpuTypeSummary>> GetCpuTypeSummaryAsync()
    {
        // Fetch all laptops from the database
        var laptops = await _contex.Laptops.ToListAsync();

        // Categorize the data in-memory
        var cpuTypeSummaries = laptops
            .GroupBy(l => GetCpuTypePrefix(l.CPUModel))
            .Select(g => new CpuTypeSummary
            {
                CpuType = g.Key,
                Count = g.Count()
            })
            .ToList();

        return cpuTypeSummaries;
    }
    public async Task<List<CpuTypeSummary>> GetCpuTypeSummaryAsyncDesktop()
    {
        // Fetch all Dessktop from the database
        var laptops = await _contex.Desktops.ToListAsync();

        // Categorize the data in-memory
        var cpuTypeSummaries = laptops
            .GroupBy(l => GetCpuTypePrefix(l.CPUModel))
            .Select(g => new CpuTypeSummary
            {
                CpuType = g.Key,
                Count = g.Count()
            })
            .ToList();

        return cpuTypeSummaries;
    }

    private static string GetCpuTypePrefix(string cpuModel)
    {
        if (cpuModel.Contains("i3", StringComparison.OrdinalIgnoreCase)) return "Core i3";
        if (cpuModel.Contains("i5", StringComparison.OrdinalIgnoreCase)) return "Core i5";
        if (cpuModel.Contains("i7", StringComparison.OrdinalIgnoreCase)) return "Core i7";
        if (cpuModel.Contains("AMD", StringComparison.OrdinalIgnoreCase)) return "AMD";
        // Add more cases as needed for other CPU types
        return "Other"; // Default case
    // Default case
    }
    //Get devices laptop and desktop by Make
    public async Task<List<DeviceMakeSummary>> GetDevicesByMakeAsync()
    {
        var laptopSummary = await _contex.Laptops
       .GroupBy(l => l.Make)
       .Select(g => new DeviceMakeSummary
       {
           Make = g.Key,
           DeviceType = "Laptop",
           Count = g.Count()
       })
       .ToListAsync();

        var desktopSummary = await _contex.Desktops
            .GroupBy(d => d.Make)
            .Select(g => new DeviceMakeSummary
            {
                Make = g.Key,
                DeviceType = "Desktop",
                Count = g.Count()
            })
            .ToListAsync();

        return laptopSummary.Concat(desktopSummary).ToList();

    
    }
    public async Task<List<AgeDistribution>> GetAgeDistributionAsync()
    {
        var laptops = await _contex.Laptops.ToListAsync();
        var desktops = await _contex.Desktops.ToListAsync();
        var tablets = await _contex.Tablets.ToListAsync();
        var mobilephones = await _contex.Mobilephones.ToListAsync();

        // Combine all devices
        var allDevices = laptops.Cast<object>()
            .Concat(desktops.Cast<object>())
            .Concat(tablets.Cast<object>())
            .Concat(mobilephones.Cast<object>())
            .ToList();

        // Group devices by age range and collect details
        var ageRangeData = allDevices
            .GroupBy(device =>
            {
                var dop = GetDateOfPurchase(device);
                var age = CalculateAge(dop);
                return GetAgeRange(age);
            })
            .Select(group => new AgeDistribution
            {
                AgeRange = group.Key,
                Laptop = group.Count(device => GetDeviceType(device) == "Laptop"),
                Desktop = group.Count(device => GetDeviceType(device) == "Desktop"),
                Tablet = group.Count(device => GetDeviceType(device) == "Tablet"),
                Mobile = group.Count(device => GetDeviceType(device) == "Mobilephone")
            })
            .ToList();

        return ageRangeData;
    }

    private DateOnly GetDateOfPurchase(object device)
    {
        // Use reflection or switch statements to get the DOP property value
        var dopProperty = device.GetType().GetProperty("DOP");
        if (dopProperty != null)
        {
            var dopValue = dopProperty.GetValue(device) as DateOnly?;
            return dopValue ?? DateOnly.FromDateTime(DateTime.Now);
        }
        return DateOnly.FromDateTime(DateTime.Now);
    }

    private string GetAgeRange(int age)
    {
        return age < 1 ? "Under 1 Year" :
               age == 1 ? "1 Year" :
               age == 2 ? "2 Years" :
               age == 3 ? "3 Years" :
               "4 Years and Above";
    }

    private int CalculateAge(DateOnly DOP)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - DOP.Year;
        if (DOP > today.AddYears(-age)) age--;
        return age;
    }

    private string GetDeviceType(object device)
    {
        switch (device)
        {
            case Laptop _: return "Laptop";
            case Desktop _: return "Desktop";
            case Tablets _: return "Tablet";
            case Mobilephones _: return "Mobilephone";
            default: return "Unknown";
        }
    }

    public InventoryComparisonResult CompareWithPreviousEntry(ComputerCount newEntry)
    {
        var result = new InventoryComparisonResult();

        // Get the most recent entry before the new entry
        var previousEntry = _contex.ComputerCounts
            .Where(c => c.Date < newEntry.Date)
            .OrderByDescending(c => c.Date)
            .FirstOrDefault();

        // If no previous entry exists, return with HasChanges = false
        if (previousEntry == null)
        {
            result.HasChanges = false;
            return result;
        }

        // Helper function to compare and add to results
        void CompareValue(string fieldName, int currentValue, int previousValue)
        {
            var status = currentValue > previousValue ? "Increased" :
                         currentValue < previousValue ? "Decreased" : "Equal";

            result.Details[fieldName] = new ComparisonDetail
            {
                CurrentValue = currentValue,
                PreviousValue = previousValue,
                Status = status
            };

            if (status != "Equal")
            {
                result.HasChanges = true;
            }
        }

        // Compare all fields
        CompareValue("AdComputerCount", newEntry.AdComputerCount, previousEntry.AdComputerCount);
        CompareValue("DbComputerCount", newEntry.DbComputerCount, previousEntry.DbComputerCount);
        CompareValue("DesktopsInUse", newEntry.DesktopsInUse, previousEntry.DesktopsInUse);
        CompareValue("DesktopsInStock", newEntry.DesktopsInStock, previousEntry.DesktopsInStock);
        CompareValue("LaptopsInUse", newEntry.LaptopsInUse, previousEntry.LaptopsInUse);
        CompareValue("LaptopsInStock", newEntry.LaptopsInStock, previousEntry.LaptopsInStock);
        CompareValue("TabletsInUse", newEntry.TabletsInUse, previousEntry.TabletsInUse);
        CompareValue("TabletsInStock", newEntry.TabletsInStock, previousEntry.TabletsInStock);
        CompareValue("MobilephonesInStock", newEntry.MobilephonesInStock, previousEntry.MobilephonesInStock);

        return result;
    }

    public async Task<List<DeviceStatus>> GetDeviceStatusCountsAsync(string deviceType)
    {
        return deviceType.ToLower() switch
        {
            "laptops" => await GetLaptopStatusCountsAsync(),
            "desktops" => await GetDesktopStatusCountsAsync(),
            "tablets" => await GetTabletStatusCountsAsync(),
            "mobilephones" => await GetMobileStatusCountsAsync(),
            _ => throw new ArgumentException($"Invalid device type: {deviceType}")
        };
    }

    private async Task<List<DeviceStatus>> GetLaptopStatusCountsAsync()
    {
        var query = from l in _contex.Laptops
                    group l by l.Status into g
                    select new DeviceStatus
                    {
                        Status = g.Key,
                        Count = g.Count()
                    };

        return await query.ToListAsync();
    }

    private async Task<List<DeviceStatus>> GetDesktopStatusCountsAsync()
    {
        var query = from d in _contex.Desktops
                    group d by d.Status into g
                    select new DeviceStatus
                    {
                        Status = g.Key,
                        Count = g.Count()
                    };

        return await query.ToListAsync();
    }

    private async Task<List<DeviceStatus>> GetTabletStatusCountsAsync()
    {
        var query = from t in _contex.Tablets
                    group t by t.Status into g
                    select new DeviceStatus
                    {
                        Status = g.Key,
                        Count = g.Count()
                    };

        return await query.ToListAsync();
    } private async Task<List<DeviceStatus>> GetMobileStatusCountsAsync()
    {
        var query = from t in _contex.Mobilephones
                    group t by t.Status into g
                    select new DeviceStatus
                    {
                        Status = g.Key,
                        Count = g.Count()
                    };

        return await query.ToListAsync();
    }
}



