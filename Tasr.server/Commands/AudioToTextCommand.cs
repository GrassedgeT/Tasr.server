using System.ComponentModel.DataAnnotations;

namespace Tasr.server.Commands
{
    public class AudioToTextCommand
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
