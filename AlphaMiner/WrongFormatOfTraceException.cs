using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    public class WrongFormatOfTraceException : Exception
    {
        public WrongFormatOfTraceException()
        {
        }

        public WrongFormatOfTraceException(string message) : base(message)
        {
        }

        public WrongFormatOfTraceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongFormatOfTraceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
