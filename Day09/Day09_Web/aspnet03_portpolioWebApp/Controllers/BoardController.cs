﻿using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

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

        // startCount = 1, 11, 21, 31, 41..
        // endCount = 10, 20, 30, 40, 50 ..
        public IActionResult Index(int page = 1) // 게시판 최초화면 리스트. 최초페이지 1
        {
            ViewData["NoScroll"] = "true"; // 게시판은 메인스크롤이 안생김. true 간단하게 문자열로 만듬

            // EntityFramework로 작업해도 되고
            // IEnumerable<Board> objBoardList = _db.Boards.ToList(); // SELECT * 쿼리
            // SQL 쿼리로 작업해도 됨
            //var objBoardList = _db.Boards.FromSql($"SELECT * FROM boards").ToList();
            
            var totalCount = _db.Boards.Count(); // 전체글 갯수 넘어옴. 현재 12개
            var pageSize = 10; // 한게시판에 한페이지 10개씩 리스트
            var totalPage = totalCount / pageSize; // ex 12 / 10 = 1 
            
            if (totalCount % pageSize > 0){ totalPage++; } // 2

            // 제일 첫번째 페이지, 제일 마지막 페이지
            var countPage = 10;
            var startPage = ((page - 1) / countPage) * countPage + 1; // 12개 데이터일때 1페이지, startPage = 1
            var endPage = startPage + countPage - 1; // 10
            if (totalPage < endPage) endPage = totalPage; // 10

            // startcount 1부터 시작해서 endcount 10까지 쭉가다가 넘어가면 2페이지 1부터 다시시작
            int startCount = ((page - 1) * countPage) + 1; // 1, 11 
            int endCount = startCount + (pageSize - 1); // 10, 20

            // HTML화면에서 사용하기 위해서 선언 == ViewData, TempData 동일한 역할
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.StartCount = startCount; // 게시판 번호를 위해서 추가 -  230525 새로추가

            var StartCount = new MySqlParameter("startCount", startCount); // sql 서버마다 다름
            var EndCount = new MySqlParameter("endCount", endCount);

            var objBoardList = _db.Boards.FromSql($"CALL New_PagingBoard({StartCount}, {EndCount})").ToList();
            return View(objBoardList);
        }

        // https://localhost:7237/Board/Create
        // GetMethod로 화면을 URL로 부를때 쓰는 액션
        [HttpGet] // 첫화면
        public IActionResult Create() // 게시판 글쓰기 // create 우클릭 뷰-추가
        {
            ViewData["NoScroll"] = "true"; // 게시판은 메인스크롤이 안생김. true 간단하게 문자열로 만듬

            return View();
        }

        // Submit이 발생해서 내부로 DB처리하기위한 데이터를 전달 액션
        [HttpPost] // 깜박인 이후에 인덱스로 넘어감
        [ValidateAntiForgeryToken] // 검증하던 내용중 해킹관련 막아줌
        public IActionResult Create(Board board)
        {
            try
            {
                board.PostDate = DateTime.Now; // 현재 저장하는 일시를 할당해줌. 안쓰면 0000-01-01 이런식으로 저장됨
                _db.Boards.Add(board); // INSERT. board를 받아서
                _db.SaveChanges(); // COMMIT. 커밋

                TempData["succeed"] = "새 게시글이 저장되었습니다."; // 성공메세지
            }
            catch (Exception)
            {
                TempData["error"] = "게시글 작성에 오류가 발생했습니다.";
            }

            return RedirectToAction("Index", "Board"); // localhost - board- index 화면으로 이동
        }

        [HttpGet]
        public IActionResult Edit(int? Id) 
        {
            ViewData["NoScroll"] = "true"; // 게시판은 메인스크롤이 안생김. true 간단하게 문자열로 만듬

            if (Id == null || Id == 0) 
            {
                return NotFound(); // Error.cshtml이 표시
            }

            var board = _db.Boards.Find(Id); // SELECT * FROM board WHERE Id = @id

            if (board == null) // board가 null이라면
            {
                return NotFound();
            }

            return View(board); // 파라미터인 board 넣어야 나옴!
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Board board) 
        {
            board.PostDate = DateTime.Now; // 현재 저장하는 일시를 할당해줌. 안쓰면 0000-01-01 이런식으로 저장됨
            _db.Boards.Update(board); // UPDATE query문 실행
            _db.SaveChanges(); // 반드시 Commit

            TempData["succeed"] = "게시글을 수정했습니다."; // 성공메세지

            return RedirectToAction("Index", "Board"); // board에 index로 이동
        }

        // 삭제
        [HttpGet]
        public IActionResult Delete(int? Id) 
        {
            ViewData["NoScroll"] = "true"; // 게시판은 메인스크롤이 안생김. true 간단하게 문자열로 만듬

            // HttpGet Edit Action의 로직과 완전동일 
            if (Id == null || Id == 0)
            {
                return NotFound(); // Error.cshtml이 표시
            }

            var board = _db.Boards.Find(Id); // SELECT * FROM board WHERE Id = @id

            if (board == null) // board가 null이라면
            {
                return NotFound();
            }

            return View(board); // 파라미터인 board 넣어야 나옴!
        }

        [HttpPost]
        public IActionResult DeletePost(int? Id)
        {
            var board = _db.Boards.Find(Id);
            
            if (board == null)
            {
                return NotFound();
            }

            _db.Boards.Remove(board); // Delete query 실행
            _db.SaveChanges(); // commint 실행

            TempData["succeed"] = "게시글을 삭제했습니다."; // 성공메세지

            return RedirectToAction("Index", "Board"); // board에 index로 이동
        }

        [HttpGet]
        public IActionResult Details(int? Id) 
        {
            ViewData["NoScroll"] = "true"; // 게시판은 메인스크롤이 안생김. true 간단하게 문자열로 만듬

            if (Id == null || Id == 0)
            {
                return NotFound(); // Error.cshtml이 표시
            }

            var board = _db.Boards.Find(Id); // SELECT * FROM board WHERE Id = @id

            if (board == null) // board가 null이라면
            {
                return NotFound();
            }

            board.ReadCount++; // 조회수 1증가
            _db.Boards.Update(board); // UPDATE 쿼리 실행
            _db.SaveChanges(); // COMMIT 실행


            return View(board); // 파라미터인 board 넣어야 나옴!
        }
    }
}
