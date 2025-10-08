using ITPrint.API.Endpoints;

namespace ITPrint.API.Common.Extensions;

public static class EndpointsExtension
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthEndpoints();
        endpoints.MapUsersEndpoint();
        endpoints.MapPrinterEndpoints();
        endpoints.MapFilesEndpoint();
        return endpoints;
    }   
}