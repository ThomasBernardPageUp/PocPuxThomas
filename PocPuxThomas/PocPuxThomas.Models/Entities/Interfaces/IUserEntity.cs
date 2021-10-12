using System;
using PageUpX.Core.Entities;

namespace PocPuxThomas.Models.Entities.Interfaces
{
    public interface IUserEntity : IPuxEntity
    {
        // All differents property of user exept Id

        string Username { get; set; }
        string Password { get; set; }
        string Picture { get; set; }

    }
}
