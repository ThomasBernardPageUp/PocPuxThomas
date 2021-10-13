using System;
using PageUpX.Core.Entities;

namespace PocPuxThomas.Models.Entities.Interfaces
{
    public interface ICharacterEntity : IPuxEntity
    {
        // All differents property of user exept Id

        long? IdCreator { get; set; }
        string Name { get; set; }
        string Image { get; set; }
        string Species { get; set; }
        string Gender { get; set; }
        string Origin { get; set; }
    }
}
