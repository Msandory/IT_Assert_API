using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Configuration;

namespace Inventory_System_API.Services_Interfaces
{
   
    public class TabletService
    {
        private readonly DataContex _contex;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        public TabletService(DataContex contex, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _contex = contex;
            _configuration = configuration;
            _environment = webHostEnvironment;
        }
        ///getting count of tablets
        public TabletOwnerDTO GetTabletOwner(int TabletID)
        {
            var tablets = _contex.Tablets
           .Include(l => l.Owner) // Include the Owner navigation property
           .FirstOrDefault(l => l.Id == TabletID);

            if (tablets == null)
            {
                return null;
            }

            var dto = new TabletOwnerDTO
            {
                id= TabletID,
                name = tablets.Owner.name,
                
                department = tablets.Owner.department,
                designation = tablets.Owner.designation,
                TabSerialnumber = tablets.Serialnumber
                // Map other necessary properties
            };

            return dto;
        }

        //Add tablet
        public void AddTablet(TabletDTO tabletDTO)
        {
            if (_contex.Tablets.Any(l => l.Serialnumber == tabletDTO.Serialnumber)) {
                throw new Exception("A Tablet with the same Serial Number already exists.");
            }
            // Find the owner by name and surname
            var owner = _contex.Owners
                .SingleOrDefault(o => o.name == tabletDTO.name);

            if (owner == null)
            {
                throw new Exception("Owner not found");
            }
            var Tablet = new Tablets
            {

               
                Make = tabletDTO.Make,
                Model = tabletDTO.Model,
                Serialnumber = tabletDTO.Serialnumber,
                Ramsize = tabletDTO.ramsize,
                OS = tabletDTO.OS,
                storagesize = tabletDTO.storagesize,
                Comments = tabletDTO.comments,
                Upgrades = tabletDTO.Upgrades,
                cost = tabletDTO.cost,
                Status = tabletDTO.status,
                DOP = tabletDTO.DOP,
                WarrantyStartDate = tabletDTO.WarrantyStartDate,
                WarrantyEndDate = tabletDTO.WarrantyEndDate,
                WarrantyTerms = tabletDTO.WarrantyTerms,
                OwnerId = owner.id,


            };

            _contex.Tablets.Add(Tablet);
            _contex.SaveChanges();
        }

        //Delete Tablet
        public void DeleteTablet(int tabletID)
        {
            var tablet = _contex.Tablets.SingleOrDefault(l=>l.Id == tabletID);
            if (tablet == null )
            {
                throw new Exception("No tablet found");
            }
            _contex.Tablets.Remove(tablet);
            _contex.SaveChanges();
        }


        //Edit tablet
        public async Task<Tablets>EditTabletsAsync(int id, [FromBody] TabletDTO tabletDTO, IFormFileCollection files)
        {
            var tabletUpdate = await _contex.Tablets.FindAsync(id);
            if(tabletUpdate == null )
            {
                throw new Exception("Tablet Not Found");
            }


            // Store the previous owner name before updating
            if (tabletUpdate.OwnerId != null)
            {
                var currentOwner = await _contex.Owners
                    .FirstOrDefaultAsync(u => u.id == tabletUpdate.OwnerId);

                if (currentOwner != null)
                {
                    // Check if the current owner's name is different from the previous owner's name
                    if (!currentOwner.name.Equals(tabletUpdate.PreviousOwner))
                    {
                        tabletUpdate.PreviousOwner = currentOwner.name;
                    }
                }
            }
            tabletUpdate.Make = tabletDTO.Make;
            tabletUpdate.Model = tabletDTO.Model;
            tabletUpdate.Serialnumber = tabletDTO.Serialnumber;
            tabletUpdate.Ramsize = tabletDTO.ramsize;
            tabletUpdate.OS = tabletDTO.OS;
            tabletUpdate.storagesize = tabletDTO.storagesize;
            tabletUpdate.Comments= tabletDTO.comments;
            tabletUpdate.Upgrades = tabletDTO.Upgrades;
            tabletUpdate.cost = tabletDTO.cost;
            tabletUpdate.Status = tabletDTO.status;
            tabletUpdate.DOP= tabletDTO.DOP;
            tabletUpdate.WarrantyStartDate = tabletDTO.WarrantyStartDate;
            tabletUpdate.WarrantyEndDate = tabletDTO.WarrantyEndDate;
            tabletUpdate.WarrantyTerms = tabletDTO.WarrantyTerms;
            //tabletUpdate.OwnerId = tabletDTO.Owner.id;


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
            // Find the new owner by name
            var newOwner = await _contex.Owners
                .FirstOrDefaultAsync(o => o.name == tabletDTO.name);

            if (newOwner == null)
            {
                throw new Exception("Owner Not Found");
            }

            // Update the tablet's OwnerId to the new owner's ID
            tabletUpdate.OwnerId = newOwner.id;
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

            // Save the changes to the database
            await _contex.SaveChangesAsync();

            return tabletUpdate;


        }
        public bool TabletExists(int id)
        {
            return _contex.Tablets.Any(e => e.Id == id);
        }

    }
}
