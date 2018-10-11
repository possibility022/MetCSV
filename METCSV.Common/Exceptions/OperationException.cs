﻿using System;

namespace METCSV.Common.Exceptions
{
    public class OperationException : Exception
    {
        public OperationException() : base()
        {
        }

        public OperationException(string message) : base(message)
        {
        }

        public OperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
