using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnet02_boardapp.Models
{
    // aspnet이라는 테이플의 게시판을 위한 테이블 매핑 정보
    public class Board
    {   // key 타이핑 하지말고 탭누르면 using system 생성
        [Key] // PK   
        public int Id { get; set; }

        // [Required] : 데이터값이 무조건 들어가야함 -> NOT NULL
        [Required(ErrorMessage = "아이디를 입력하세요")] 
        [DisplayName("아이디")]
        public string UserId { get; set; }
        [DisplayName("이름")]
        public string? UserName { get; set; } // NULL 허용

        [Required(ErrorMessage ="제목을 입력하세요")] // 데이터값이 무조건 들어가야함 -> NOT NULL
        [MaxLength(200)] // == Varchar(200) 200자 제한
        [DisplayName("제목")]
        public string Title { get; set; }
        [DisplayName("조회")]
        public int ReadCount { get; set; }
        [DisplayName("작성일")]
        public DateTime PostDate { get; set; }
        [DisplayName("게시글")]
        public string Contents { get; set; }

        [NotMapped]
        [DisplayName("번호")]
        public int rowNum { get; set; }
    }
}
