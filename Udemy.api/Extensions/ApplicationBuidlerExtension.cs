using Microsoft.Extensions.FileProviders;

namespace Udemy.api.Extensions;

public static class ApplicationBuidlerExtension
{
    public static void ConfigureSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "UdemyApi");
            options.RoutePrefix = "swagger";
            options.DisplayRequestDuration();
        });
    }
    public static void ConfigureStaticFile(this IApplicationBuilder app)
    {
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
            RequestPath = "/Images"
        });
    }
}
