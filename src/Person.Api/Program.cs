using Microsoft.EntityFrameworkCore;
using Person.Api;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen();   


// Adds Swagger generation
builder.Services.AddDbContext<Person.Api.Models.PersonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.RegisterEndpoints();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

