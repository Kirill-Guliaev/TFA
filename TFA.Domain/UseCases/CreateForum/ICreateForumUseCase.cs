using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

public interface ICreateForumUseCase
{
    Task<Forum> ExecuteAsync(CreateForumCommand command, CancellationToken cancellationToken);
}