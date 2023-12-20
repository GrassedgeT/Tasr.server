using Json.Schema.Generation;

namespace Tasr.Server.Commands;

public class TextContentCommand
{
	[Required]
	public string Content { get; set;}
}