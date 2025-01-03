using Microsoft.AspNetCore.Mvc;
using TFA.API.Authentication;
using TFA.API.Models;
using TFA.Domain.UseCases.SignIn;
using TFA.Domain.UseCases.SignOn;

namespace TFA.API.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SignOnAsync(
        [FromBody] SignOn request,
        [FromServices] ISignOnUseCase useCase,
        CancellationToken cancellationToken)
    {
        var identity = await useCase.ExecuteAsync(new SignOnCommand(request.Login, request.Password), cancellationToken);
        return Ok(identity);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync(
        [FromBody] SignIn request,
        [FromServices] ISignInUseCase useCase,
        [FromServices] IAuthTokenStorage tokenStorage,
        CancellationToken cancellationToken)
    {
        var (identity, token) = await useCase.ExecuteAsync(new SignInCommand(request.Login, request.Password), cancellationToken);
        tokenStorage.Store(HttpContext, token);
        return Ok(identity);

    }
}
