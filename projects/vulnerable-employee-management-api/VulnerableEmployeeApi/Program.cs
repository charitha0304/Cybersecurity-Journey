using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VulnerableEmployeeApi.Data;
using VulnerableEmployeeApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<TokenService>();

// VULN (insecure configuration — CORS): any website may call this API with
// credentials. The fix restricts AllowAnyOrigin to a known front-end origin.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // VULN (weak JWT validation): issuer and audience are not validated, so a
        // token minted for any issuer/audience is accepted. Combined with the weak,
        // crackable signing key (see TokenService / appsettings.json), an attacker
        // can forge a valid Admin token. The fix validates issuer + audience and uses
        // a long random key.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Wire a JWT "Authorize" button into Swagger so you can paste a token from
// /api/auth/login and have Swagger attach "Authorization: Bearer <token>" to the
// protected endpoints. (This is tester convenience; the API is still insecure.)
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the token from /api/auth/login. Don't type 'Bearer ' — just the token."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// NOTE (missing rate limiting): .NET has built-in rate limiting
// (builder.Services.AddRateLimiter(...)). It is deliberately NOT registered here, so
// the login endpoint can be brute-forced. The fix adds a limiter and app.UseRateLimiter().

var app = builder.Build();

// Seed the SQLite database on first run.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

// VULN (insecure configuration — verbose errors + Swagger always on): the developer
// exception page and Swagger UI are enabled unconditionally, leaking stack traces and
// the full API surface even outside Development. The fix guards these with
// app.Environment.IsDevelopment().
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

// VULN (insecure configuration — no transport security): HTTPS redirection is not
// enforced, so tokens and credentials can travel in clear text. The fix adds
// app.UseHttpsRedirection() and runs the API over HTTPS.

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
