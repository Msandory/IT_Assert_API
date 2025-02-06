using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Inventory_System_API.Services_Interfaces
{

    public class DesktopService
    {
        private readonly DataContex _contex;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DesktopService(DataContex context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _contex = context;
            _configuration = configuration;
            _environment = webHostEnvironment;
        }


        //getting Desktop Onwer
            public DesktopOwnerDTO GetDesktopOwner(int laptopId)
        {   


            var desktop = _contex.Desktops
           .Include(l => l.Owner) // Include the Owner navigation property
           .FirstOrDefault(l => l.Id == laptopId);

            if (desktop == null)
            {
                return null;
            }

            var dto = new DesktopOwnerDTO
            {
                ComputerName = desktop.ComputerName,
                name = desktop.Owner.name,
               
                department = desktop.Owner.department,
                designation = desktop.Owner.designation
                // Map other necessary properties
            };

            return dto;
        }

        //Add Desktop
        public void AddDesktop(DesktopDto desktopDto) {
            // Check if ComputerName or SerialNumber already exists
            if (_contex.Desktops.Any(l => l.ComputerName == desktopDto.ComputerName))
            {
                throw new Exception("A Desktop with the same Computer Name already exists.");
            }

            if (_contex.Desktops.Any(l => l.Serialnumber == desktopDto.serialnumber))
            {
                throw new Exception("A Desktop with the same Serial Number already exists.");
            }

            // Find the owner by name and surname
            var owner = _contex.Owners
                .SingleOrDefault(o => o.name == desktopDto.name );

            if (owner == null)
            {
                throw new Exception("Owner not found");
            }
            var Desktops = new Desktop
            {

                ComputerName = desktopDto.ComputerName,
                Make = desktopDto.Make,
                Model = desktopDto.Model,
                Serialnumber = desktopDto.serialnumber,
                Ramsize = desktopDto.ramsize,
                OS = desktopDto.OS,
                CPUModel = desktopDto.CPUModel,
                storagesize = desktopDto.storagesize,
                StorageType = desktopDto.StorageType,
                Upgrades = desktopDto.Upgrades,
                cost = desktopDto.cost,
                Status = desktopDto.Status,
                DOP = desktopDto.DOP,
                Comments= desktopDto.comments,
                WarrantyStartDate = desktopDto.WarrantyStartDate,
                WarrantyEndDate = desktopDto.WarrantyEndDate,
                WarrantyTerms = string.IsNullOrEmpty(desktopDto.WarrantyTerms) ? string.Empty : desktopDto.WarrantyTerms,
                OwnerId = owner.id,


            };

            _contex.Desktops.Add(Desktops);
            _contex.SaveChanges();
        }


        //Delete Desktop
        public void DeleteDesktop(int desktopID)
        {
            var desktop = _contex.Desktops.SingleOrDefault(l => l.Id == desktopID);
            if (desktop == null)
            {
                throw new Exception("Laptop not found.");
            }

            _contex.Desktops.Remove(desktop);
            _contex.SaveChanges();
        }
        //Update laptop
        public async Task<Desktop> EditDesktopAsync(int id, [FromBody] DesktopDto desktopDto, IFormFileCollection files)
        {
            var laptopToUpdate = await _contex.Desktops.FindAsync(id);
            if (laptopToUpdate == null)
            {
                throw new KeyNotFoundException("Laptop not found");
            }

            // Store the previous owner name before updating
            if (laptopToUpdate.OwnerId != null)
            {
                var currentOwner = await _contex.Owners
                    .FirstOrDefaultAsync(u => u.id == laptopToUpdate.OwnerId);

                if (currentOwner != null)
                {
                    // Check if the current owner's name is different from the previous owner's name
                    if (!currentOwner.name.Equals(laptopToUpdate.PreviousOwner))
                    {
                        laptopToUpdate.PreviousOwner = currentOwner.name;
                    }
                }
            }
            // Update the properties of the laptop entity
            laptopToUpdate.ComputerName = desktopDto.ComputerName;
            laptopToUpdate.Model = desktopDto.Model;
            laptopToUpdate.Make = desktopDto.Make;
            laptopToUpdate.Serialnumber = desktopDto.serialnumber;
            laptopToUpdate.Ramsize = desktopDto.ramsize;
            laptopToUpdate.OS = desktopDto.OS;
            laptopToUpdate.CPUModel = desktopDto.CPUModel;
            laptopToUpdate.storagesize = desktopDto.storagesize;
            laptopToUpdate.StorageType = desktopDto.StorageType;
            laptopToUpdate.Upgrades = desktopDto.Upgrades;
            laptopToUpdate.DOP = desktopDto.DOP;
            laptopToUpdate.cost = desktopDto.cost;
            laptopToUpdate.Status = desktopDto.Status;
            laptopToUpdate.Comments= desktopDto.comments;
            laptopToUpdate.WarrantyStartDate = desktopDto.WarrantyStartDate;
            laptopToUpdate.WarrantyEndDate = desktopDto.WarrantyEndDate;
            laptopToUpdate.WarrantyTerms = desktopDto.WarrantyTerms;
            // laptopToUpdate.Owner.name = desktopDto.name;
            //  laptopToUpdate.Owner.surname = desktopDto.surname;
            // laptopToUpdate.OwnerId = desktopDto.OwnerId;

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

            // Optionally update the Owner entity if needed
            var ownerToUpdate = await _contex.Owners.FirstOrDefaultAsync(d => d.name == desktopDto.name);
            
            if (ownerToUpdate == null)
            {
                throw new Exception("Owner Not Found");

            }
            laptopToUpdate.OwnerId = ownerToUpdate.id; // Assuming desktodto contains OwnerName and other owner properties

            // If the new owner is not "Stock," handle file path updates 
            if (!ownerToUpdate.name.Equals("stock", StringComparison.OrdinalIgnoreCase))
            {
                if (ownerToUpdate.Filepath == null)
                {
                    ownerToUpdate.Filepath = new List<string>();
                }
                ownerToUpdate.Filepath.AddRange(filePaths);
                _contex.Owners.Update(ownerToUpdate);
            }

            // Save the changes to the database
            await _contex.SaveChangesAsync();

            return laptopToUpdate;
        }

        public bool DesktopExists(int id)
        {
            return _contex.Desktops.Any(e => e.Id == id);
        }
    }
}

