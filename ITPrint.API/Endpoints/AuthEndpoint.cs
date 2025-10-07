using ITPrint.Core.DTOs.Auth;

namespace ITPrint.API.Endpoints;

public static class AuthEndpoint
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group =  endpoints
            .MapGroup("api/auth")
            .WithTags("Authentication")
            .WithOpenApi();
        
        return group;
    }

    private static async Task<IResult> LoginAsync(LoginRequestDto request )
    {
        throw new NotImplementedException();
    }
}