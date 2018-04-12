using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staaworks.BankExpert.FuzzyExpert.Language.Exceptions
{
    internal class InterpretationException: Exception
    {
        public InterpretationException(string message, int lineNumber) : base("InterpretationException: Cannot interpret statement " + lineNumber + "\n" + message) { }
        public InterpretationException(string message, int lineNumber, Exception innerException) : base("InterpretationException: Cannot interpret statement " + lineNumber + "\n" + message, innerException) { }
    }
}
