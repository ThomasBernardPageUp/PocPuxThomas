using System;
using System.Net;

namespace PocPuxThomas.Models.Business
{
    public class DataTransferHandlerResult<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public T Result { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }

        public bool IsSuccess { get; set; }

    }
}
