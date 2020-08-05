using System;

namespace DocuStore.Api.Models
{
    public class ServiceException
    {
        public ServiceException(Exception exception)
        {
            Message = exception.Message;
            StackTrace = exception.StackTrace;

            if (exception.InnerException != null)
                InnerException = new ServiceException(exception.InnerException);
        }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        public ServiceException InnerException { get; set; }
    }
}