namespace Tasr.Server.Services;

public interface IMeetingSummaryService
{
	Task<string> SummaryAsync(string meetingminutes);
}