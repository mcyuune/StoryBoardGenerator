using Microsoft.EntityFrameworkCore;
using StoryBoardGenerator.Data;
using StoryBoardGenerator.Services;
using StoryBoardGenerator.Services.ImageGenerator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSingleton<ISceneSplitter, RuleSplitGenerator>();
builder.Services.AddHttpClient<IImageGenerator, OpenAiImageGenerator>();
builder.Services.AddScoped<StoryBoardService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
