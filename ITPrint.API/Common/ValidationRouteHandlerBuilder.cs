using ITPrint.API.Common.Extensions;

namespace ITPrint.API.Common;

public static class ValidationRouteHandlerBuilder
{
    public static RouteHandlerBuilder Validate<T>(this RouteHandlerBuilder builder, bool firstErrorOnly = true)
    { 
        builder.AddEndpointFilter(async (invocationContext, next) =>
        {
            var argument = invocationContext.Arguments.OfType<T>().FirstOrDefault();
            if (argument == null) return await next(invocationContext);
            var response = argument.DataAnnotationsValidate();

            if (response.IsValid) return await next(invocationContext);
            var errorMessage =   firstErrorOnly ? 
                response.Results.FirstOrDefault()?.ErrorMessage : 
                string.Join("|", response.Results.Select(x => x.ErrorMessage));

            return Results.Problem(errorMessage, statusCode: 400);

        });

        return builder;
    }
}