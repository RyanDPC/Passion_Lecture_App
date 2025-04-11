using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace App.ViewModels
{
    public class User : ObservableObject
    {
        private readonly IAuthInterface _auth;
        private User _currentUser;

        public User(IAuthInterface auth)
        {
            _auth = auth;
        }
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public async Task<bool> Login(string username, string password)
        {
            CurrentUser = await _auth.LoginAsync(username, password);
            return CurrentUser != null;
        }

        public async Task<bool> Register(string username, string password, string email)
        {
            CurrentUser = await _auth.RegisterAsync(username, password, email);
            return CurrentUser != null;
        }

        public async Task<bool> UpdateUser(Models.User user)
        {
            return await _auth.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUser(int userId)
        {
            return await _auth.DeleteUserAsync(userId);
        }
    }

}
