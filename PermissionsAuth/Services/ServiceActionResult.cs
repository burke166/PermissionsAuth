using System.Collections.Generic;
using System.Net;

namespace PermissionsAuth.Services
{
    public class ServiceActionResult<T>
    {
        public bool Success
        {
            get
            {
                return Errors.Count < 1;
            }
        }
        public List<string> Errors { get; set; }
        public T Value { get; set; }
        public int PageCount { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        public ServiceActionResult()
        {
            Errors = new List<string>();
        }

        public ServiceActionResult(List<string> errors, T result)
        {
            Errors = errors;
            Value = result;
        }

        public ServiceActionResult(T result)
        {
            Errors = new List<string>();
            Value = result;
        }

    }
}
