using System;
using PageUpX.Core.Entities;
using PageUpX.DataAccess.DataAccessor;
using PageUpX.DataAccess.Repository;

namespace PocPuxThomas.Repositories.Interfaces
{
    public class RepositoryBase<TEntity, TEntityIdType> : PuxSimpleRepositoryBase<TEntity, TEntityIdType>, IPuxSimpleRepository<TEntity, TEntityIdType>
        where TEntity : class, IPuxEntity<TEntityIdType>, new()
    {
        public RepositoryBase(IPuxSimpleDataAccessor<TEntity, TEntityIdType> puxDataAccess) : base(puxDataAccess)
        {
        }
    }

    public class RepositoryBase<TEntity> : PuxSimpleRepositoryBase<TEntity>, IPuxSimpleRepository<TEntity>
        where TEntity : class, IPuxEntity, new()
    {
        public RepositoryBase(IPuxSimpleDataAccessor<TEntity> puxDataAccess) : base(puxDataAccess)
        {
        }
    }
}
