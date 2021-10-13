using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PocPuxThomas.Commons;
using PocPuxThomas.Helpers.Interfaces;
using PocPuxThomas.Models.DTO.Down;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class MenuViewModel:BaseViewModel
    {

        public string SearchedCharacterName { get; set; }
        public string SelectedGender { get; set; }
        public DelegateCommand<CharacterEntity> CharacterTappedCommand { get; set; }
        public Command SearchCommand { get; set; }
        public Command ProfileCommand { get; set; }
        public Command ResetCharactersCommand { get; set; }
        public DelegateCommand<CharacterEntity> DeleteCharacterCommand { get; set; }

        private IDataTransferHelper _dataTransferHelper;
        private List<CharacterEntity> _allCharacterEntities;
        private ICharacterRepository _characterRepository;


        public MenuViewModel(INavigationService navigationService, IDataTransferHelper dataTransferHelper, ICharacterRepository characterRepository) : base(navigationService)
        {
            _dataTransferHelper = dataTransferHelper;
            _characterRepository = characterRepository;
            SearchCommand = new Command(SearchCharacter);
            CharacterTappedCommand = new DelegateCommand<CharacterEntity>(ShowCharacter);
            ProfileCommand = new Command(ProfilePage);
            ResetCharactersCommand = new Command(ResetCharacters);
            DeleteCharacterCommand = new DelegateCommand<CharacterEntity>(DeleteCharacter);

            AllSorts = new List<string>() {"Any", "Gender", "Name", "Origin" };
        }


        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            LoadCharacters(); // Load all characters
        }


        public async void DeleteCharacter(CharacterEntity characterEntity)
        {
            if(await App.Current.MainPage.DisplayAlert("Warning", "Do you want delete this character ?", "Yes", "No"))
            {
                await _characterRepository.DeleteItemAsync(characterEntity); // Delete it in the dataBase
                Characters.Remove(characterEntity); // Delete it in the current list of characters
                _allCharacterEntities.Remove(characterEntity); // Delete it in the list of all characters
            }

        }

        public async void ResetCharacters()
        {
            bool reply = await App.Current.MainPage.DisplayAlert("Warning", "Do you wan't to delete all characters ?", "Yes", "No");

            if (reply)
            {
                await _characterRepository.DropAsync(true);
                LoadCharacters();
            }

        }


        public async void LoadCharacters()
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
            Characters = new ObservableCollection<CharacterEntity>(_allCharacterEntities);
        }

        public async void SearchCharacter()
        {
            // 1) Reset the list
            Characters = new ObservableCollection<CharacterEntity>(_allCharacterEntities);


            // 2) We filter with the name
            if (!string.IsNullOrEmpty(SearchedCharacterName))
            {
                Characters = new ObservableCollection<CharacterEntity>(Characters.Where(character => character.Name.Contains(SearchedCharacterName)).ToList());
            }


            // We filter with the gender
            if (!string.IsNullOrEmpty(SelectedGender) && SelectedGender != "All")
            {
                Characters = new ObservableCollection<CharacterEntity>(Characters.Where(character => character.Gender.Contains(SelectedGender)).ToList());
            }
        }


        public async void ShowCharacter(CharacterEntity characterEntity)
        {
            var parameter = new NavigationParameters { { "character", characterEntity } };
            await NavigationService.NavigateAsync(Constants.CharacterPage, parameter);
        }

        public async void ProfilePage()
        {
            await NavigationService.NavigateAsync(Constants.ProfilePage);
        }

        public async void ChangeSort()
        {
            List<CharacterEntity> characterSorted = new List<CharacterEntity>();

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

            Characters = new ObservableCollection<CharacterEntity>(characterSorted);
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

        private ObservableCollection<CharacterEntity> _characters;
        public ObservableCollection<CharacterEntity> Characters
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
