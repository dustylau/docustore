using System;
using System.Net;

namespace DocuStore.Api.Models
{
    public class ServiceResponse
    {
        public ServiceResponse() : this(HttpStatusCode.OK)
        {
        }

        public ServiceResponse(HttpStatusCode status)
        {
            Status = status;
        }

        public ServiceResponse(Exception exception) : this(HttpStatusCode.InternalServerError)
        {
            Exception = new ServiceException(exception);
        }

        public virtual HttpStatusCode Status { get; set; }
        public virtual bool Error => Exception != null;
        public virtual ServiceException Exception { get; set; }
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public ServiceResponse() : base()
        {
        }

        public ServiceResponse(T data) : this(data, HttpStatusCode.OK)
        {
        }

        public ServiceResponse(T data, HttpStatusCode status) : base(status)
        {
            Data = data;
        }

        public ServiceResponse(Exception exception) : base(exception)
        {
        }

        public T Data { get; set; }
    }
}