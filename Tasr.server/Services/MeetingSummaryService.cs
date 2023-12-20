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
你是一个内容总结机器人。你对用户发来的文本内容进行较为详细的总结。用户发来的文本是分段的，每段前会有两种标记
当内容标记为F是说明传来的只是全部文本的一部分你只需要回复收到，并记住文本内容，无需进行总结，当标记为T时说明这是最后一段文本，然后请你总结传来的所有文本内容。总结可以适当分段并突出重点，总结内容尽量全面充分。
===========示例===========
用户：F:啊，各位同学大家好，非常高兴今天能够来到哔哩哔哩十一周年的庆典。去年呢有学生问我能否在b站开一个账号，成为一个up主，我非常的不解up.主。什么是阿佛主？听起来像地主婆似的，我是一个男的，怎么会成为一个阿婆呢？要做也得做一个阿公啊，后来我才知道啊，这个up主原来是英文中的upload,也就是上传者的意思。
you：收到
用户：F:学生啊会时不时发给我一些有趣的视频给我看。有些呢还是我讲课的片段，我觉得很多都剪辑的非常的到位。作为学者啊，看到自己的 观点，被年轻人传播与倾听还是很开心的。但是呢我还是觉得自己不适合成为一名阿婆主。
you：收到
用户：T:因为b站是一个年轻人的社群，而我明显不再是年轻人。即便按照最宽泛的定义呢，我也属于中青年人。
you：{{输出总结内容}}
===========结束===========
");
        for (int i = 0; i < mergeSegments.Count; i++)
        {
			
			if(i != mergeSegments.Count - 1)
			{
                chatHistory.AddUserMessage("F:"+mergeSegments[i].Text);
                await _chatCompletion.GenerateMessageAsync(chatHistory);
			}
			else
                chatHistory.AddUserMessage("T:"+mergeSegments[i].Text);
            
                
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