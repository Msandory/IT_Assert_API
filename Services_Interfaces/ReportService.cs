using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Inventory_System_API.Services_Interfaces
{
    public class ReportService
    {
        private readonly DataContex _contex;
        private readonly IList<DeviceTypeConfig> _deviceTypes;
        public ReportService(DataContex dataContex)
        {
            _contex = dataContex;
            _deviceTypes = DiscoverDeviceTypes();
        }
        //Age Report
        public async Task<List<DeviceDetails>> GetDevicesByAgeCriteriaAsync(string deviceType, string ageRangeCriteria)
        {
            var devicesByAge = new List<DeviceDetails>();

            // Filter devices based on the device type
            if (deviceType == "Laptop" || deviceType == "All")
            {
                var laptops = await _contex.Laptops.ToListAsync();

                devicesByAge.AddRange(laptops
                    .Where(laptop => laptop.DOP.HasValue &&
                        (ageRangeCriteria == "All" || GetAgeRange(CalculateAge(laptop.DOP.Value)) == ageRangeCriteria))
                    .Select(laptop => new DeviceDetails
                    {
                        DeviceType = "Laptop",
                        Make = laptop.Make,
                        Model = laptop.Model,
                        SerialNumber = laptop.Serialnumber,
                        Age = CalculateAge(laptop.DOP.Value)
                    }));
            }

            if (deviceType == "Desktop" || deviceType == "All")
            {
                var desktops = await _contex.Desktops.ToListAsync();
                devicesByAge.AddRange(desktops
                    .Where(desktop => desktop.DOP.HasValue &&
                        (ageRangeCriteria == "All" || GetAgeRange(CalculateAge(desktop.DOP.Value)) == ageRangeCriteria))
                    .Select(desktop => new DeviceDetails
                    {
                        DeviceType = "Desktop",
                        Make = desktop.Make,
                        Model = desktop.Model,
                        SerialNumber = desktop.Serialnumber,
                        Age = CalculateAge(desktop.DOP.Value)
                    }));
            }

            if (deviceType == "Tablet" || deviceType == "All")
            {
                var tablets = await _contex.Tablets.ToListAsync();
                devicesByAge.AddRange(tablets
                    .Where(tablet => tablet.DOP.HasValue &&
                        (ageRangeCriteria == "All" || GetAgeRange(CalculateAge(tablet.DOP.Value)) == ageRangeCriteria))
                    .Select(tablet => new DeviceDetails
                    {
                        DeviceType = "Tablet",
                        Make = tablet.Make,
                        Model = tablet.Model,
                        SerialNumber = tablet.Serialnumber,
                        Age = CalculateAge(tablet.DOP.Value)
                    }));
            }

            if (deviceType == "Mobilephone" || deviceType == "All")
            {
                var mobilephones = await _contex.Mobilephones.ToListAsync();
                devicesByAge.AddRange(mobilephones
                    .Where(mobile => mobile.DOP.HasValue &&
                        (ageRangeCriteria == "All" || GetAgeRange(CalculateAge(mobile.DOP.Value)) == ageRangeCriteria))
                    .Select(mobile => new DeviceDetails
                    {
                        DeviceType = "Mobilephone",
                        Make = mobile.Make,
                        Model = mobile.Model,
                        SerialNumber = mobile.Serialnumber,
                        Age = CalculateAge(mobile.DOP.Value)
                    }));
            }

            return devicesByAge;
        }

        private string GetAgeRange(int? age)
        {
            if (!age.HasValue) return "Unknown";
            if (age < 1) return "Less than 1 year";
            if (age == 1) return "1 year";
            if (age == 2) return "2 years";
            if (age == 3) return "3 years";
            return "4+ years";
        }

        private int CalculateAge(DateOnly dateOfPurchase)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            int age = today.Year - dateOfPurchase.Year;
            if (dateOfPurchase > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }

        private List<DeviceTypeConfig> DiscoverDeviceTypes()
        {
            var deviceTypes = new List<DeviceTypeConfig>();

            var dbSetProperties = _contex.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var property in dbSetProperties)
            {
                var entityType = property.PropertyType.GetGenericArguments()[0];

                if (typeof(IDevice).IsAssignableFrom(entityType))
                {
                    var statusProperty = entityType.GetProperties()
                        .FirstOrDefault(p => p.Name.Equals("Status", StringComparison.OrdinalIgnoreCase));

                    if (statusProperty != null)
                    {
                        deviceTypes.Add(new DeviceTypeConfig
                        {
                            EntityType = entityType,
                            Name = entityType.Name,
                            DbSetPropertyName = property.Name,
                            StatusPropertyName = statusProperty.Name
                        });
                    }
                }
            }

            return deviceTypes;
        }

        public async Task<List<DeviceDetailsByStatus>> GetDevicesByStatusCriteriaAsync(string deviceType, string statusCriteria)
        {
            var devicesByStatus = new List<DeviceDetailsByStatus>();
            var typesToQuery = deviceType == "All"
                ? _deviceTypes
                : _deviceTypes.Where(t => t.Name.Equals(deviceType, StringComparison.OrdinalIgnoreCase));

            foreach (var type in typesToQuery)
            {
                var dbSetProperty = _contex.GetType().GetProperty(type.DbSetPropertyName);
                if (dbSetProperty == null) continue;

                // Get the DbSet as IQueryable<T>
                var dbSetValue = dbSetProperty.GetValue(_contex);
                if (dbSetValue == null) continue;

                // Create method to handle the specific type
                var method = typeof(ReportService)
                    .GetMethod(nameof(ProcessDeviceType), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .MakeGenericMethod(type.EntityType);

                var results = await (Task<List<DeviceDetailsByStatus>>)method.Invoke(this, new object[]
                {
                dbSetValue,
                type.Name,
                type.StatusPropertyName,
                statusCriteria
                });

                devicesByStatus.AddRange(results);
            }

            return devicesByStatus;
        }

        private async Task<List<DeviceDetailsByStatus>> ProcessDeviceType<T>(
            IQueryable<T> dbSet,
            string deviceTypeName,
            string statusPropertyName,
            string statusCriteria) where T : class, IDevice
        {
            IQueryable<T> query = dbSet;

            if (statusCriteria != "All")
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, statusPropertyName);
                var constant = Expression.Constant(statusCriteria);
                var equals = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
                query = query.Where(lambda);
            }

            // Convert to IEnumerable to avoid EF Core translation issues
            var results = await query.ToListAsync();

            return results
                .GroupBy(x => x.Status)
                .Select(g => new DeviceDetailsByStatus
                {
                    DeviceType = deviceTypeName,
                    Status = g.Key,
                    Count = g.Count() // Now Count() will work as we're working with IEnumerable
                })
                .ToList();
        }


        public async Task<List<DeviceDetailsByOS>> GetDeviceDetailsByOsAsync(string deviceType, string OS_Criteria)
        {
            var devicesByOS = new List<DeviceDetailsByOS>();
            if (deviceType == "Laptop" || deviceType == "All")
            {
                var laptops = await _contex.Laptops.ToListAsync();
                devicesByOS.AddRange(laptops
                    .Where(x => OS_Criteria == "All" || x.OS == OS_Criteria)
                    .Select(laptops => new DeviceDetailsByOS
                    {
                        DeviceType = "Laptop",
                        Make = laptops.Make,
                        Model = laptops.Model,
                        SerialNumber = laptops.Serialnumber,
                        OS = laptops.OS,


                    }));
            }
            // For Desktops
            if (deviceType == "Desktop" || deviceType == "All")
            {
                var desktops = await _contex.Desktops.ToListAsync();
                devicesByOS.AddRange(desktops
                    .Where(x => OS_Criteria == "All" || x.OS == OS_Criteria)
                    .Select(desktop => new DeviceDetailsByOS
                    {
                        DeviceType = "Desktop",
                        Make = desktop.Make,
                        Model = desktop.Model,
                        SerialNumber = desktop.Serialnumber,
                        OS = desktop.OS,
                    }));
            }

            // For Tablets
            if (deviceType == "Tablet" || deviceType == "All")
            {
                var tablets = await _contex.Tablets.ToListAsync();
                devicesByOS.AddRange(tablets
                    .Where(x => OS_Criteria == "All" || x.OS.Contains(OS_Criteria))
                    .Select(tablet => new DeviceDetailsByOS
                    {
                        DeviceType = "Tablet",
                        Make = tablet.Make,
                        Model = tablet.Model,
                        SerialNumber = tablet.Serialnumber,
                        OS = tablet.OS,
                    }));
            }

            // For Mobilephones
            if (deviceType == "Mobilephone" || deviceType == "All")
            {
                var mobilephones = await _contex.Mobilephones.ToListAsync();
                devicesByOS.AddRange(mobilephones
                    .Where(x => OS_Criteria == "All" || x.OS.Contains(OS_Criteria))
                    .Select(mobilephone => new DeviceDetailsByOS
                    {
                        DeviceType = "Mobilephone",
                        Make = mobilephone.Make,
                        Model = mobilephone.Model,
                        SerialNumber = mobilephone.Serialnumber,
                        OS = mobilephone.OS,
                    }));
            }

            return devicesByOS;
        }
        private string GetWarrantyStatus(DateOnly warrantyEndDate)
        {
            // Convert current date to DateOnly for comparison
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            // Compare warranty end date with the current date
            return warrantyEndDate >= currentDate ? "Under Warranty" : "Out of Warranty";
        }
        public async Task<List<DeviceDetailsByWarrenty>> GetDeviceDetailsByWarrentiesAsync(string deviceType, string WarrentyState)
        {
            var deviceByWarrenty = new List<DeviceDetailsByWarrenty>();

            if (deviceType == "Laptop" || deviceType == "All")
            {
                var laptops = await _contex.Laptops.ToListAsync();
                deviceByWarrenty.AddRange(laptops
                    .Where(laptop => laptop.WarrantyEndDate.HasValue &&
                                     (WarrentyState == "All" || GetWarrantyStatus(laptop.WarrantyEndDate.Value) == WarrentyState))
                    .Select(laptop => new DeviceDetailsByWarrenty
                    {
                        DeviceType = "Laptop",
                        Make = laptop.Make,
                        Model = laptop.Model,
                        SerialNumber = laptop.Serialnumber,
                        WarrentyStart = (DateOnly)laptop.WarrantyStartDate,
                        WarrentyEnd= (DateOnly)laptop.WarrantyEndDate,
                        WarrentyStatus = GetWarrantyStatus(laptop.WarrantyEndDate.Value) // Calculate warranty status
                    }));
            }

            if (deviceType == "Desktop" || deviceType == "All")
            {
                var desktops = await _contex.Desktops.ToListAsync();
                deviceByWarrenty.AddRange(desktops
                    .Where(desktop => desktop.WarrantyEndDate.HasValue &&
                                      (WarrentyState == "All" || GetWarrantyStatus(desktop.WarrantyEndDate.Value) == WarrentyState))
                    .Select(desktop => new DeviceDetailsByWarrenty
                    {
                        DeviceType = "Desktop",
                        Make = desktop.Make,
                        Model = desktop.Model,
                        SerialNumber = desktop.Serialnumber,
                        WarrentyStart = (DateOnly)desktop.WarrantyStartDate,
                        WarrentyEnd = (DateOnly)desktop.WarrantyEndDate,
                        WarrentyStatus = GetWarrantyStatus(desktop.WarrantyEndDate.Value)
                    }));
            }

            if (deviceType == "Tablet" || deviceType == "All")
            {
                var tablets = await _contex.Tablets.ToListAsync();
                deviceByWarrenty.AddRange(tablets
                    .Where(tablet => tablet.WarrantyEndDate.HasValue &&
                                     (WarrentyState == "All" || GetWarrantyStatus(tablet.WarrantyEndDate.Value) == WarrentyState))
                    .Select(tablet => new DeviceDetailsByWarrenty
                    {
                        DeviceType = "Tablet",
                        Make = tablet.Make,
                        Model = tablet.Model,
                        SerialNumber = tablet.Serialnumber,
                        WarrentyStart = (DateOnly)tablet.WarrantyStartDate,
                        WarrentyEnd = (DateOnly)tablet.WarrantyEndDate,
                        WarrentyStatus = GetWarrantyStatus(tablet.WarrantyEndDate.Value)
                    }));
            }

            if (deviceType == "Mobilephone" || deviceType == "All")
            {
                var mobilephones = await _contex.Mobilephones.ToListAsync();
                deviceByWarrenty.AddRange(mobilephones
                    .Where(mobilephone => mobilephone.WarrantyEndDate.HasValue &&
                                          (WarrentyState == "All" || GetWarrantyStatus(mobilephone.WarrantyEndDate.Value) == WarrentyState))
                    .Select(mobilephone => new DeviceDetailsByWarrenty
                    {
                        DeviceType = "Mobilephone",
                        Make = mobilephone.Make,
                        Model = mobilephone.Model,
                        SerialNumber = mobilephone.Serialnumber,
                        WarrentyStart = (DateOnly)mobilephone.WarrantyStartDate,
                        WarrentyEnd = (DateOnly)mobilephone.WarrantyEndDate,
                        WarrentyStatus = GetWarrantyStatus(mobilephone.WarrantyEndDate.Value)
                    }));
            }

      

            return deviceByWarrenty;
        }

    }
}
