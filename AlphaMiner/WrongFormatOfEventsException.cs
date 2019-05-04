using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    class WrongFormatOfEventsException : Exception
    {
        public WrongFormatOfEventsException()
        {
        }

        public WrongFormatOfEventsException(string message) : base(message)
        {
        }

        public WrongFormatOfEventsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongFormatOfEventsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
