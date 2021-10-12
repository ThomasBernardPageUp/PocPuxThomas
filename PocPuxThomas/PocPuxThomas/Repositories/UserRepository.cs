using System;
using PageUpX.DataAccess.DataAccessor;
using PocPuxThomas.Models.Entities;
using PocPuxThomas.Repositories.Interfaces;

namespace PocPuxThomas.Repositories
{
    public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(IPuxSimpleDataAccessor<UserEntity> puxDataAccess) : base(puxDataAccess)
        {
        }
    }
}
