using Microsoft.AspNetCore.Mvc;
using TFA.API.Models;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;

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
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(201, Type = typeof(Topic))]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        [FromServices] ICreateTopicUseCase useCase,
        CancellationToken cancellationToken)
    {
            var topic = await useCase.ExecuteAsync(new(forumId, request.Title), cancellationToken);
            return CreatedAtRoute(nameof(GetForums), new Topic()
            {
                Id = topic.Id,
                CreatedAd = topic.CreatedAt,
                Title = topic.Title
            });
    }

    [HttpGet("{forumId:guid}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetTopics(
        [FromRoute] Guid forumId,
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IGetTopicsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new(forumId, skip, take), cancellationToken);
        return Ok(new { result.resources, result.totalCount });
    }
}
