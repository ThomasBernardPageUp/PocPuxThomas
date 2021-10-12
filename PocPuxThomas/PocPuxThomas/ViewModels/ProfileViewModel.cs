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

namespace PocPuxThomas.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public DelegateCommand<CharacterEntity> DeleteCommand { get; set; }
        public DelegateCommand<CharacterEntity> ViewCommand { get; set; }

        private ICharacterRepository _characterRepository;

        ProfileViewModel(INavigationService navigationService, ICharacterRepository characterRepository):base(navigationService)
        {
            _characterRepository = characterRepository;
            DeleteCommand = new DelegateCommand<CharacterEntity>(DeleteCharacter);
            ViewCommand = new DelegateCommand<CharacterEntity>(ViewCharacter);
        }


        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            UserEntity = App.ConnectedUser;
            Characters = new ObservableCollection<CharacterEntity>(await _characterRepository.GetItemsAsync( (characterEntity) => characterEntity.IdCreator == App.ConnectedUser.Id ));
        }

        public async void DeleteCharacter(CharacterEntity characterEntity)
        {
            characterEntity.IdCreator = 0;
            await _characterRepository.UpdateItemAsync(characterEntity);
            Characters.Remove(characterEntity);
        }

        public async void ViewCharacter(CharacterEntity characterEntity)
        {
            var parameter = new NavigationParameters { { "character", characterEntity } };
            await NavigationService.NavigateAsync(Constants.CharacterPage, parameter);
        }

        private UserEntity _userEntity;
        public UserEntity UserEntity
        {
            get { return _userEntity; }
            set { SetProperty(ref _userEntity, value); }
        }


        private ObservableCollection<CharacterEntity> _characters;
        public ObservableCollection<CharacterEntity> Characters
        {
            get { return _characters; }
            set { SetProperty(ref _characters, value); }
        }
    }
}
