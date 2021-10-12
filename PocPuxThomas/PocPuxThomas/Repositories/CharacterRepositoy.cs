using PageUpX.DataAccess.DataAccessor;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PocPuxThomas.Repositories
{
    public class CharacterRepository : RepositoryBase<CharacterEntity>, ICharacterRepository
    {
        public CharacterRepository(IPuxSimpleDataAccessor<CharacterEntity> puxDataAccess) : base(puxDataAccess)
        {
        }
    }
}
