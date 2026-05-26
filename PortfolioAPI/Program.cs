using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortfolioAPI.Data;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Allows our app to make external web requests
builder.Services.AddHttpClient();


// 1. Use the native .NET 9 OpenAPI generator
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Allow Angular to talk to the API
// Allow Vercel to talk to the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel",
        policy =>
        {
            policy.AllowAnyOrigin() // This safely allows your Vercel site through
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong!"); // Change this to a secure random string!
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // This automatically builds your tables!
}

if (app.Environment.IsDevelopment())
{
    // 2. Generate the API data
    app.MapOpenApi();

    // 3. Attach the beautiful new Scalar UI
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseCors("AllowVercel");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
