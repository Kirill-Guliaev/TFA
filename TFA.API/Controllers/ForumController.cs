using Microsoft.AspNetCore.Mvc;
using TFA.API.Models;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;

namespace TFA.API.Controllers;

[ApiController]
[Route("forums")]
public class ForumController : ControllerBase
{
    /// <summary>
    /// Get lost on every forums
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Name = nameof(GetForums))]
    [ProducesResponseType(200, Type = typeof(ForumResponse[]))]
    public async Task<IActionResult> GetForums(
        [FromServices] IGetForumsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var forums = await useCase.ExecuteAsync(cancellationToken);
        return Ok(forums.Select(f => new ForumResponse() { Id = f.Id, Title = f.Title }));
    }

    [HttpPost("{forumId:guid}/topics")]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(201, Type = typeof(Topic))]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        [FromServices] ICreateTopicUseCase useCase,
        CancellationToken cancellationToken)
    {
        try
        {
            var topic = await useCase.ExecuteAsync(forumId, request.Title, cancellationToken);
            return CreatedAtRoute(nameof(GetForums), new Topic()
            {
                Id = topic.Id,
                CreatedAd = topic.CreatedAd,
                Title = topic.Title
            });
        }
        catch(Exception ex)
        {
            return ex switch
            {
                IntentionManagerException => Forbid(),
                ForumNotFoundException => StatusCode(StatusCodes.Status410Gone),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
