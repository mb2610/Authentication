using MyApi.Configuration;

namespace MyApi.Helpers;

public static class ConfigureApplicationHelpers
{
   public static void RegisterSwagger(this IApplicationBuilder app, ApiConfiguration apiConfiguration)
   {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
         c.SwaggerEndpoint($"{apiConfiguration.ApiBaseUrl}/swagger/{apiConfiguration.ApiVersion}/swagger.json", apiConfiguration.ApiName);
         // c.OAuthClientId(apiConfiguration.OidcSwaggerUIClientId);
         // c.OAuthAppName(apiConfiguration.ApiName);
         // c.OAuthUsePkce();
         
      });
   }

}