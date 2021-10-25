using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Prism.Commands;
using PocPuxThomas.Commons;
using ReactiveUI;
using DynamicData;
using System.Reactive.Linq;
using System.Reactive;
using PocPuxThomas.Models.DTO.Down;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public ReactiveCommand<CharacterEntity, Task> DeleteCommand { get; set; }
        public ReactiveCommand<CharacterEntity, Task> ViewCommand { get; set; }
        public ReactiveCommand<Unit, Task> GenerateQrCodeCommand { get; set; }
        public UserEntity UserEntity { get; set; }
        private ICharacterRepository _characterRepository;

        ProfileViewModel(INavigationService navigationService, ICharacterRepository characterRepository):base(navigationService)
        {
            UserEntity = App.ConnectedUser;

            _characterRepository = characterRepository;
            DeleteCommand = ReactiveCommand.Create<CharacterEntity, Task>(async characterEntity => await DeleteCharacter(characterEntity));
            ViewCommand = ReactiveCommand.Create<CharacterEntity, Task>(async characterEntity => await ViewCharacter(characterEntity));
            GenerateQrCodeCommand = ReactiveCommand.Create<Unit, Task>(async c => await GenerateQrCode());

            _charactersCache.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out _characters).Subscribe();
        }

        public async Task GenerateQrCode()
        {
            UserQrCodeDownDTO userQrCodeDownDTO = new UserQrCodeDownDTO() { Password = UserEntity.Password, Username = UserEntity.Username };
            QrCodeValue = JsonConvert.SerializeObject(userQrCodeDownDTO);
        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            var charactersFromDatabase = await _characterRepository.GetItemsAsync((characterEntity) => characterEntity.IdCreator == App.ConnectedUser.Id);
            _charactersCache.AddOrUpdate(charactersFromDatabase);
        }

        public async Task DeleteCharacter(CharacterEntity characterEntity)
        {
            characterEntity.IdCreator = 0;
            await _characterRepository.UpdateItemAsync(characterEntity);
            _charactersCache.Remove(characterEntity);
        }

        public async Task ViewCharacter(CharacterEntity characterEntity)
        {
            var parameter = new NavigationParameters { { "character", characterEntity } };
            await NavigationService.NavigateAsync(Constants.CharacterPage, parameter);
        }

        private string _qrCodeValue = "0";
        public string QrCodeValue
        {
            get { return _qrCodeValue; }
            set { this.RaiseAndSetIfChanged(ref _qrCodeValue, value); }
        }

        private SourceCache<CharacterEntity, long> _charactersCache = new SourceCache<CharacterEntity, long>(c => c.Id);
        private ReadOnlyObservableCollection<CharacterEntity> _characters;
        public ReadOnlyObservableCollection<CharacterEntity> Characters => _characters;
    }
}
