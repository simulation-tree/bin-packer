using System;

namespace BinPacker
{
    public class ImpossibleFitException : Exception
    {
        public ImpossibleFitException(string message) : base(message)
        {
        }
    }
}