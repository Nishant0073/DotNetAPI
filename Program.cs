var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://localhost:4200/", "http://localhost:300/", "http://localhost:8000/")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyHeader();
    });

     options.AddPolicy("ProdCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://myprodcutions.com:4200/")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyHeader();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.MapControllers();
app.Run();

// Server=localhost;Database=DotNetCourseDatabase;Trusted_Connection=false;TrustServerCertificate=True;User Id=sa;Password=SQLConnect1;