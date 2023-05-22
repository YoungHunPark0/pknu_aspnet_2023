using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnet02_boardapp.Controllers
{
    // https://localhost:7800/Board/Index 로 호출
    public class BoardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BoardController(ApplicationDbContext db)
        {
            _db = db; // 알아서 DB가 연결됨
        }

        public IActionResult Index() // 게시판 최초화면 리스트
        {
            IEnumerable<Board> objBoardList = _db.Boards.ToList(); // SELECT쿼리
            return View(objBoardList);
        }

        // https://localhost:7237/Board/Create
        // GetMethod로 화면을 URL로 부를때 쓰는 액션
        [HttpGet]
        public IActionResult Create() // 게시판 글쓰기 // create 우클릭 뷰-추가
        {
            return View();
        }

        // Submit이 발생해서 내부로 DB처리하기위한 데이터를 전달 액션
        [HttpPost]
        [ValidateAntiForgeryToken] // 검증하던 내용중 해킹관련 막아줌
        public IActionResult Create(Board board)
        {
            _db.Boards.Add(board); // INSERT. board를 받아서
            _db.SaveChanges(); // COMMIT. 커밋

            return RedirectToAction("Index", "Board");
        }
    }
}
