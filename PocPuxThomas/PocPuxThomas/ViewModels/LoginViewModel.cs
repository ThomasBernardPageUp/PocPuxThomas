using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PocPuxThomas.Commons;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using Prism.Navigation;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Command LoginCommand { get; set; }
        public Command CreateAccountCommand { get; set; }


        public LoginViewModel(INavigationService navigationService, IUserRepository userRepository) :base(navigationService)
        {
            LoginCommand = new Command(CheckLogin);
            CreateAccountCommand = new Command(AccountPage);
            _userRepository = userRepository;
        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);
        }


        public async void CheckLogin()
        {
            if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password) )
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter one username and one password", "Ok");
            }
            else
            {
                UserEntity user = await _userRepository.GetItemAsync((userEntity) => userEntity.Username == Username && userEntity.Password == Password);

                if(user != null)
                {
                    App.ConnectedUser = user;
                    await NavigationService.NavigateAsync(Constants.MenuPage);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Username or password incorrect", "Ok");
                    
                }
            }
        }

        public async void AccountPage()
        {
            await NavigationService.NavigateAsync(Constants.AccountPage);
        }

        private IUserRepository _userRepository;
    }
}
