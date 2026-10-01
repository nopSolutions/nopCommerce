using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nop.Plugin.Api.Rest.Infrastructure;

/// <summary>
/// Removes the back office security scheme from the public store document
/// </summary>
/// <remarks>
/// Swashbuckle has no per document security definitions: <c>AddSecurityDefinition</c> applies to every
/// document the generator produces, so both credentials land on both documents and Swagger UI lists
/// them all in the Authorize dialog regardless of whether any operation uses them. That matters because
/// the two documents accept different credentials: the middleware answers 403 when an admin level
/// credential is presented to a public store route, so advertising the API key there would promise a
/// credential the runtime refuses. This filter corrects the public store document, and leaves the
/// definitions declared in <c>NopStartup</c> as the back office ones.
/// </remarks>
public class FrontendSecuritySchemeFilter : IDocumentFilter
{
    #region Methods

    /// <summary>
    /// Applies the filter to one generated document
    /// </summary>
    /// <param name="document">The generated document</param>
    /// <param name="context">Filter context</param>
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        //document filters run for every document the generator produces, so this one has to opt out
        if (context.DocumentName != NopStartup.FrontendSwaggerGroup)
            return;

        var schemes = document.Components?.SecuritySchemes;
        if (schemes == null)
            return;

        //no operation in this document accepts an admin level credential, so it should not be offered
        schemes.Remove(ApiRestDefaults.ApiKeySchemeId);

        //a requirement naming a scheme that is no longer declared emits a dangling $ref, which is not
        //valid OpenAPI and makes generators reject the document. Nothing references the API key here
        //today, but sweeping keeps the document self consistent whatever an operation filter does later.
        //The keys are removed by instance rather than by an equal looking one, because the scheme type
        //does not reliably compare by value and a constructed key would not match.
        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations.Values))
        {
            if (operation.Security == null)
                continue;

            foreach (var requirement in operation.Security)
            {
                var stale = requirement.Keys
                    .Where(scheme => scheme.Reference?.Id == ApiRestDefaults.ApiKeySchemeId)
                    .ToList();

                foreach (var scheme in stale)
                    requirement.Remove(scheme);
            }
        }

        //the shared Bearer description is written for the back office, where an admin token and the API
        //key both apply. Here the only token is the customer one, so describe that instead
        if (schemes.TryGetValue(ApiRestDefaults.BearerSchemeId, out var bearer))
            bearer.Description = FrontendBearerDescription;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// The Bearer scheme description published in the public store document
    /// </summary>
    private const string FrontendBearerDescription =
        "A signed JWT with an expiry, scoped to one customer. Post the customer's credentials to "
        + "POST /api/rest/customer/token and read the token field of the response. The shared API key "
        + "and an admin token are both refused here with 403; only this document's token is accepted. "
        + "Tokens expire, the response says when, and there is no refresh: post the credentials again "
        + "for a new one.";

    #endregion
}