
using System.Text.Json;
using System;

namespace Cw7.Models
{
    public class LoggingData
    {
        public DateTime Date { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
        public string QueryString { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }



    }
}
