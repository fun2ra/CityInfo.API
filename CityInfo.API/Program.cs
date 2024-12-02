using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using CityInfo.API.Services;
using CityInfo.API;
using CityInfo.API.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddDbContext<CityInfoContext>(dbContextOptions =>
{
    dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDatabase"]);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


//app.MapControllers();

app.Run();
