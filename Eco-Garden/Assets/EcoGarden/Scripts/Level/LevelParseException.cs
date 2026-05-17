using System;

namespace EcoGarden.Level
{
    public sealed class LevelParseException : Exception
    {
        public LevelParseException(string message) : base(message)
        {
        }
    }
}
