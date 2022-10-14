using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<robot_controller_api.Persistence.IRobotCommandDataAccess, robot_controller_api.Persistence.RobotCommandRepository>();
builder.Services.AddScoped<robot_controller_api.Persistence.IMapDataAccess, robot_controller_api.Persistence.MapRepository>();
builder.Services.AddScoped<robot_controller_api.Persistence.IUserDataAccess, robot_controller_api.Persistence.UserRepository>();
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, robot_controller_api.Authentication.BasicAuthenticationHandler>("BasicAuthentication", default);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "admin", "ADMIN"));
    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "User", "admin", "user", "ADMIN", "USER"));
    options.AddPolicy("RegisteredOnly", policy => policy.RequireClaim(ClaimTypes.Email));
});
// === enable the generation of OpenAPI Speicification files ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  // filling authorship
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "Robot Controller API",
    Description = "New backend service that provides resources for the Moon robot simulator.",
    Contact = new OpenApiContact
    {
      Name = "Betty Yuliani",
      Email = "byuliani@deakin.edu.au"
    },
  });

  // generates xml comments file
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  // pass the file into open API generator
  // open API generator gets the comments out and use them in the documentation
  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

var app = builder.Build();

app.UseStaticFiles(); //enable static files usage (CSS, favicons, images from wwwroot)
app.UseSwagger();

//overloaded version of UseSwaggerUI(). Action<SwaggerUIOptions> is an optional parameter
//since Action is a predefined generic delegates, lambda expressions can be used here
app.UseSwaggerUI(setup => setup.InjectStylesheet("/styles/theme-muted.css"));

// if (app.Environment.IsDevelopment()) app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
