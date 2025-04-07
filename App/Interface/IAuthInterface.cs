using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.ViewModels;

namespace App.Interface
{
    public interface IAuthInterface
    {
        Task<User> LoginAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password, string email);
        Task<bool> LogoutAsync();
        Task<User> GetUserAsync(int userId);
        Task<bool> UpdateUserAsync(Models.User user);
        Task<bool> DeleteUserAsync(int userId);
    }
}
