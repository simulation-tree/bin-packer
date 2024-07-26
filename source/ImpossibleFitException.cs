using System;

namespace BinPacking
{
    public class ImpossibleFitException : Exception
    {
        public ImpossibleFitException(string message) : base(message)
        {
        }
    }
}