using System;
using System.Runtime.Serialization;

namespace DistSysAcw.Models
{
    [Serializable]
    internal class HttpResponseException : Exception
    {
        private string v1;
        private int v2;

        public HttpResponseException()
        {
        }

        public HttpResponseException(string message, System.Net.HttpStatusCode badRequest) : base(message)
        {
        }

        public HttpResponseException(string v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public HttpResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}