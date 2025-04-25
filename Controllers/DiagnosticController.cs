using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrongFitApp.Data;
using StrongFitApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrongFitApp.Controllers
{
    [AllowAnonymous] // Importante: permitir acesso sem autenticação
    public class DiagnosticController : Controller
    {
        private readonly StrongFitContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DiagnosticController> _logger;

        public DiagnosticController(
            StrongFitContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DiagnosticController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Status()
        {
            var statusInfo = new Dictionary<string, object>();

            // Verificar autenticação
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var roles = await _userManager.GetRolesAsync(user);

                statusInfo.Add("IsAuthenticated", true);
                statusInfo.Add("UserName", User.Identity.Name);
                statusInfo.Add("Roles", roles.ToList());
            }
            else
            {
                statusInfo.Add("IsAuthenticated", false);
                statusInfo.Add("UserName", "Não autenticado");
                statusInfo.Add("Roles", new List<string>());
            }

            // Verificar contagem de entidades
            try
            {
                statusInfo.Add("PersonalCount", await _context.Personals.CountAsync());
                statusInfo.Add("AlunoCount", await _context.Alunos.CountAsync());
                statusInfo.Add("TreinoCount", await _context.Treinos.CountAsync());
                statusInfo.Add("ExercicioCount", await _context.Exercicios.CountAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar entidades");
                statusInfo.Add("DatabaseError", ex.Message);
                statusInfo.Add("PersonalCount", 0);
                statusInfo.Add("AlunoCount", 0);
                statusInfo.Add("TreinoCount", 0);
                statusInfo.Add("ExercicioCount", 0);
            }

            return View(statusInfo);
        }

        // Método para criar um usuário admin
        public async Task<IActionResult> CreateAdmin()
        {
            try
            {
                // Verificar se já existe um usuário admin
                var adminUser = await _userManager.FindByEmailAsync("admin@strongfit.com");

                if (adminUser == null)
                {
                    // Criar usuário admin
                    adminUser = new IdentityUser
                    {
                        UserName = "admin@strongfit.com",
                        Email = "admin@strongfit.com",
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin@123");

                    if (result.Succeeded)
                    {
                        // Verificar se a role Admin existe
                        if (!await _roleManager.RoleExistsAsync("Admin"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        }

                        // Adicionar usuário à role Admin
                        await _userManager.AddToRoleAsync(adminUser, "Admin");

                        // Criar um Personal para o admin
                        var personal = new Personal
                        {
                            Nome = "Administrador",
                            Especialidade = "Administração",
                            Email = "admin@strongfit.com"
                        };

                        _context.Personals.Add(personal);
                        await _context.SaveChangesAsync();

                        return View("AdminCreated", "Usuário admin criado com sucesso! Email: admin@strongfit.com, Senha: Admin@123");
                    }
                    else
                    {
                        return View("AdminCreated", $"Erro ao criar usuário admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    return View("AdminCreated", "Usuário admin já existe!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário admin");
                return View("AdminCreated", $"Erro: {ex.Message}");
            }
        }

        // Método para limpar e recriar o banco de dados
        public async Task<IActionResult> ResetDatabase()
        {
            try
            {
                // Excluir todos os dados
                var exercicios = await _context.Exercicios.ToListAsync();
                _context.Exercicios.RemoveRange(exercicios);

                var treinos = await _context.Treinos.ToListAsync();
                _context.Treinos.RemoveRange(treinos);

                var alunos = await _context.Alunos.ToListAsync();
                _context.Alunos.RemoveRange(alunos);

                var personals = await _context.Personals.ToListAsync();
                _context.Personals.RemoveRange(personals);

                await _context.SaveChangesAsync();

                // Recriar dados iniciais
                var seeder = new DataSeeder(_context, _userManager, _roleManager);
                await seeder.SeedAsync();

                return View("AdminCreated", "Banco de dados limpo e recriado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao resetar banco de dados");
                return View("AdminCreated", $"Erro ao resetar banco de dados: {ex.Message}");
            }
        }
    }
}