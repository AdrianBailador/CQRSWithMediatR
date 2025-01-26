using CQRSWithMediatR.Data;
using CQRSWithMediatR.Validators;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// (InMemory Database)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));


builder.Services.AddControllers();

// MediatR
builder.Services.AddMediatR(typeof(Program));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.UseHttpsRedirection()
   .Use((context, next) =>
   {
       context.Request.Scheme = "https";
       return next();
   });


app.Run();
