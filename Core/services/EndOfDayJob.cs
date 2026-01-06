using PaceFriendsBackend.Data;

namespace PaceFriendsBackend.Core.services;

using Quartz;

public class EndOfDayJob : IJob
{
    private readonly PaceFriendsRepository _repository;
    private readonly ILogger<EndOfDayJob> _logger;

    public EndOfDayJob(PaceFriendsRepository repository, ILogger<EndOfDayJob> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Resetting streaks...");
        
        var serviceResult = await _repository.ResetDailyStreaksAsync();
        
        if (serviceResult.Success) 
        {
            var wasReset = serviceResult.Data; 
            _logger.LogInformation($"Streaks reset success: {wasReset}");
        }
        else
        {
            _logger.LogError($"Failed: {serviceResult.ErrorMessage}");
        }
    }
}
