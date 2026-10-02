using _28_VuDucLuc_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 1. Đăng ký Controllers + OData (không dùng EDM model tường minh)
// =====================================================
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100));

// =====================================================
// 2. Đăng ký Repositories (DI)
// =====================================================
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();

// =====================================================
// 3. CORS — cho phép FrontEnd gọi API
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7034",
                "http://localhost:5246",
                "https://localhost:7123",
                "http://localhost:5157")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =====================================================
// 4. Swagger
// =====================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =====================================================
// 5. Middleware Pipeline
// =====================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontEnd");

app.UseAuthorization();

app.MapControllers();

app.Run();
