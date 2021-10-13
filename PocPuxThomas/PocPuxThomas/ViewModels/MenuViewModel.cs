using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PocPuxThomas.Commons;
using PocPuxThomas.Helpers.Interfaces;
using PocPuxThomas.Models.DTO.Down;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using PocPuxThomas.Wrappers;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class MenuViewModel:BaseViewModel
    {

        public string SearchedCharacterName { get; set; }
        public string SelectedGender { get; set; }
        public DelegateCommand<CharacterWrapper> CharacterTappedCommand { get; set; }
        public Command SearchCommand { get; set; }
        public Command ProfileCommand { get; set; }
        public Command ResetCharactersCommand { get; set; }
        public DelegateCommand<CharacterWrapper> DeleteCharacterCommand { get; set; }

        private IDataTransferHelper _dataTransferHelper;
        private List<CharacterEntity> _allCharacterEntities;
        private ICharacterRepository _characterRepository;


        public MenuViewModel(INavigationService navigationService, IDataTransferHelper dataTransferHelper, ICharacterRepository characterRepository) : base(navigationService)
        {
            _dataTransferHelper = dataTransferHelper;
            _characterRepository = characterRepository;
            SearchCommand = new Command(async () => await SearchCharacter());
            CharacterTappedCommand = new DelegateCommand<CharacterWrapper>(async (characterWrapper) => await ShowCharacter(characterWrapper));
            ProfileCommand = new Command(async () => await ProfilePage());
            ResetCharactersCommand = new Command(async () => await ResetCharacters());
            DeleteCharacterCommand = new DelegateCommand<CharacterWrapper>(async (characterWrapper) => await DeleteCharacter(characterWrapper));

            AllSorts = new List<string>() {"Any", "Gender", "Name", "Origin" };
        }


        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);


            // If we do a navigation.goback()
            if (parameters.GetNavigationMode().Equals(NavigationMode.Back))
            {
                // If we had a character in parameter
                if (parameters.ContainsKey("characterId"))
                {
                    int editedCharacterId = parameters.GetValue<int>("characterId");

                    CharacterEntity editedCharacter = await _characterRepository.GetItemAsync(editedCharacterId);
                    Characters.FirstOrDefault(character => character.Id == editedCharacterId).Name = editedCharacter.Name; // Replace the name

                    int index = _allCharacterEntities.IndexOf(_allCharacterEntities.FirstOrDefault(character => character.Id == editedCharacterId));
                    _allCharacterEntities.RemoveAt(index);
                    _allCharacterEntities.Insert(index, editedCharacter);

                }
            }
            else // If we come from a NavigateTo
            {
                await LoadCharacters(); // Load all characters
            }
        }


        public async Task DeleteCharacter(CharacterWrapper characterWrapper)
        {
            if(await App.Current.MainPage.DisplayAlert("Warning", "Do you want delete this character ?", "Yes", "No"))
            {
                await _characterRepository.DeleteItemAsync(new CharacterEntity(characterWrapper)); // Delete it in the dataBase
                Characters.Remove(characterWrapper); // Delete it in the current list of characters
                _allCharacterEntities.Remove(_allCharacterEntities.FirstOrDefault(character => character.Id == characterWrapper.Id)); // Delete it in the list of all characters
            }
        }

        public async Task ResetCharacters()
        {
            bool reply = await App.Current.MainPage.DisplayAlert("Warning", "Do you wan't to delete all characters ?", "Yes", "No");

            if (reply)
            {
                await _characterRepository.DropAsync(true);
                await LoadCharacters();
            }

        }


        public async Task LoadCharacters()
        {
            _allCharacterEntities = new List<CharacterEntity>();

            var charactersFromDatabase = await _characterRepository.GetItemsAsync();

            // If characters are already save in the database
            if (charactersFromDatabase?.Any() ?? false)
            {
                _allCharacterEntities = charactersFromDatabase.ToList();
            }
            else
            {
                string baseUrl = "https://rickandmortyapi.com/api/character?page=";

                // 1 to 34 (number of pages)
                for (int i = 1; i < 35; i++)
                {
                    var result = await _dataTransferHelper.SendClientAsync<CharactersDownDTO>(baseUrl + i, HttpMethod.Get);

                    if (result.IsSuccess)
                    {
                        _allCharacterEntities.AddRange(Converters.CharacterDownToCharacterEntity.ConvertCharacterDownToCharacterEntity(result.Result.Results));
                    }
                    else
                    {
                        Console.WriteLine("call error");
                    }
                }

                await _characterRepository.InsertOrReplaceItemsAsync(_allCharacterEntities);
            }

            // Get all differents genders
            AllGenders = new ObservableCollection<string>(_allCharacterEntities.Select(characterEntity => characterEntity.Gender).Distinct().ToList());
            AllGenders.Insert(0, "All");
            Characters = new ObservableCollection<CharacterWrapper>(_allCharacterEntities.Select(characterEntity => new CharacterWrapper(characterEntity)));
        }

        public async Task SearchCharacter()
        {
            // 1) Reset the list
            Characters = new ObservableCollection<CharacterWrapper>(_allCharacterEntities.Select(characterEntity => new CharacterWrapper(characterEntity)));


            // 2) We filter with the name
            if (!string.IsNullOrEmpty(SearchedCharacterName))
            {
                Characters = new ObservableCollection<CharacterWrapper>(Characters.Where(character => character.Name.Contains(SearchedCharacterName)).ToList());
            }


            // We filter with the gender
            if (!string.IsNullOrEmpty(SelectedGender) && SelectedGender != "All")
            {
                Characters = new ObservableCollection<CharacterWrapper>(Characters.Where(character => character.Gender.Contains(SelectedGender)).ToList());
            }
        }


        public async Task ShowCharacter(CharacterWrapper characterWrapper)
        {
            var parameter = new NavigationParameters { { "character", characterWrapper } };
            await NavigationService.NavigateAsync(Constants.CharacterPage, parameter);
        }

        public async Task ProfilePage()
        {
            await NavigationService.NavigateAsync(Constants.ProfilePage);
        }

        public async Task ChangeSort()
        {
            List<CharacterWrapper> characterSorted = new List<CharacterWrapper>();

            switch (SelectedSort)
            {
                case "Any":
                    characterSorted = Characters.OrderBy(character => character.Id).ToList();
                    break;
                case "Gender":
                    characterSorted = Characters.OrderBy(character => character.Gender).ToList();
                    break;
                case "Name":
                    characterSorted = Characters.OrderBy(character => character.Name).ToList();
                    break;
                case "Origin":
                    characterSorted = Characters.OrderBy(character => character.Origin).ToList();
                    break;
            }

            Characters = new ObservableCollection<CharacterWrapper>(characterSorted);
        }

        private string _selectedSort;
        public string SelectedSort
        {
            get { return _selectedSort; }
            set { SetProperty(ref _selectedSort, value); ChangeSort(); }
        }

        private List<string> _allSorts;
        public List<string> AllSorts
        {
            get { return _allSorts; }
            set { SetProperty(ref _allSorts, value); }
        }

        private ObservableCollection<CharacterWrapper> _characters;
        public ObservableCollection<CharacterWrapper> Characters
        {   
            get { return _characters; }
            set { SetProperty(ref _characters, value); }
        }

        private ObservableCollection<string> _allGenders;
        public ObservableCollection<string> AllGenders
        {
            get { return _allGenders; }
            set { SetProperty(ref _allGenders, value); }
        }


    }
}
