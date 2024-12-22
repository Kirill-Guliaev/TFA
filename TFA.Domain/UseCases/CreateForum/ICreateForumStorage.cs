using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

public interface ICreateForumStorage
{
    Task<Forum> Create(string Title, CancellationToken cancellationToken);
}