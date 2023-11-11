using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly DbContext _dbContext;

    public FeedbackRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResult<FeedbackResponse>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Feedback>.Filter.Empty;

        var totalRecords = await _dbContext.Feedback.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Feedback.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        var mappedData = data.Select(MapToResponse).ToList();

        return new PaginatedResult<FeedbackResponse>
        {
            Data = mappedData,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<string> Save(FeedbackRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var newFeedback = new Feedback()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Email = request.Email,
                Comment = request.Comment,
                CreatedDate = DateTime.UtcNow
            };

            await _dbContext.Feedback.InsertOneAsync(newFeedback, null, cancellationToken);

            return newFeedback.Id;
        }
        catch
        {
            return string.Empty;
        }
    }

    private FeedbackResponse MapToResponse(Feedback feedback)
    {
        return new FeedbackResponse
        {
            Name = feedback.Name,
            Email = feedback.Email,
            Comment = feedback.Comment,
            Data = feedback.CreatedDate
        };
    }
}
