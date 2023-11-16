using ApiRestBot.Domain;
using ApiRestBot.Hubs;
using ApiRestBot.Providers;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Configuracion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddAuthentication(
    x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
        var Key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };


    });
    services.AddSingleton<IJWTManagerRepository, JWTManagerRepository>();
    services.AddControllers();
    services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
    {
        builder
        .WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod();
    }));
    services.AddSignalR(oprions =>
    {
        oprions.EnableDetailedErrors = true;
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options=>
    { 
    options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Api Bibliochat UTN",
                        Description = "Esta api esta construida para proveer data respecto al funcionaiento del Bibliochat universitario de la UTN",
                        Version = "v1"
                    }
                    );
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
    //var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
    //options.IncludeXmlComments(filePath);
     });

    services.AddScoped<IRepositoriesBot, RepositoriesBot>();
    services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
    services.Configure<MailConfiguracion>(builder.Configuration.GetSection("MailConfigracion"));
    string mySqlConnectionStr = builder.Configuration.GetConnectionString("ConnectionStringMySQL");
    services.AddDbContext<MySqlContext>(options => options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));

}




var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Bibliochat UTN");
    });
}
// global cors policy
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication(); // This need to be added	
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotifyHub>("/prueba");
});

app.MapControllers();

app.Run();
