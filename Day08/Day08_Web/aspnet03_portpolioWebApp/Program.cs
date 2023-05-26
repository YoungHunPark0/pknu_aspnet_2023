using aspnet02_boardapp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace aspnet02_boardapp
{
    public class Program
    {
        // ASP.NET 실행을 위한 구성초기화. 여기에 내용을 다 넣음
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Data에서 만든 ApplicationDbContext를 사용하겠다는 설정 추가
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
                // appsetttings.json ConnectionStrings내의 연결문자열 할당
                /*
                 "ConnectionStrings": {
                    "DefaultConnection": "server=localhost;port=3306;database=aspnet;user=root;password=12345;"}
                 */
                builder.Configuration.GetConnectionString("DefaultConnection"),
                // 연결문자열로 DB의 서버 버전을 자동으로 가져올것. 위의 DefaultConnection의 DB정보와 합쳐서 사용
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
            ));
            // 2. ASP.NET Identity - ASP.NET 계정을 위한 서비스 설정
            builder.Services.AddIdentity<IdentityUser, IdentityRole>() // role은 관리자
                .AddEntityFrameworkStores<ApplicationDbContext>() // 어디저장했는가 - ApplicationDbContext
                .AddDefaultTokenProviders();

            // 비밀번호 정책 변경 설정
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // 원래 기본으로 있던 복잡한 조건 없애는 중.
                // 실무에서는 아래와같이 변경하면 안됨- 보안문제생김
                // Custom Password policy.
                options.Password.RequireDigit = false; // 영문자 필요여부
                options.Password.RequireLowercase = false; // 소문자 필요여부
                options.Password.RequireUppercase = false; // 대문자 필요여부
                options.Password.RequireNonAlphanumeric = false; // 특수문자 필요여부
                options.Password.RequiredLength = 4; // 최소 패스워드 길이 수
                options.Password.RequiredUniqueChars = 0; // 암호 고유문자 갯수
            });

            // 권한을 위한 설정추가
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // 3. ASP.NET Identity - 계정추가
            app.UseAuthorization(); // 권한

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}