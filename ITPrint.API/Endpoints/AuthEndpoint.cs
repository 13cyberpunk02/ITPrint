using ITPrint.API.Common;
using ITPrint.Core.DTOs.Auth;
using ITPrint.Core.DTOs.Common;
using ITPrint.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITPrint.API.Endpoints;

public static class AuthEndpoint
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group =  endpoints
            .MapGroup("api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("login", LoginAsync)
            .Validate<LoginRequestDto>()
            .Produces<ApiResponseDto<LoginResponseDto>>()
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        return group;
    }

    private static async Task<IResult> LoginAsync(
        IAuthService authService,
        [FromBody]LoginRequestDto request,
        IConfiguration configuration)
    {
        try
        {
            var result = await authService.LoginAsync(request.Email, request.Password);
            if (!result.HasValue)
                return Results.Unauthorized();
        
            var (user, token) = result.Value;
            var expiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "60");
        
            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
            return Results.Ok(ApiResponseDto<LoginResponseDto>.SuccessResult(response, "Успешная авторизация"));
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Возникла ошибка при авторизации",
                Errors = [e.Message]
            });
        }
    }
}