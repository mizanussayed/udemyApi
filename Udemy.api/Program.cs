using Udemy.api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEssentials(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ConfigureSwagger();
}
app.ConfigureStaticFile();
app.UseHttpsRedirection();
app.UseMiddleware<Udemy.api.GlobalErrorHandlerMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
