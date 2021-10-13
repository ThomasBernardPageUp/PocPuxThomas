using PocPuxThomas.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PocPuxThomas.Wrappers
{
    public class CharacterWrapper : ICharacterEntity, INotifyPropertyChanged
    {
        public long Id { get; set; }
        public long? IdCreator { get; set; }
        public string Image { get; set; }
        public string Species { get; set; }
        public string Gender { get; set; }
        public string Origin { get; set; }


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


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name
        {
            get { return this._name; }

            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
