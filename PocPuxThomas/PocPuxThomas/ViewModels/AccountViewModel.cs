using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using Prism.Navigation;
using ReactiveUI;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class AccountViewModel:BaseViewModel
    {
        public ReactiveCommand<Unit, Task> CreateCommand { get; set; }


        public AccountViewModel(INavigationService navigationService, IUserRepository userRepository) : base(navigationService)
        {
            var canExecuteCreate = this.WhenAnyValue(vm => vm.Username, vm => vm.Password, vm => vm.PictureUrl)
                .Select(query => 
                {
                    if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(PictureUrl))
                        return false;
                    else
                        return true;
                });

            CreateCommand = ReactiveCommand.Create(CreateAccount, canExecuteCreate);
            _userRepository = userRepository;
        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);
        }


        public async Task CreateAccount()
        {
            // If password or username == null
            if(String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Enter one usersname and one password", "Ok ");
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

        private string _pictureUrl;
        public string PictureUrl
        {
            get { return _pictureUrl; }
            set { this.RaiseAndSetIfChanged(ref _pictureUrl, value); }
        }

    }
}
