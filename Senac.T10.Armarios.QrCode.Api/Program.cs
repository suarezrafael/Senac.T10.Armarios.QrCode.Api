using Microsoft.EntityFrameworkCore;
using Senac.T10.Armarios.QrCode.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
var conexao = builder.Configuration.GetConnectionString("conexao");
builder.Services.AddDbContext<AppDbContext>(opcoes =>
{
    // opcoes.UseMySql(conexao, ServerVersion.Parse("10.4.28-MariaDB"));
    opcoes.UseSqlServer(conexao);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
