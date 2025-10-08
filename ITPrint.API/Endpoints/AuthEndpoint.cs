using System.Security.Claims;
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
        
        group.MapPost("register", Register)
            .Validate<RegisterRequestDto>()
            .Produces<ApiResponseDto<LoginResponseDto>>(201)
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest);
        
        group.MapGet("me", GetCurrentUser)
            .RequireAuthorization()
            .Produces<ApiResponseDto<LoginResponseDto>>()
            .Produces(401);
        
        group.MapPost("change-password", ChangePassword)
            .Validate<ChangePasswordRequestDto>()
            .Produces<ApiResponseDto<bool>>()
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

    private static async Task<IResult> Register(
        IAuthService authService,
        [FromBody]RegisterRequestDto request,
        IConfiguration configuration)
    {
        try
        {
            var user = await authService.RegisterAsync(
                request.Email,
                request.Username,
                request.Password,
                request.FirstName,
                request.LastName
            );
        
            var loginResult = await authService.LoginAsync(request.Email, request.Password);

            if (!loginResult.HasValue)
            {
                return Results.InternalServerError(new ErrorResponseDto
                {
                    Message = "Регистрация успешно, но автоматическая авторизация не сработала"
                });
            }
        
            var (_, token) = loginResult.Value;
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

            return Results.Created(
                user.Id.ToString(),
                ApiResponseDto<LoginResponseDto>.SuccessResult(response, "Успешная регистрация"));
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка",
                Errors = [ex.Message]
            });
        }
    }

    private static async Task<IResult> GetCurrentUser(
        HttpContext context,
        IUserService userService)
    {
        try
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound(new ErrorResponseDto
                {
                    Message = "Пользователь не найден"
                });
            }
            
            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = string.Empty,
                ExpiresAt = DateTime.MinValue
            };

            return Results.Ok(ApiResponseDto<LoginResponseDto>.SuccessResult(response));
            
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "An error occurred",
                Errors = [e.Message]
            });
        }
    }

    private static async Task<IResult> ChangePassword(
        [FromBody]ChangePasswordRequestDto request,
        HttpContext context,
        IUserService userService)
    {
        try
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }
            
            var success = await userService.ChangePasswordAsync(
                userId,
                request.CurrentPassword,
                request.NewPassword
            );

            if (!success)
            {
                return Results.BadRequest(new ErrorResponseDto
                {
                    Message = "Не удалось сменить пароль. Проверьте текущий пароль."
                });
            }
            
            return Results.Ok(ApiResponseDto<bool>.SuccessResult(true, "Пароль успешно изменен"));
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "An error occurred while changing password",
                Errors = [ex.Message]
            });
        }
    }
}