using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Xml.Linq;

namespace Inventory_System_API.Services_Interfaces
{
    public class MobileService
    {
        private readonly DataContex _contex;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        public MobileService(DataContex contex, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        { 
            _contex = contex;
            _configuration = configuration;
            _environment = webHostEnvironment;
        }
        //Get Mobile owner
        public MobileOwnerDTO GetMobileOwner(int mobileId)
        {
            var mobile = _contex.Mobilephones
          .Include(l => l.Owner) // Include the Owner navigation property
          .FirstOrDefault(l => l.Id == mobileId);

            if (mobile == null)
            {
                return null;
            }

            var dto = new MobileOwnerDTO
            {

                name = mobile.Owner.name,
               
                department = mobile.Owner.department,
                designation = mobile.Owner.designation,
                TabSerialnumber = mobile.Serialnumber
                // Map other necessary properties
            };

            return dto;
        }

        //Add Mobile
        public void AddMobile(MobileDTO mobileDTO)
        {
            if (_contex.Mobilephones.Any(l => l.Serialnumber == mobileDTO.Serialnumber))
            {
                throw new Exception("A Mobile with SerialNumber already Exists");
            }

            var owner = _contex.Owners.SingleOrDefault(o => o.name == mobileDTO.name);
            if (owner == null)
            {
                throw new Exception("Owner Not Found");
            }

            var Mobile = new Mobilephones
            {
                Make = mobileDTO.Make,
                Model = mobileDTO.Model,
                Serialnumber = mobileDTO.Serialnumber,
                Ramsize = mobileDTO.ramsize,
                OS = mobileDTO.OS,
                storagesize = mobileDTO.storagesize,
                DOP = mobileDTO.DOP,
                Upgrades = mobileDTO.Upgrades,
                cost = mobileDTO.cost,
                Status = mobileDTO.status,
                IMEI1 = mobileDTO.IMEI1,
                IMEI2= mobileDTO.IMEI2,
                WarrantyStartDate = mobileDTO.WarrantyStartDate,
                WarrantyEndDate = mobileDTO.WarrantyEndDate,
                WarrantyTerms = mobileDTO.WarrantyTerms,
                Comments = mobileDTO.comments,
                OwnerId = owner.id,
            };

            _contex.Mobilephones.Add(Mobile);
            _contex.SaveChanges();  
        }

        //Delete Mobile
        public void DeleteMobile(int MobileID) 
        {
            var mobile = _contex.Mobilephones.SingleOrDefault(l => l.Id == MobileID);
            if (mobile == null) { throw new Exception("No Tablet Found"); }
            _contex.Mobilephones.Remove(mobile);
            _contex.SaveChanges(); 

        }

        //Edit Tablet
        public async Task<Mobilephones>EditMobileAsync(int MobileID, [FromBody] MobileDTO tabletDTO, IFormFileCollection files)
        {
            var mobileupdate = await _contex.Mobilephones.FindAsync(MobileID);
            if (mobileupdate == null)
            {
                throw new Exception("Mobile Device Not found");
            }

            // Store the previous owner name before updating
            if (mobileupdate.OwnerId != null)
            {
                var currentOwner = await _contex.Owners
                    .FirstOrDefaultAsync(u => u.id == mobileupdate.OwnerId);

                if (currentOwner != null)
                {
                    // Check if the current owner's name is different from the previous owner's name
                    if (!currentOwner.name.Equals(mobileupdate.PreviousOwner))
                    {
                        mobileupdate.PreviousOwner = currentOwner.name;
                    }
                }
            }


            mobileupdate.Make = tabletDTO.Make;
            mobileupdate.Model = tabletDTO.Model;
            mobileupdate.Serialnumber = tabletDTO.Serialnumber;
            mobileupdate.Ramsize = tabletDTO.ramsize;
            mobileupdate.OS = tabletDTO.OS;
            mobileupdate.storagesize = tabletDTO.storagesize;
            mobileupdate.DOP = tabletDTO.DOP;
            mobileupdate.Upgrades = tabletDTO.Upgrades;
            mobileupdate.cost = tabletDTO.cost;
            mobileupdate.Status = tabletDTO.status;
            mobileupdate.IMEI2 = tabletDTO.IMEI2;
            mobileupdate.IMEI1= tabletDTO.IMEI1;
            mobileupdate.WarrantyStartDate = tabletDTO.WarrantyStartDate;
            mobileupdate.WarrantyEndDate = tabletDTO.WarrantyEndDate;
            mobileupdate.WarrantyTerms = tabletDTO.WarrantyTerms;
            mobileupdate.Comments= tabletDTO.comments;

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
            //update owner
            var ownerToUpdate = await _contex.Owners.FirstOrDefaultAsync(o => o.name == tabletDTO.name);
            if (ownerToUpdate == null)
            {
                throw new Exception("Owner Not Found");
            }
            //Update the ID for the owner
            mobileupdate.OwnerId=ownerToUpdate.id;

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

            return mobileupdate;
        }
        public bool TabletExists(int id)
        {
            return _contex.Mobilephones.Any(e => e.Id == id);
        }


    }


}
