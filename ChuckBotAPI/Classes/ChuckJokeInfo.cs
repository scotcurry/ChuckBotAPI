using System.Collections.Generic;


namespace ChuckBotAPI.Classes
{
   
    public class ChuckJokeInfo
    {
        public string type { get; set; }
        public Value value { get; set; }
    }

    public class Value
    {
        public int id { get; set; }
        public string joke { get; set; }
        public List<string> category { get; set; }
    }
}
