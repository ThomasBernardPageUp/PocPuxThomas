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

        private IDataTransferHelper _dataTransferHelper;
        private List<CharacterEntity> _allCharacterEntities;


        public MenuViewModel(INavigationService navigationService, IDataTransferHelper dataTransferHelper) : base(navigationService)
        {
            _dataTransferHelper = dataTransferHelper;
            SearchCommand = new Command(SearchCharacter);
            CharacterTappedCommand = new DelegateCommand<CharacterEntity>(ShowCharacter);
            ProfileCommand = new Command(ProfilePage);
        }


        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            LoadCharacters(); // Load all characters
        }


        public async void LoadCharacters()
        {
            _allCharacterEntities = new List<CharacterEntity>();
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

            // Get all differents genders
            AllGenders = new ObservableCollection<string>(_allCharacterEntities.Select(characterEntity => characterEntity.Gender).Distinct().ToList());
            Characters = _allCharacterEntities;
        }

        public async void SearchCharacter()
        {
            // 1) Reset the list
            Characters = _allCharacterEntities;


            // 2) We filter with the name
            if (!string.IsNullOrEmpty(SearchedCharacterName))
            {
                Characters = Characters.Where(character => character.Name.Contains(SearchedCharacterName)).ToList();
            }


            // We filter with the gender
            if (!string.IsNullOrEmpty(SelectedGender) && SelectedGender != "All")
            {
                Characters = Characters.Where(character => character.Gender.Contains(SelectedGender)).ToList();
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

        private List<CharacterEntity> _characters;
        public List<CharacterEntity> Characters
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
