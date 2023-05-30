using aspnet02_boardapp.Data;
using aspnet03_portpofioWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace aspnet03_portpofioWebApp.Controllers
{
    public class PortfolioController : Controller
    {
        // DB 부분
        private readonly ApplicationDbContext _db;

        //파일업로드 웹환경 객체 - 필수. 이게 없으면 웹을 통해서 파일을 업로드 할 수 없음
        private readonly IWebHostEnvironment _environment;
        //생성자 생성
        public PortfolioController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var list = _db.Portfolios.ToList(); // SELECT *
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() 
        {   // PortfolioModel이 아니고(X) => TempPortfolioModel 선택(O)
            return View();
        }

        [HttpPost]
        public IActionResult Create(TempPortfolioModel temp)
        {
            // 파일업로드, Temp -> Model DB저장
            if (ModelState.IsValid) 
            {
                // 파일업로드되면 새로운 이미지 파일명을 받음
                string upFileName = UploadImageFile(temp); // 파일업로드메서드 생성

                // TempPortfolioModel에서 -> PortfolioModel로 변경
                var portfolio = new PortfolioModel()
                {
                    Division = temp.Division,
                    Title = temp.Title,
                    Description = temp.Description,
                    Url = temp.Url,
                    FileName = upFileName // 이게 핵심. filename을 업로드된 파일로 쓰기때문에 upfilename으로
                };

                _db.Portfolios.Add(portfolio);
                _db.SaveChanges();

                TempData["succeed"] = "포트폴리오 저장완료!";
                return RedirectToAction("Index", "Portfolio");
            }
            // 실패했다면 화면그대로
            return View(temp);
        }
        // Routing이나 GET/POST랑 관계없음. 파일업로드 내부 메서드.
		private string UploadImageFile(TempPortfolioModel temp)
		{
            var resultFileName = "";

            try
            {
				// 파일업로드 로직
				if (temp.PortfolioImage != null)
				{                                   // environment.WebRootPath = 웹서버의 위치를 알려줘
					string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads"); // wwwroot 밑에 uploads라는 폴더 존재
					resultFileName = Guid.NewGuid() + "_" + temp.PortfolioImage.FileName; // Guid.NewGuid = 25자리 한번도 중복되지 않게함, 중복된 이미지파일명 제거
					string filePath = Path.Combine(uploadFolder, resultFileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						temp.PortfolioImage.CopyTo(fileStream); // 복사가 끝남
					}
				}
			}
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // 복사 끝나면
            return resultFileName; // 파일이 없으면 "", 있으면 fileName을 전달
		}
	}
}
