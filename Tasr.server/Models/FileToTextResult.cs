using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tasr.Models
{
    public class FileToTextResult
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("sentences")]
        public IEnumerable<Sentence> Sentences { get; set; }
    }
    public class Sentence
    {
        [JsonPropertyName("text")]
        string Text { get; set; }

        [JsonPropertyName("start")]
        int Start { get; set; }

        [JsonPropertyName("end")]
        int End { get; set; }
    }

} 
