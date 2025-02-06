using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using static NetTopologySuite.Geometries.Utilities.GeometryMapper;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;



namespace Inventory_System_API.Services_Interfaces
{
    public class LaptopService: ILaptopService
    {
        private readonly DataContex _contex;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public LaptopService(DataContex context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _contex = context;
            _configuration = configuration;
            _environment = webHostEnvironment;
        }

        public void AddLaptop(LaptopDto laptopDto)
        {

            // Check if ComputerName or SerialNumber already exists
            if (_contex.Laptops.Any(l => l.ComputerName == laptopDto.ComputerName))
            {
                throw new Exception("A laptop with the same Computer Name already exists.");
            }

            if (_contex.Laptops.Any(l => l.Serialnumber == laptopDto.serialnumber))
            {
                throw new Exception("A laptop with the same Serial Number already exists.");
            }

            // Find the owner by name and surname
            var owner = _contex.Owners
                .SingleOrDefault(o => o.name == laptopDto.name);

            if (owner == null)
            {
                throw new Exception("Owner not found");
            }
            var laptop = new Laptop
            {

                ComputerName = laptopDto.ComputerName,
                Make = laptopDto.Make,
                Model = laptopDto.Model,
                Serialnumber = laptopDto.serialnumber,
                Ramsize = laptopDto.ramsize,
                OS = laptopDto.OS,
                CPUModel = laptopDto.CPUModel,
                storagesize = laptopDto.storagesize,
                StorageType = laptopDto.StorageType,
                Upgrades = laptopDto.Upgrades,
                cost = laptopDto.cost,
                Status = laptopDto.Status,
                DOP = laptopDto.DOP,
                WarrantyStartDate = laptopDto.WarrantyStartDate,
                WarrantyEndDate = laptopDto.WarrantyEndDate,
                WarrantyTerms = laptopDto.WarrantyTerms,
                Comments = laptopDto.comments,
                OwnerId = owner.id,


            };

            _contex.Laptops.Add(laptop);
            _contex.SaveChanges();
        }

      

        public LaptopOwnerDTO GetLaptopOwner(int laptopId)
        {


            var laptop = _contex.Laptops
           .Include(l => l.Owner) // Include the Owner navigation property
           .FirstOrDefault(l => l.Id == laptopId);

            if (laptop == null)
            {
                return null;
            }

            var dto = new LaptopOwnerDTO
            {
                ComputerName = laptop.ComputerName,
                name = laptop.Owner.name,
                
                department = laptop.Owner.department,
                designation = laptop.Owner.designation
                // Map other necessary properties
            };

            return dto;
        }

        public void UpdateLaptop(LaptopDto laptopDto)
        {
            throw new NotImplementedException();
        }

        public void DeleteLaptop(int laptopId)
        {
            var laptop = _contex.Laptops.SingleOrDefault(l => l.Id == laptopId);
            if (laptop == null)
            {
                throw new Exception("Laptop not found.");
            }

            _contex.Laptops.Remove(laptop);
            _contex.SaveChanges();
        }
        public async Task<Laptop> EditLaptopAsync(int id, LaptopDto laptopDto, IFormFileCollection files)
        {
            // Get laptop with included Owner details 
            var laptopToUpdate = await _contex.Laptops
                .Include(l => l.Owner)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (laptopToUpdate == null)
            {
                throw new KeyNotFoundException("Laptop not found");
            }

            // Handle previous owner logic 
            if (laptopToUpdate.OwnerId != null)
            {
                var currentOwner = await _contex.Owners
                    .FirstOrDefaultAsync(u => u.id == laptopToUpdate.OwnerId);
                if (currentOwner != null && !currentOwner.name.Equals(laptopToUpdate.PreviousOwner))
                {
                    laptopToUpdate.PreviousOwner = currentOwner.name;
                }
            }

            // Update basic laptop properties 
            laptopToUpdate.ComputerName = laptopDto.ComputerName;
            laptopToUpdate.Model = laptopDto.Model;
            laptopToUpdate.Make = laptopDto.Make;
            laptopToUpdate.Serialnumber = laptopDto.serialnumber;
            laptopToUpdate.Ramsize = laptopDto.ramsize;
            laptopToUpdate.OS = laptopDto.OS;
            laptopToUpdate.CPUModel = laptopDto.CPUModel;
            laptopToUpdate.storagesize = laptopDto.storagesize;
            laptopToUpdate.StorageType = laptopDto.StorageType;
            laptopToUpdate.Upgrades = laptopDto.Upgrades;
            laptopToUpdate.DOP = laptopDto.DOP;
            laptopToUpdate.cost = laptopDto.cost;
            laptopToUpdate.Status = laptopDto.Status;
            laptopToUpdate.Comments = laptopDto.comments;
            laptopToUpdate.WarrantyStartDate = laptopDto.WarrantyStartDate;
            laptopToUpdate.WarrantyEndDate = laptopDto.WarrantyEndDate;
            laptopToUpdate.WarrantyTerms = laptopDto.WarrantyTerms;

            // File Upload Logic
            List<string> filePaths = new List<string>();
            if (files != null && files.Any())
            {
                // Get settings from configuration
                var virtualDirectoryUrl = _configuration["FileStorage:VirtualDirectoryUrl"];
                var physicalUploadPath = _configuration[$"FileStorage:PhysicalPath:{_environment.EnvironmentName}"];

                if (string.IsNullOrEmpty(virtualDirectoryUrl) || string.IsNullOrEmpty(physicalUploadPath))
                {
                    throw new Exception("File storage configuration is missing");
                }

                if (!Directory.Exists(physicalUploadPath))
                {
                    Directory.CreateDirectory(physicalUploadPath);
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string fileName = file.FileName;
                        string baseFileName = Path.GetFileNameWithoutExtension(fileName);
                        string extension = Path.GetExtension(fileName);
                        int counter = 1;

                        string physicalFilePath = Path.Combine(physicalUploadPath, fileName);

                        while (File.Exists(physicalFilePath))
                        {
                            fileName = $"{baseFileName}({counter}){extension}";
                            physicalFilePath = Path.Combine(physicalUploadPath, fileName);
                            counter++;
                        }

                        var virtualFilePath = $"{virtualDirectoryUrl}/{fileName}";

                        try
                        {
                            using (var stream = new FileStream(physicalFilePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            filePaths.Add(virtualFilePath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error saving file: {fileName}, Error: {ex.Message}");
                        }
                    }
                }
            }

            var newOwner = await _contex.Owners.FirstOrDefaultAsync(l => l.name == laptopDto.name);
            if (newOwner == null)
            {
                throw new Exception("Owner Not Found");
            }

            laptopToUpdate.OwnerId = newOwner.id;

            // If the new owner is not "Stock," handle file path updates 
            if (!newOwner.name.Equals("stock", StringComparison.OrdinalIgnoreCase))
            {
                if (newOwner.Filepath == null)
                {
                    newOwner.Filepath = new List<string>();
                }
                newOwner.Filepath.AddRange(filePaths);
                _contex.Owners.Update(newOwner);
            }

            await _contex.SaveChangesAsync();
            return laptopToUpdate;
        }
        public bool LaptopExists(int id)
        {
            return _contex.Laptops.Any(e => e.Id == id);
        }

        
    }

}




