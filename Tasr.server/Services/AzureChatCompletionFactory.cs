using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;

namespace Tasr.Server.Services;

public class AzureChatCompletionFactory :IChatCompletionFactory
{
	private IConfiguration _configuration;

	public AzureChatCompletionFactory(IConfiguration configuration)
	{
		_configuration = configuration;
	}


	public IChatCompletion GetChatCompletion()
	{
		var modelId = _configuration["AzureChatCompletionFactory.ModelId"];
		string endpoint = _configuration["AzureChatCompletionFactory.Endpoint"];
		var apiKey = _configuration["AzureChatCompletionFactory.ApiKey"];

		var azureOpenAiChatCompletion = new AzureOpenAIChatCompletion(modelId,
			endpoint,
			apiKey);
		return azureOpenAiChatCompletion;
	}
}