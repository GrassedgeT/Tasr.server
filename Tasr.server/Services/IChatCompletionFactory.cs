using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace Tasr.Server.Services;

public interface IChatCompletionFactory
{
	IChatCompletion GetChatCompletion();
}