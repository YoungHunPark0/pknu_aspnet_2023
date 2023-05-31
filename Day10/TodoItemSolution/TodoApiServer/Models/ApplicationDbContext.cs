using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace TodoApiServer.Models
{
    public class ApplicationDbContext : DbContext // ApplicationDbContext은 DbContext를 상속해야됨
    {
        // 생성자-옵션 마법사로 생성
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        // 이거 없으면 db랑 연결안됨. 이렇게하면 db상에 TodoItems 테이블이 생성됨 
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
