using System;
using System.Threading.Tasks;
using PocPuxThomas.Models.Entities;
using Prism.Navigation;

namespace PocPuxThomas.ViewModels
{
    public class CharacterViewModel: BaseViewModel
    {
        public CharacterViewModel(INavigationService navigationService): base(navigationService)
        {
        }

        protected override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            if (parameters.ContainsKey("character"))
            {
                Character = parameters.GetValue<CharacterEntity>("character");
            }

        }


        private CharacterEntity _character;
        public CharacterEntity Character
        {
            get { return _character; }
            set { SetProperty(ref _character, value); }
        }
    }
}
