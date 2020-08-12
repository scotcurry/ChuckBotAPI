using System;
using System.Collections.Generic;

namespace ChuckBotAPI.Classes
{
    public class BotUpdateRequest
    {
        public bool ok { get; set; }
        public List<Result> result { get; set; }
    }

    public class Result
    {
        public int update_id { get; set; }
        public Message message { get; set; }
        public InLineQuery inline_query { get; set; }
    }

    public class InLineQuery
    {
        public string id { get; set; }
        public From from { get; set; }
        public string query { get; set; }
        public string offset { get; set; }
    }

    public class Message
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
        public List<Entity> entities { get; set; }
    }

    public class From
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string type { get; set; }
    }

    public class Entity
    {
        public string type { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
    }
}
