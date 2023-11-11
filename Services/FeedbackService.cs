using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Interfaces.Infra;
using Domain.Interfaces.Services;

namespace Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;

    public FeedbackService(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task<PaginatedResult<FeedbackResponse>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default)
    {
        var feedbacks = await _feedbackRepository.GetAll(pagination, cancellationToken);

        return feedbacks;
    }

    public async Task<string> Save(FeedbackRequest request, CancellationToken cancellationToken = default)
    {
        return await _feedbackRepository.Save(request, cancellationToken);
    }
}
