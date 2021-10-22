using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;
using PocPuxThomas.Commons;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using Prism.Navigation;
using ReactiveUI;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using ZXing.Mobile;
using System.Collections.Generic;
using Newtonsoft.Json;
using PocPuxThomas.Models.DTO.Down;

namespace PocPuxThomas.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public ReactiveCommand<Unit, Task> LoginCommand { get; }
        public ReactiveCommand<Unit, Task> CreateAccountCommand { get; set; }
        public ReactiveCommand<Unit, Task> ScanCommand { get; set; }


        public LoginViewModel(INavigationService navigationService, IUserRepository userRepository) :base(navigationService)
        {
            _mobileBarcode = new MobileBarcodeScanner();
            _userRepository = userRepository;

            var canExecuteLogin = this.WhenAnyValue(vm => vm.Password, vm => vm.Username)
                .Select(query =>
                {
                    if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
                        return false;
                    else
                        return true;
                });

            LoginCommand = ReactiveCommand.Create(CheckLogin, canExecuteLogin);
            CreateAccountCommand = ReactiveCommand.Create(AccountPage);
            ScanCommand = ReactiveCommand.Create(Scan);

        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);
        }

        public async Task Scan()
        {
            var result = await _mobileBarcode.Scan();

            try
            {
                var deserializedResult = JsonConvert.DeserializeObject<UserQrCodeDownDTO>(result.Text);
                Username = deserializedResult.Username;
                Password = deserializedResult.Password;

                await CheckLogin();
            }
            catch(Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Error during scaning your QR code, please retry", "Ok");
                Console.WriteLine(ex);
            }
        }


        public async Task CheckLogin()
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
        

        public async Task AccountPage()
        {
            await NavigationService.NavigateAsync(Constants.AccountPage);
        }

        private IUserRepository _userRepository;
        private MobileBarcodeScanner _mobileBarcode;

        private string _username;
        public string Username
        {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }
    }
}
