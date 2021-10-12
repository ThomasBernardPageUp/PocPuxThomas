using System;
using PocPuxThomas.Models.Entities.Interfaces;
using SQLite;

namespace PocPuxThomas.Models.Entities
{
    public class UserEntity : IUserEntity
    {
        public UserEntity()
        {
        }

        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Picture { get; set; }
    }
}
