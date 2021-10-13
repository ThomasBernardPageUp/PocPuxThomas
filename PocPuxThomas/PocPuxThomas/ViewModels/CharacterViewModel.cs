using System;
using System.Threading.Tasks;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using PocPuxThomas.Wrappers;
using Prism.Navigation;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class CharacterViewModel: BaseViewModel
    {
        public Command SaveCommand { get; set; }


        private ICharacterRepository _characterRepository;

        public CharacterViewModel(INavigationService navigationService, ICharacterRepository characterRepository): base(navigationService)
        {
            _characterRepository = characterRepository;
            SaveCommand = new Command(SaveCharacter);
        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            if (parameters.ContainsKey("character"))
            {
                Character = parameters.GetValue<CharacterWrapper>("character");
            }

        }

        public async void SaveCharacter()
        {
            _character.IdCreator = App.ConnectedUser.Id;
            await _characterRepository.InsertOrReplaceItemAsync(new CharacterEntity(_character));

            // We add the edited character in parameter
            var parameter = new NavigationParameters { { "characterId", Character.Id } };
            // We go back with the character
            await NavigationService.GoBackAsync(parameter);
        }


        private CharacterWrapper _character;
        public CharacterWrapper Character
        {
            get { return _character; }
            set { SetProperty(ref _character, value); }
        }
    }
}
