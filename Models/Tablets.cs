using Inventory_System_API.Services_Interfaces;

namespace Inventory_System_API.Models
{
    public class Tablets :IDevice
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string Ramsize { get; set; }
        public string OS { get; set; }
        public string storagesize { get; set; }
        public string? Comments { get; set; }
        public string? Upgrades { get; set; }
        public DateOnly? DOP { get; set; }
        // Calculated property
        public int? Age => CalculateAge(DOP);
        public decimal cost { get; set; }
        // Warranty properties
        public DateOnly? WarrantyStartDate { get; set; }
        public DateOnly? WarrantyEndDate { get; set; }
        public string? WarrantyTerms { get; set; } = string.Empty;
        public string Status { get; set; }
        
        // Foreign key
        public int OwnerId { get; set; }
        // Navigation property
        public Owner Owner { get; set; }

        public string? PreviousOwner { get; set; }

        // calcaulate Age of Laptop
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
