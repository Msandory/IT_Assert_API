namespace Inventory_System_API.Models
{
    public class ComputerCount
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int AdComputerCount { get; set; }
        public int DbComputerCount { get; set; }
        public int DesktopsInUse { get; set; }
        public int DesktopsInStock { get; set; }
        public int LaptopsInUse { get; set; }
        public int LaptopsInStock { get; set; }
        public int TabletsInUse { get; set; }
        public int TabletsInStock { get; set; }
        public int MobilephonesInStock { get; set; }
        public class DeviceTrend
        {
            public int LatestCount { get; set; }
            public string Trend { get; set; } // "up", "down", "stable"
        }

        // Add these classes at the top of your file, after the existing using statements
        public class InventoryComparisonResult
        {
            public bool HasChanges { get; set; }
            public Dictionary<string, ComparisonDetail> Details { get; set; }

            public InventoryComparisonResult()
            {
                Details = new Dictionary<string, ComparisonDetail>();
            }
        }

        public class ComparisonDetail
        {
            public int PreviousValue { get; set; }
            public int CurrentValue { get; set; }
            public string Status { get; set; }  // "Increased", "Decreased", or "Equal"
        }
    }
    }
