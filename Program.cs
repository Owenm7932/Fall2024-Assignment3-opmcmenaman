using Fall2024_Assignment3_opmcmenaman.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var dbUsername = builder.Configuration["DbUsername"];
var dbPassword = builder.Configuration["DbPassword"];
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
var openAiEndpoint = builder.Configuration["OpenAI:Endpoint"];

var connectionString = $"Server=tcp:fall2024-assignment3-opmcmenaman.database.windows.net,1433;Initial Catalog=Fall2024-Assignment3-opmcmenaman;Persist Security Info=False;User ID={dbUsername};Password={dbPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<OpenAIService>(sp => new OpenAIService(openAiApiKey, openAiEndpoint));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
