using ITPrint.API.Common;
using ITPrint.Core.Constants;
using ITPrint.Core.DTOs.Common;
using ITPrint.Core.DTOs.Users;
using ITPrint.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ITPrint.API.Endpoints;

public static class UsersEndpoint
{
    public static IEndpointRouteBuilder MapUsersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("api/users")
            .RequireAuthorization(options => 
                options.AddRequirements(requirements: new RolesAuthorizationRequirement(["Admin"])))
            .WithTags("Users")
            .WithOpenApi();

        group.MapGet("/", GetAllUsers)
            .Produces<ApiResponseDto<List<UserDto>>>();
        
        group.MapGet("active", GetActiveUsers)
            .Produces<ApiResponseDto<List<UserDto>>>();
        
        group.MapGet("{id:guid}", GetUserById)
            .Produces<ApiResponseDto<UserDto>>()
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest);
        
        group.MapPost("create", CreateUser)
            .Validate<CreateUserDto>()
            .Produces<ApiResponseDto<UserDto>>(StatusCodes.Status201Created)
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest);
        
        group.MapPut("{id:guid}", UpdateUser)
            .Validate<UpdateUserDto>()
            .Produces<ApiResponseDto<UserDto>>()
            .Produces(StatusCodes.Status404NotFound);
         
        group.MapDelete("{id:guid}", DeleteUser)
            .Produces<ApiResponseDto<bool>>()
            .Produces(StatusCodes.Status404NotFound);
        
        return group;
    }

    private static async Task<IResult> GetAllUsers(IUserService userService)
    {
        try
        {
            var users = await userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
                .ToList();
            
            return Results.Ok(ApiResponseDto<List<UserDto>>.SuccessResult(userDtos));
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении пользователей"
            });
        }
    }

    private static async Task<IResult> GetActiveUsers(IUserService userService)
    {
        try
        {
            var users = await userService.GetActiveUsersAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToList();

            return Results.Ok(ApiResponseDto<List<UserDto>>.SuccessResult(userDtos));
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении пользователей"
            });
        }
    }

    private static async Task<IResult> GetUserById(IUserService userService, Guid id)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return Results.BadRequest(new ErrorResponseDto
                {
                    Message = $"Пользователь с идентификатором {id} не найден"
                });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Results.Ok(ApiResponseDto<UserDto>.SuccessResult(userDto));
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении пользователя"
            });
        }
    }

    private static async Task<IResult> CreateUser(
        [FromBody]CreateUserDto request,
        IUserService userService)
    {
        try
        {
            var user = await userService.CreateUserAsync(
                request.Email,
                request.Username,
                request.Password,
                request.FirstName,
                request.LastName,
                request.Role
            );

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Results.Created(user.Id.ToString(),
                ApiResponseDto<UserDto>.SuccessResult(userDto, "Пользователь создан успешно")
            );
        }
        catch(InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при создании пользователя"
            });
        }
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        [FromBody]UpdateUserDto request,
        IUserService userService)
    {
        try
        {
            var user = await userService.UpdateUserAsync(
                id,
                request.Email,
                request.Username,
                request.FirstName,
                request.LastName,
                request.Role,
                request.IsActive
            );

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Results.Ok(ApiResponseDto<UserDto>.SuccessResult(userDto, "Информация о пользователе успешно обновлена"));
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new ErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при обновлении информации о пользователе"
            });
        }
    }

    private static async Task<IResult> DeleteUser(
        Guid id,
        IUserService userService)
    {
        try
        {
            await userService.DeleteUserAsync(id);
            return Results.Ok(ApiResponseDto<bool>.SuccessResult(true, "Пользователь удален успешно"));
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new ErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка в момент удаления пользователя"
            });
        }
    }
}