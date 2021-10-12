using System;
using PageUpX.Core.Log;
using PageUpX.DataAccess.DataAccessor;
using PageUpX.DataAccess.SQLite;
using PocPuxThomas.Commons;
using PocPuxThomas.Helpers;
using PocPuxThomas.Helpers.Interfaces;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories;
using PocPuxThomas.Repositories.Interfaces;
using PocPuxThomas.ViewModels;
using PocPuxThomas.Views;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PocPuxThomas
{
    public partial class App
    {

        public static UserEntity ConnectedUser { get; set; }

        public App(IPlatformInitializer initializer) : base(initializer)
        {
        }



        protected override async void OnInitialized()
        {
            try
            {
                await NavigationService.NavigateAsync($"{Constants.NavigationPage}/{Constants.LoginPage}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            try
            {
                RegisterHelpers(containerRegistry);
                RegisterServices(containerRegistry);

                RegisterDataAscessor(containerRegistry);
                RegisterRepositories(containerRegistry);

                //Register for navigation is always the last registration method
                RegisterForNavigation(containerRegistry);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private void RegisterHelpers(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IHttpClientHelper, HttpClientHelper>();
            containerRegistry.RegisterSingleton<IDataTransferHelper, DataTransferHelper>();

        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            //Example :
            //containerRegistry.RegisterSingleton<ILoginService, LoginService>();

            containerRegistry.RegisterSingleton<IPuxLogger, ConsoleLoggerService>();
        }


        private void RegisterDataAscessor(IContainerRegistry containerRegistry)
        {
            try
            {
                containerRegistry.RegisterSingleton<IPuxSimpleDataAccessor<UserEntity>, PuxSimpleDataAccessorSQLite<UserEntity>>();
                containerRegistry.RegisterSingleton<IPuxSimpleDataAccessor<CharacterEntity>, PuxSimpleDataAccessorSQLite<CharacterEntity>>();

            }
            catch (Exception ex)
            {
                Console.WriteLine("==========================================================");
                Console.WriteLine(ex);
                Console.WriteLine("==========================================================");

            }
        }

        private void RegisterRepositories(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IUserRepository, UserRepository>();
            containerRegistry.RegisterSingleton<ICharacterRepository, CharacterRepository>();

        }

        private void RegisterForNavigation(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>(Constants.NavigationPage);


            //Here we register the page and the viewmodel to link them.
            //More, we had a name at this couple to reuse them more efficently.


            containerRegistry.RegisterForNavigation<LoginPage, LoginViewModel>(Constants.LoginPage);
            containerRegistry.RegisterForNavigation<AccountPage, AccountViewModel>(Constants.AccountPage);
            containerRegistry.RegisterForNavigation<MenuPage, MenuViewModel>(Constants.MenuPage);
            containerRegistry.RegisterForNavigation<CharacterPage, CharacterViewModel>(Constants.CharacterPage);
            containerRegistry.RegisterForNavigation<ProfilePage, ProfileViewModel>(Constants.ProfilePage);
        }


        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
