using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

public interface ICreateForumStorage
{
    Task<Forum> CreateAsync(string Title, CancellationToken cancellationToken);
}