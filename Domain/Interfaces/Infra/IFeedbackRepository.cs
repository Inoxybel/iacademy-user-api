using CrossCutting.Helpers;
using Domain.DTO;

namespace Domain.Interfaces.Infra;

public interface IFeedbackRepository
{
    Task<PaginatedResult<FeedbackResponse>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default);
    Task<string> Save(FeedbackRequest request, CancellationToken cancellationToken = default);
}
