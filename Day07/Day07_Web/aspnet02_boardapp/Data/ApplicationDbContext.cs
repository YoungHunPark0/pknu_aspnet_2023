using aspnet02_boardapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace aspnet02_boardapp.Data
{   // DbContent는 EntityFrameworkCore안에 기본으로 있는것
    public class ApplicationDbContext : IdentityDbContext // 1. ASP.NET Identity : DbContext -> IdentityDbContext 결국 DbContext 쓰는것하고 동일
    {
        // 생성자 alt+tab해서 자동생성
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        // DB서버 주소 중간자 Board라는 컬럼의 데이터값은 1개, 그 이상이면 Boards
        public DbSet<Board> Boards { get; set; } // Borad라는 테이블 생성
    }
}
