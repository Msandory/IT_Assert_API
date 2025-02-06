using Inventory_System_API.Services_Interfaces;

namespace Inventory_System_API.Models
{
    public class Laptop:IDevice
    {
        public int Id { get; set; }
        public string ComputerName { get; set; } = string.Empty;
        public string Make { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string Ramsize { get; set; }
        public string OS { get; set; }
        public string CPUModel { get; set; }
        public string storagesize { get; set; }
        public string StorageType { get; set; }
        public string? Upgrades { get; set; }
        public string? Comments { get; set; }
        public DateOnly? DOP { get; set; }
        public int? Age => CalculateAge(DOP);
        public decimal? cost { get; set; }
        public string Status { get; set; }
        
        // Warranty properties
        public DateOnly? WarrantyStartDate { get; set; }
        public DateOnly? WarrantyEndDate { get; set; }
        public string? WarrantyTerms { get; set; } = string.Empty;

        // Foreign key
        public int OwnerId { get; set; }
        // Navigation property
        public Owner Owner { get; set; }

        public string? PreviousOwner { get; set; }
        private int? CalculateAge(DateOnly? dop)
        {
            if (!dop.HasValue)
            {
                return null; // Return null if DOP is null
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dop.Value.Year;

            if (dop.Value > today.AddYears(-age)) age--;

            return age;
        }
    }
}
