using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Exceptions
{
    public class GameException : Exception
    {
        public GameException(string message) : base(message) { }
    }
}
