using Microsoft.SemanticKernel.AI.ChatCompletion;
using System.Text.Json;
using Tasr.Models;

namespace Tasr.Server.Services;

public class MeetingSummaryService : IMeetingSummaryService
{
	private IChatCompletion _chatCompletion;

	public MeetingSummaryService(IChatCompletionFactory chatCompletionFactory)
	{
		_chatCompletion = chatCompletionFactory.GetChatCompletion();
	}
	public async Task<string> SummaryAsync(string jsonContent)
	{
		var segments = JsonSerializer.Deserialize<List<Sentence>>(jsonContent);
		List<Sentence> mergeSegments = MergeSegments(segments);
		var chatHistory = _chatCompletion.CreateNewChat();
		chatHistory.AddSystemMessage(@"
你是一个内容总结机器人。你对用户发来的文本内容进行较为详细的总结。用户发来的文本是分段的并且含有开始时间和结束时间的毫秒时间戳，每段前会有两种标记,
当内容标记为F是说明传来的只是全部文本的一部分你只需要回复收到，并记住文本内容，无需进行总结，当标记为T时说明这是最后一段文本，然后请你总结传来的所有文本内容，并总结内容进行适当的解释说明。
===========示例===========
用户：F:文本段1
you：收到
用户：F:文本段2
you：收到
用户：T:文本段3
you：{{总结内容和解释说明}}
===========结束===========
");
        for (int i = 0; i < mergeSegments.Count; i++)
        {
			
			if(i != mergeSegments.Count - 1)
			{
				chatHistory.AddUserMessage("F:" + mergeSegments[i].Text + "startTime:" + mergeSegments[i].Start.ToString()+" - endTime:" + mergeSegments[i].End.ToString());
                await _chatCompletion.GenerateMessageAsync(chatHistory);
			}
			else
                chatHistory.AddUserMessage("T:"+mergeSegments[i].Text + "startTime:" + mergeSegments[i].Start.ToString()+ " - endTime:" + mergeSegments[i].End.ToString());
            
                
        }
		var reply = await _chatCompletion.GenerateMessageAsync(chatHistory);
		return reply;
	}

	public List<Sentence> MergeSegments(List<Sentence> segments)
	{
		int MAX_LENGTH = 1500;
        List<Sentence> mergeSegments = new();
        Sentence mergeSegment = new();
		for (int i = 0; i < segments.Count; i++)
		{
            if (mergeSegment.Start == 0)
            {
                mergeSegment.Start = segments[i].Start;
            }
            mergeSegment.Text += segments[i].Text;
			if(i+1 != segments.Count)
			{
				if (mergeSegment.Text.Length + segments[i+1].Text.Length > MAX_LENGTH)
				{
					mergeSegment.End = segments[i].End;
					mergeSegments.Add(mergeSegment);
					mergeSegment = new();
				}
			}
        }

        return mergeSegments;
    }   
}