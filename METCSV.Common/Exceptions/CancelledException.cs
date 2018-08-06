using System;

namespace METCSV.Common.Exceptions
{
    public class CancelledException : Exception
    {
        public CancelledException() : base()
        {
        }

        public CancelledException(string message) : base(message)
        {
        }

        public CancelledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CancelledException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
