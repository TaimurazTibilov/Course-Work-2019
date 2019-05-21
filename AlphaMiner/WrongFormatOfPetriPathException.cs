using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    public class WrongFormatOfPetriPathException : Exception
    {
        public WrongFormatOfPetriPathException()
        {
        }

        public WrongFormatOfPetriPathException(string message) : base(message)
        {
        }

        public WrongFormatOfPetriPathException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongFormatOfPetriPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
