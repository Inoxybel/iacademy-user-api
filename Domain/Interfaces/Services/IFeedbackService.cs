using CrossCutting.Helpers;
using Domain.DTO;

namespace Domain.Interfaces.Services;

public interface IFeedbackService
{
    Task<PaginatedResult<FeedbackResponse>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken= default);
    Task<string> Save(FeedbackRequest request, CancellationToken cancellationToken = default);
}
