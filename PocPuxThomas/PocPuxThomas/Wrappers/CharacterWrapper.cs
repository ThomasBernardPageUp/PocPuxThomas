using PocPuxThomas.Models.Entities.Interfaces;
using ReactiveUI;

namespace PocPuxThomas.Wrappers
{
    public class CharacterWrapper : ReactiveObject, ICharacterEntity
    {
        public long Id { get; set; }
        public long? IdCreator { get; set; }
        public string Image { get; set; }
        public string Gender { get; set; }


        public CharacterWrapper(ICharacterEntity characterEntity)
        {
            Id = characterEntity.Id;
            IdCreator = characterEntity.IdCreator;
            Name = characterEntity.Name;
            Image = characterEntity.Image;
            Species = characterEntity.Species;
            Gender = characterEntity.Gender;
            Origin = characterEntity.Origin;
        }

        private string _name;
        public string Name
        {
            get { return _name; }

            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _species;
        public string Species
        {
            get { return _species; }
            set { this.RaiseAndSetIfChanged(ref _species, value); }
        }

        private string _origin;
        public string Origin
        {
            get { return _origin; }
            set { this.RaiseAndSetIfChanged(ref _origin, value); }
        }
    }
}
