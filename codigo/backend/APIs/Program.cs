using System.Reflection;
using APIs.Contexto;
using APIs.Interfaces;
using APIs.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("local");
const string myCors = "allowAll";
builder.Services.AddCors(options => options.AddPolicy(name: myCors, policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); }));
builder.Services.AddDbContext<AppDbContexto>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAlunoRepositorio, AlunoServico>();
builder.Services.AddScoped<IEmpresaRepositorio, EmpresaServico>();
builder.Services.AddScoped<IAuthRepositorio, AuthServico>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
  {
      options.Audience = builder.Configuration["Jwt:ValidAudience"];
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
          ValidateAudience = true,
          ValidAudience = builder.Configuration["Jwt:ValidAudience"],
          ValidateLifetime = false,
          ValidateIssuerSigningKey = true
      };
  });
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Laboratório - ICEI PUCMINAS",
        Version = "v1"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myCors);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
