using System;
using System.Runtime.Serialization;

namespace DistSysAcw.Models
{
    [Serializable]
    internal class HttpException : Exception
    {
        private string v1;
        private int v2;

        public HttpException()
        {
        }

        public HttpException(string message) : base(message)
        {
        }

        public HttpException(string v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public HttpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}