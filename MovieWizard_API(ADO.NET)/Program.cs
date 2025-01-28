using MovieWizardAPI.Data;
using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ISecurityService, SecurityService>();        // No change needed
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IGenreService, GenreService>();

builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<IActorService, ActorService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();  // Changed to Scoped
builder.Services.AddScoped<IUserService, UserService>();        // No change needed

builder.Services.AddScoped<ITransactionalRepository, TransactionalRepository>();  // Changed to Scoped
builder.Services.AddScoped<IUserService, UserService>();        // No change needed


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//Minimal Apis
app.MapGet("/Hello", () => "Hello!");
app.MapGet("/HelloWithName", (string name) => $"Hello {name} I am from Middleware");

// Configure the HTTP request pipeline.

app.UseAuthentication(); // This must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();
app.UseCors(options => options
    .AllowAnyOrigin()  // Allow requests from any origin
    .AllowAnyMethod()   // Allow any HTTP method (GET, POST, PUT, etc.)
    .AllowAnyHeader()); // Allow any request header


app.Run();

