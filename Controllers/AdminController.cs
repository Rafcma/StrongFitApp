using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrongFitApp.Data;
using System.Threading.Tasks;

namespace StrongFitApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataSeeder _seeder;

        public AdminController(DataSeeder seeder)
        {
            _seeder = seeder;
        }

        // GET: /Admin/SeedData
        public IActionResult SeedData()
        {
            return View();
        }

        // POST: /Admin/SeedData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedDataConfirmed()
        {
            await _seeder.SeedAsync();
            TempData["Message"] = "Banco de dados populado com sucesso!";
            return RedirectToAction("Index", "Home");
        }
    }
}