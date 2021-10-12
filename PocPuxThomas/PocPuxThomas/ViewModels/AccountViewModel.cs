﻿using System;
using System.Threading.Tasks;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using Prism.Navigation;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class AccountViewModel:BaseViewModel
    {
        #region prop
        public string Username { get; set; }
        public string Password { get; set; }
        public string PictureUrl { get; set; }
        public Command PictureCommand { get; set; }
        public Command FileCommand { get; set; }
        public Command CreateCommand { get; set; }
        #endregion


        public AccountViewModel(INavigationService navigationService, IUserRepository userRepository) : base(navigationService)
        {
            CreateCommand = new Command(CreateAccount);
            _userRepository = userRepository;
        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);
        }


        public async void CreateAccount()
        {
            // If password or username == null
            if(String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
            {
                // Display error
            }
            else
            {
                try
                {
                    await _userRepository.InsertItemAsync(new UserEntity() { Password = Password, Username = Username, Picture = PictureUrl }) ;
                    await NavigationService.GoBackAsync();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private IUserRepository _userRepository;
    }
}
