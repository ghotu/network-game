using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandServer
{

    public class AuthManager
    {
        public static bool Login(string userName, string password)
        {
            var user = MasterData.ValidUsers.Where(x => x.UserName.Equals(userName) && x.Password.Equals(password)).FirstOrDefault();
            if(user!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class GameState
    {
        public int SelectedWordLength { get; set; }
        public string ResolvedWord { get; set; }
        public int CurrentIndex { get; set; }

        public User NextUserTurn { get; set; }

        public bool IsGameFinished { get; set; }
        public Dictionary<string, Score> Scores { get; set; }
    }
    public class Game
    {
        public int NextUserIndex { get; set; }
        
        public string SelectedWord = string.Empty;
        public int CurrentIndex { get; set; }
        public Game()
        {
            this.Players = new List<User>();
            this.Scores = new Dictionary<string, Score>();

            Random r = new Random();
            int rInt = r.Next(0, MasterData.Words.Count);
            this.SelectedWord = MasterData.Words[rInt];
            this.CurrentIndex = 0;
            this.NextUserIndex = 0;
        }


        
        public GameState GetGameState()
        {
            GameState state = new GameState()
            {
                CurrentIndex = this.CurrentIndex,
                Scores = this.Scores,
                NextUserTurn = this.Players[NextUserIndex]
            };
            if (this.CurrentIndex > 0)
            {
                state.ResolvedWord = this.SelectedWord.Substring(0, this.CurrentIndex);
            }
            if(CurrentIndex>=this.SelectedWord.Length)
            {
                state.IsGameFinished = true;
            }
            state.SelectedWordLength = this.SelectedWord.Length;
            return state;
        }
        public void RegisterUser(User user)
        {
            var existingUser = this.Players.Where(x => x.UserName.Equals(user.UserName)).FirstOrDefault();
            if (existingUser == null)
            {
                this.Players.Add(user);
            }
            else
            {
                //send message this user is already registered
            }
        }

        public List<User> Players { get; set; }
        public Dictionary<string, Score> Scores { get; set; }

        public void RecieveLetter(string userName, string c)
        {
            if (CurrentIndex < this.SelectedWord.Length)
            {
                NextUserIndex++;
                if (NextUserIndex >= this.Players.Count)
                {
                    NextUserIndex = 0;
                }
                if (SelectedWord[CurrentIndex].ToString().ToLower() == c.ToString().ToLower())
                {
                    if (this.Scores.ContainsKey(userName))
                    {
                        this.Scores[userName].NumberOfAtempts++;
                        this.Scores[userName].Value = this.Scores[userName].Value + 10;
                    }
                    else
                    {
                        this.Scores.Add(userName, new Score() { UserName = userName, NumberOfAtempts = 1, Value = 10 });
                    }
                    CurrentIndex++;
                    //send message it is correct
                }
                else
                {
                    if (this.Scores.ContainsKey(userName))
                    {
                        this.Scores[userName].NumberOfAtempts++;
                        //this.Scores[userName].Value = this.Scores[userName].Value + 10;
                    }
                    else
                    {
                        this.Scores.Add(userName, new Score() { UserName = userName, NumberOfAtempts = 1, Value = 0 });
                    }

                    //send message it is wrong
                }

                if (CurrentIndex == this.SelectedWord.Length)
                {
                    //game is finished and send scores
                }
            }
        }
    }

    public class Score
    {
        public string UserName { get; set; }
        public int NumberOfAtempts { get; set; }
        public int Value { get; set; }
    }

    public static class MasterData
    {
        public static List<string> Words { get; set; }
        public static List<User> ValidUsers { get; set; }
        static MasterData()
        {
            ValidUsers = new List<User>();
            Words = new List<string>();
            ValidUsers.Add(new User() { UserName = "ashwani.kumar", Name = "Ashwani Kumar", Password = "india@987" });
            ValidUsers.Add(new User() { UserName = "anant.gupta", Name = "Anant Gupta", Password = "india@987" });
            ValidUsers.Add(new User() { UserName = "ishant.gupta", Name = "Ishant Gupta", Password = "india@987" });

            Words.Add("World");
            Words.Add("United");
            Words.Add("India");
            Words.Add("Digital");
            Words.Add("Cricket");
        }
    }
    public class User
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
