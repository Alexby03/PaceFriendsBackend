using PaceFriendsBackend.Core.Utils;
using PaceFriendsBackend.Data;

namespace PaceFriendsBackend.Core.services;

using Quartz;

public class EndOfWeekJob : IJob
{
    private readonly PaceFriendsRepository _repository;
    private readonly ILogger<EndOfDayJob> _logger;

    public EndOfWeekJob(PaceFriendsRepository repository, ILogger<EndOfDayJob> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Deciding winner of this week's leaderboard...");
        
        var serviceResult = await _repository.ProcessWeeklyResetAsync();
        var newWinner = await _repository.GetWeeklyWinnerAsync();
        
        if (serviceResult.Success && newWinner.Success) 
        {
            var wasReset = serviceResult.Data; 
            _logger.LogInformation($"Leaderboard reset success: {wasReset}");
            _logger.LogInformation($"Newest winner for this week is: {newWinner.Data}");
        }
        else
        {
            _logger.LogError($"Failed: {serviceResult.ErrorMessage}");
        }
    }
}
