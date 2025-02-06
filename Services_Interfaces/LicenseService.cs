using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Numerics;

namespace Inventory_System_API.Services_Interfaces
{
    public class LicenseService
    {
        private readonly DataContex _dataContex;
        public LicenseService(DataContex dataContex) { 
        _dataContex = dataContex;
        }
        //Geting license
        public async Task<Licence> GetLicenseAsync(int id)
        {
            return await _dataContex.Licences.FirstOrDefaultAsync(l => l.id == id);
        }

        //Add license
        public void AddLicense(LicenseDTO licenseDTO)
        {

            var licenses = new Licence
            {
                product = licenseDTO.Product,
                Vendor = licenseDTO.Vendor,
                LicenseType = licenseDTO.LicenseType,
                ExpiryDate = licenseDTO.ExpiryDate,
                ContactEmail = licenseDTO.ContactEmail,
    };

          _dataContex.Licences.Add(licenses);
           _dataContex.SaveChanges();
         
        }

        //Delete License
        public void DeleteLicense(int id) 
        {
            var license = _dataContex.Licences.SingleOrDefault(license => license.id == id);
            if (license != null) {
                _dataContex.Licences.Remove(license);
                _dataContex.SaveChanges();
            }else
            {
                throw new Exception("No data found.");
            }
        }
        //Edit license
        public async Task<Licence> EditLicenseAsync(int id, [FromBody] LicenseDTO licenseDTO) 
        {
            var LicenseToUpdate = await _dataContex.Licences.FindAsync(id);
            if (LicenseToUpdate == null)
            {
                throw new KeyNotFoundException("License not found");
            }
            LicenseToUpdate.product = licenseDTO.Product;
            LicenseToUpdate.Vendor = licenseDTO.Vendor;
            LicenseToUpdate.LicenseType = licenseDTO.LicenseType;
            LicenseToUpdate.ExpiryDate = licenseDTO.ExpiryDate;
            LicenseToUpdate.ContactEmail = licenseDTO.ContactEmail;

            await _dataContex.SaveChangesAsync();
            return LicenseToUpdate;
        }
        public bool LicenseExists(int id)
        {
            return _dataContex.Licences.Any(e => e.id == id);
        }
    }
}
