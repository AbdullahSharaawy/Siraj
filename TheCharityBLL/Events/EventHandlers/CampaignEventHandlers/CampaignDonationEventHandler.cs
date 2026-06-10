using TheCharityBLL.Events.Abstraction;
using TheCharityBLL.Events.CampaignEvents;
using TheCharityBLL.Services.Abstraction;

namespace TheCharityBLL.Events.EventHandlers.CampaignEventHandlers
{
    public class CampaignDonationEventHandler : IEventHandler<CampaignDonationReceivedEvent>
    {
        private readonly IJobSchedulerService _jobScheduler;

        public CampaignDonationEventHandler(IJobSchedulerService jobScheduler)
        {
            _jobScheduler = jobScheduler;
        }

        public async Task HandleAsync(CampaignDonationReceivedEvent @event)
        {
            var campaign = @event.Campaign;

            // Calculate percentage
            var percentage = (campaign.Achieved / campaign.Target) * 100;

            // Check milestones (ALL logic here, NOT in service)
            if (percentage >= 25 && percentage < 50)
            {
                _jobScheduler.EnqueueJob<SendMilestoneEmailJob>(new { CampaignId = campaign.Id, Milestone = 25 });
            }
            else if (percentage >= 50 && percentage < 75)
            {
                _jobScheduler.EnqueueJob<SendMilestoneEmailJob>(new { CampaignId = campaign.Id, Milestone = 50 });
            }
            else if (percentage >= 75 && percentage < 100)
            {
                _jobScheduler.EnqueueJob<SendMilestoneEmailJob>(new { CampaignId = campaign.Id, Milestone = 75 });

                // Check if urgent (less than 7 days left)
                var daysLeft = (campaign.Deadline - DateTime.UtcNow).Days;
                if (daysLeft < 7 && daysLeft > 0)
                {
                    _jobScheduler.EnqueueJob<UrgentCampaignAlertJob>(new { CampaignId = campaign.Id });
                }
            }
            else if (percentage >= 100)
            {
                _jobScheduler.EnqueueJob<SendMilestoneEmailJob>(new { CampaignId = campaign.Id, Milestone = 100 });
            }

            await Task.CompletedTask;
        }
    }
}
