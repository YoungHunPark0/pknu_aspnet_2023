﻿using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet02_boardapp.Controllers
{
    public class AdminController : Controller
    {
        // private 변수는 _ 이런형식 많이씀
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleModel model)
        {
            if (ModelState.IsValid) 
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(role); // 지정한 권한명이 DB - aspnetroles에 저장됨

                if (result.Succeeded) // 성공했으면 
                {
                    // TODO : 토스트메세지 추가
                    return RedirectToAction("ListRoles", "Admin");
                }

                foreach (var error in result.Errors) 
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id) 
        {
            var role = await _roleManager.FindByIdAsync(id); // aspnetroles 테이플에서 id로 조회(내부적으로 select문 사용)
            if (role == null) 
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }
            // 오류가 없으면
            var model = new EditRoleModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            var userList = await _userManager.Users.ToListAsync(); // 사용자 리스트

            foreach (var user in userList) // 모든 user를 다 가지고와서
            {
                if (await _userManager.IsInRoleAsync(user, role.Name)) 
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleModel model) 
        {
            var role = await _roleManager.FindByIdAsync(model.Id); // aspnetroles 테이플에서 id로 조회(내부적으로 select문 사용)
            if (role == null)
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }

            var model = new List<UserRoleModel>();

            var userList = await _userManager.Users.ToListAsync();

            foreach (var user in userList)
            {
                var userRoleViewModel = new UserRoleModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleModel> model, string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }

            IdentityResult result = null;

            foreach (var user in model)
            {
                var userObj = await _userManager.FindByIdAsync(user.UserId);

                if (user.IsSelected && !(await _userManager.IsInRoleAsync(userObj, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(userObj, role.Name); // 사용자에게 권한 할당
                }
                else if (!user.IsSelected && await _userManager.IsInRoleAsync(userObj, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(userObj, role.Name); // 사용자를 권한에서 삭제
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId }); // 끝나면 editrole 화면으로 돌아감
        }
    }
}
