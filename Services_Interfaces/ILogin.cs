namespace Inventory_System_API.Services_Interfaces
{
    public interface ILogin
    {
        Task<bool> CheckCredential(string username, string password);
    }
}
