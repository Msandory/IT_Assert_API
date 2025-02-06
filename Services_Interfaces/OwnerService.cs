using Azure.Messaging;
using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices;

namespace Inventory_System_API.Services_Interfaces
{
    public class OwnerService
    {
        private readonly DataContex _contex;
        private readonly string _ldapPath;


        public OwnerService(DataContex contex, string ldapPath)
        {
            _contex = contex;
            _ldapPath = ldapPath;   
        }

        //Getting Number of Users in the system
        public int GetNumberOfOwner()
        {
            int NumberOfOwners = _contex.Owners.Count();
            return NumberOfOwners;
        }
        //Check if Owner is in the DB
        public bool OnwerExist(int id)
        {
           return _contex.Owners.Any(l => l.id == id);
        }


        //Add Owner
        public void AddOwner(OwnerDTO ownerDto)
        {
            try
            {
                if (string.IsNullOrEmpty(ownerDto.name) || string.IsNullOrEmpty(ownerDto.department) || string.IsNullOrEmpty(ownerDto.designation))
                {
                    throw new InvalidOperationException("Fill empty Fields.");
                }

                var ownerExist = _contex.Owners.Any(l => l.name == ownerDto.name);
                if (!ownerExist)
                {
                    var owner = new Owner
                    {
                        name = ownerDto.name,
                        department = ownerDto.department,
                        designation = ownerDto.designation,
                        Filepath = ownerDto.Filepath != null ? new List<string>(ownerDto.Filepath) : new List<string>(),
                    };

                    _contex.Owners.Add(owner);
                    _contex.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException("An owner with the same name already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error: " + ex.Message);
            }
        }

        //Delete Owner
        public async Task<bool> DeleteOwner(int ownerId) 
        {
            var owner = await _contex.Owners.FindAsync(ownerId);
            if (owner == null) 
            {
                throw new Exception("Owner Not found");
            }
            owner.IsDeleted = true;
            _contex.Entry(owner).State = EntityState.Modified;
            await _contex.SaveChangesAsync();

            return true;
        }
        //Edit Owner
        public async Task<Owner>EditOwner(int id, [FromBody] OwnerDTO owner)
        {
            var OwnerUpdate = await _contex.Owners.FindAsync(id);
            if ((OwnerUpdate==null))
            {
                throw new Exception("Owner Not Found");
            }
            OwnerUpdate.name = owner.name;
            
            OwnerUpdate.department = owner.department;
            OwnerUpdate.designation = owner.designation;


            await _contex.SaveChangesAsync();
            return OwnerUpdate;
        }

        //Get List of devices for Owner
        public OwnerDevicesDTO GetOwnerDevices(int id)
        {

            var owner = _contex.Owners
          .Include(o => o.Laptop)
          .Include(o => o.Desktop)
          .Include(o => o.Mobilephones)
          .Include(o => o.Tablets)
          .FirstOrDefault(o => o.id == id);
            if (owner == null)
            {
                return null;
            }
            var dto = new OwnerDevicesDTO
            {
              
              
                Laptops = owner.Laptop.Select(l => new DeviceDTO
                {
                    ComputerName = l.ComputerName,
                    SerialNumber = l.Serialnumber,
                    Model = l.Model,
                    // Map other necessary properties
                }).ToList(),
                Desktops = owner.Desktop.Select(d => new DeviceDTO
                {
                   ComputerName = d.ComputerName,
                    SerialNumber = d.Serialnumber,
                    Model = d.Model,
                    // Map other necessary properties
                }).ToList(),
                Mobilephones = owner.Mobilephones.Select(m => new DeviceDTO
                {
                    
                    SerialNumber = m.Serialnumber,
                    Model = m.Model,
                    // Map other necessary properties
                }).ToList(),
                Tablets = owner.Tablets.Select(t => new DeviceDTO
                {
                   
                    SerialNumber = t.Serialnumber,
                    Model = t.Model,
                    // Map other necessary properties
                }).ToList()
            };
            return dto;

        }
        //for searching owners in AD,
        public IEnumerable<AdUserDto> SearchUsers(string query)
        {
            var users = new List<AdUserDto>();

            using (var entry = new DirectoryEntry(_ldapPath))
            {
                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = $"(&(objectCategory=person)(objectClass=user)(|(cn=*{query}*)(sAMAccountName=*{query}*)))";

                    searcher.PropertiesToLoad.Add("cn"); // common name
                    searcher.PropertiesToLoad.Add("sAMAccountName"); // username
                    searcher.PropertiesToLoad.Add("mail"); // email
                    searcher.PropertiesToLoad.Add("department"); // department
                    searcher.PropertiesToLoad.Add("title");

                    var results = searcher.FindAll();

                    foreach (SearchResult result in results)
                    {
                        users.Add(new AdUserDto
                        {
                            Name = result.Properties["cn"].Count > 0 ? result.Properties["cn"][0].ToString() : null,
                            UserName = result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0].ToString() : null,
                            Email = result.Properties["mail"].Count > 0 ? result.Properties["mail"][0].ToString() : null,
                            Department = result.Properties["department"].Count > 0 ? result.Properties["department"][0].ToString() : null,
                            Designation = result.Properties["title"].Count>0? result.Properties["title"][0].ToString():null
                        });
                    }
                }
            }

            return users;
        }

        //search owners in your DB
        public async Task<IEnumerable<OwnerDTO>> SearchOwnersAsync(string query)
        {
            return await _contex.Owners.AsNoTracking()
            .Where(o => o.name.Contains(query) && !o.IsDeleted)
            .Select(o => new OwnerDTO
            {
                name = o.name,
                department = o.department,
                designation = o.designation,
                Id = o.id
            })
            .ToListAsync();
        }


        //upload files for user on user page

        public async Task UpdateUserFilePathsAsync(int ID, string filePath) //Assumed to be the username since it doesn't work with Ids!
        {
            // Use FirstOrDefaultAsync with the correct property
            var owner = await _contex.Owners.FirstOrDefaultAsync(o => o.id == ID); //Find the first name it matches.

            if (owner == null)
            {
                throw new KeyNotFoundException($"Owner with ID {ID} not found");
            }

            // Ensure Filepath is not null
            owner.Filepath ??= new List<string>();

            // Add the new file path to the list, avoiding duplicates
            if (!owner.Filepath.Contains(filePath))
            {
                owner.Filepath.Add(filePath);
            }

            _contex.Owners.Update(owner); // Tell EF that the entity is modified
            await _contex.SaveChangesAsync();
        }
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _contex.Owners.AnyAsync(o => o.id == userId);
        }
    }
}
