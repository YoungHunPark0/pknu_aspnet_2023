using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApiServer.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }
        
        [Column(TypeName = "Varchar(100)")] // title은 길어서 이렇게 지정안하면 안됨
        public string? Title { get; set; }
        
        public string TodoDate { get; set; }
        public int IsComplete { get; set; } // db값도 tinyint여서 int로 변경
    }
}
