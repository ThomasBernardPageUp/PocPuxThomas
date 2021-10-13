using System;
using System.Net.Http;
using System.Threading.Tasks;
using PocPuxThomas.Models.Business;

namespace PocPuxThomas.Helpers.Interfaces
{
    public interface IDataTransferHelper
    {
        Task<DataTransferHandlerResult<TResult>> SendClientAsync<TResult>
            (string route, HttpMethod httpMethod, object jsonContent = null) where TResult : class;
    }
}
