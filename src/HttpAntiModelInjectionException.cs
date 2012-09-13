using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Runtime.Serialization;

namespace Byaxiom.MvcExtensions
{
    [Serializable]
    public sealed class HttpAntiModelInjectionException : HttpException
    {

        public HttpAntiModelInjectionException()
        {
        }

        private HttpAntiModelInjectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public HttpAntiModelInjectionException(string message)
            : base(message)
        {
        }

        public HttpAntiModelInjectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
