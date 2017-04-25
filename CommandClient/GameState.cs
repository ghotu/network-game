using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandClient
{
    public class GameState
    {
        public string ResolvedWord { get; set; }
        public int currentIndex { get; set; }
        public int SelectedWordLength { get; set; }
        public User NextUserTurn { get; set; }
        public bool IsGameFinished { get; set; }
        public Dictionary<string, Score> Scores { get; set; }
    }


    public class Score
    {
        public string UserName { get; set; }
        public int NumberOfAtempts { get; set; }
        public int Value { get; set; }
    }

    public class User
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
