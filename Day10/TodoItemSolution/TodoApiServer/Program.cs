using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TodoApiServer.Models;

namespace TodoApiServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(); // 서버에 CORS 설정
            
            // Add services to the container.
            // ntt프레임워크에서 db랑 연동
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
                ));

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

            // 서버에 CORS 사용허가                어떤 메소드, 헤더든 허용
            app.UseCors(options => options.AllowAnyOrigin()
                                          .AllowAnyMethod()
                                          .AllowAnyHeader());

            app.MapControllers();

            app.Run();
        }
    }
}