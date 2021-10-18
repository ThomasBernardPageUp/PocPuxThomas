using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using PageUpX.Core.Log;
using PocPuxThomas.Commons;
using PocPuxThomas.Helpers.Interfaces;
using PocPuxThomas.Models.DTO.Down;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using PocPuxThomas.Wrappers;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using Xamarin.Forms;

namespace PocPuxThomas.ViewModels
{
    public class MenuViewModel:BaseViewModel
    {
        public ReactiveCommand<CharacterWrapper, Task> CharacterTappedCommand { get; }
        public ReactiveCommand<Unit, Task> ResetCharactersCommand { get; }
        public ReactiveCommand<Unit, Task> ProfileCommand { get; }
        public ReactiveCommand<CharacterWrapper, Task> DeleteCharacterCommand { get; set; }

        private IDataTransferHelper _dataTransferHelper;
        private ICharacterRepository _characterRepository;
        private IPuxLogger _puxLogger;


        public MenuViewModel(INavigationService navigationService, IDataTransferHelper dataTransferHelper, ICharacterRepository characterRepository, IPuxLogger puxLogger) : base(navigationService)
        {
            CharacterTappedCommand = ReactiveCommand.Create<CharacterWrapper, Task>(async characterWrapper => await ShowCharacter(characterWrapper));
            ResetCharactersCommand = ReactiveCommand.Create(ResetCharacters);
            ProfileCommand = ReactiveCommand.Create(ProfilePage);
            DeleteCharacterCommand = ReactiveCommand.Create<CharacterWrapper, Task>(async characterWrapper => await DeleteCharacter(characterWrapper));

            _puxLogger = puxLogger;
            _dataTransferHelper = dataTransferHelper;
            _characterRepository = characterRepository;

            _allSortsList.AddRange(new List<string>() { "Any", "Gender", "Name", "Origin" });

            IObservable<Func<CharacterEntity, bool>> filterSearch = 
                
                this.WhenAnyValue(vm => vm.SearchedCharacterName, vm => vm.SelectedGender) // When SearchedCharacterName's or SelectedGender's value change 
                .Throttle(TimeSpan.FromSeconds(1)) // And the value don't change from 1 sec
                .Select(query => // We execute (query is value of params)
                {
                    if (!String.IsNullOrEmpty(query.Item1) && !String.IsNullOrEmpty(query.Item2) && query.Item2 != "All")
                        return new Func<CharacterEntity, bool>(c => c.Name.Contains(query.Item1) && c.Gender.Equals(query.Item2));
                    else if (!String.IsNullOrEmpty(query.Item1) && String.IsNullOrEmpty(query.Item2))
                        return new Func<CharacterEntity, bool>(c => c.Name.Contains(query.Item1));
                    else if (String.IsNullOrEmpty(query.Item1) && !String.IsNullOrEmpty(query.Item2) && query.Item2 != "All")
                        return new Func<CharacterEntity, bool>(c => c.Gender.Equals(query.Item2));
                    else
                        return new Func<CharacterEntity, bool>(c => true);
                });

            IObservable<SortExpressionComparer<CharacterEntity>> sortSearch = this.WhenAnyValue(vm => vm.SelectedSort) // When the value of SelectedSort change 
                .Select(query => // We execute
                {
                    switch (query)
                    {
                        case "Gender":
                            return SortExpressionComparer<CharacterEntity>.Ascending(c => c.Gender);
                        case "Name":
                            return SortExpressionComparer<CharacterEntity>.Ascending(c => c.Name);
                        case "Origin":
                            return SortExpressionComparer<CharacterEntity>.Ascending(c => c.Origin);
                        case "Any":
                            return SortExpressionComparer<CharacterEntity>.Ascending(c => c.Id);
                    }
                    return SortExpressionComparer<CharacterEntity>.Ascending(c => c.Id);
                }); 





             _allCharacterEntities.Connect().Filter(filterSearch).Sort(sortSearch).Transform(x => new CharacterWrapper(x)).ObserveOn(RxApp.MainThreadScheduler).Bind(out _characters).DisposeMany().SubscribeSafe(_puxLogger);
            _allSortsList.Connect().Bind(out _allSorts).Subscribe(); // We link the source list and the private property
            _allGendersSource.Connect().Bind(out _allGenders).Subscribe();
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
                    int index = _allCharacterEntities.Items.IndexOf(_allCharacterEntities.Items.FirstOrDefault(c => c.Id == editedCharacterId));
                    _allCharacterEntities.RemoveAt(index);
                    _allCharacterEntities.Insert(index, editedCharacter);
                }
            }
            else // If we come from a NavigateTo
                await LoadCharacters(); // Load all characters
        }


        public async Task DeleteCharacter(CharacterWrapper characterWrapper)
        {
            if(await App.Current.MainPage.DisplayAlert("Warning", "Do you want delete this character ?", "Yes", "No"))
            {
                await _characterRepository.DeleteItemAsync(new CharacterEntity(characterWrapper)); // Delete it in the dataBase
                _allCharacterEntities.Remove(_allCharacterEntities.Items.FirstOrDefault(character => character.Id == characterWrapper.Id)); // Delete it in the list of all characters
            }
        }

        public async Task ResetCharacters()
        {
            if (await App.Current.MainPage.DisplayAlert("Warning", "Do you wan't to delete all characters ?", "Yes", "No"))
            {
                await _characterRepository.DropAsync(true);
                await LoadCharacters();
            }
        }


        public async Task LoadCharacters()
        {
            _allCharacterEntities.RemoveMany(_allCharacterEntities.Items);

            var charactersFromDatabase = await _characterRepository.GetItemsAsync();

            // If characters are already save in the database
            if (charactersFromDatabase?.Any() ?? false)
            {
                _allCharacterEntities.AddRange(charactersFromDatabase);
            }
            else
            {
                string baseUrl = "https://rickandmortyapi.com/api/character?page=";

                // 1 to 34 (number of pages)
                for (int i = 1; i < 35; i++)
                {
                    var result = await _dataTransferHelper.SendClientAsync<CharactersDownDTO>(baseUrl + i, HttpMethod.Get);

                    if (result.IsSuccess)
                        _allCharacterEntities.AddRange(Converters.CharacterDownToCharacterEntity.ConvertCharacterDownToCharacterEntity(result.Result.Results));
                    else
                        Console.WriteLine("call error");
                }

                await _characterRepository.InsertItemsAsync(_allCharacterEntities.Items);
            }

            // Get all differents genders
            _allGendersSource.RemoveMany(_allGendersSource.Items);
            _allGendersSource.AddRange(_allCharacterEntities.Items.Select(characterEntity => characterEntity.Gender).Distinct().ToList());
            _allGendersSource.Insert(0, "All");
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

        private string _searchedCharacterName = "";
        public string SearchedCharacterName
        {
            get => _searchedCharacterName;
            set { this.RaiseAndSetIfChanged(ref _searchedCharacterName, value); }
        }

        private string _selectedGender = "";
        public string SelectedGender
        {
            get => _selectedGender;
            set { this.RaiseAndSetIfChanged(ref _selectedGender, value); }
        }

        private string _selectedSort = "";
        public string SelectedSort
        {
            get { return _selectedSort; }
            set { this.RaiseAndSetIfChanged(ref _selectedSort, value);}
        }

        private SourceList<CharacterEntity> _allCharacterEntities = new SourceList<CharacterEntity>();
        private ReadOnlyObservableCollection<CharacterWrapper> _characters;
        public ReadOnlyObservableCollection<CharacterWrapper> Characters => _characters;

        private readonly SourceList<string> _allSortsList = new SourceList<string>(); // The source List
        private readonly ReadOnlyObservableCollection<string> _allSorts; // The private property
        public ReadOnlyObservableCollection<string> AllSorts => _allSorts; // The public property

        private SourceList<string> _allGendersSource = new SourceList<string>();
        private readonly ReadOnlyObservableCollection<string> _allGenders;
        public ReadOnlyObservableCollection<string> AllGenders => _allGenders;
    }
}
