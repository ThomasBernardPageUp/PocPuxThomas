using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Models.Entities.Interfaces;
using PocPuxThomas.Repositories.Interfaces;
using PocPuxThomas.Wrappers;
using Prism.Navigation;
using ReactiveUI;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class CharacterViewModel: BaseViewModel
    {
        public ReactiveCommand<Unit, Task> SaveCommand { get; }


        private ICharacterRepository _characterRepository;

        public CharacterViewModel(INavigationService navigationService, ICharacterRepository characterRepository): base(navigationService)
        {
            _characterRepository = characterRepository;

            var canExecuteCreate = this.WhenAnyValue(vm => vm.Character.Name, vm => vm.Character.Species, vm => vm.Character.Origin)
                .Select(query => 
                {

                        if (String.IsNullOrEmpty(query.Item1) || String.IsNullOrEmpty(query.Item2) || String.IsNullOrEmpty(query.Item3))
                            return false;
                        else
                            return true;
                    
                    return false;
                    
                });

            SaveCommand = ReactiveCommand.Create(SaveCharacter, canExecuteCreate);
        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            if (parameters.ContainsKey("character"))
                Character = new CharacterWrapper(parameters.GetValue<ICharacterEntity>("character"));
        }

        public async Task SaveCharacter()
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
            set { this.RaiseAndSetIfChanged(ref _character, value); }
        }
    }
}
