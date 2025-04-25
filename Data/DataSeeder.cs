using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StrongFitApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrongFitApp.Data
{
    public class DataSeeder
    {
        private readonly StrongFitContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(
            StrongFitContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Criar roles
            await CreateRolesAsync();

            // Criar usuários
            await CreateUsersAsync();

            // Criar dados iniciais
            await CreateInitialDataAsync();
        }

        private async Task CreateRolesAsync()
        {
            string[] roleNames = { "Admin", "Personal", "Aluno" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task CreateUsersAsync()
        {
            // Criar usuário admin
            var adminUser = new IdentityUser
            {
                UserName = "admin@strongfit.com",
                Email = "admin@strongfit.com",
                EmailConfirmed = true
            };

            if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await _userManager.CreateAsync(adminUser, "Admin@123");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Criar usuário personal
            var personalUser = new IdentityUser
            {
                UserName = "personal@strongfit.com",
                Email = "personal@strongfit.com",
                EmailConfirmed = true
            };

            if (await _userManager.FindByEmailAsync(personalUser.Email) == null)
            {
                await _userManager.CreateAsync(personalUser, "Personal@123");
                await _userManager.AddToRoleAsync(personalUser, "Personal");
            }

            // Criar usuário aluno
            var alunoUser = new IdentityUser
            {
                UserName = "aluno@strongfit.com",
                Email = "aluno@strongfit.com",
                EmailConfirmed = true
            };

            if (await _userManager.FindByEmailAsync(alunoUser.Email) == null)
            {
                await _userManager.CreateAsync(alunoUser, "Aluno@123");
                await _userManager.AddToRoleAsync(alunoUser, "Aluno");
            }
        }

        private async Task CreateInitialDataAsync()
        {
            // Criar personals
            var personal1 = new Personal
            {
                Nome = "João Silva",
                Especialidade = "Musculação",
                Email = "joao.silva@strongfit.com"
            };

            var personal2 = new Personal
            {
                Nome = "Maria Oliveira",
                Especialidade = "Funcional",
                Email = "maria.oliveira@strongfit.com"
            };

            _context.Personals.Add(personal1);
            _context.Personals.Add(personal2);
            await _context.SaveChangesAsync();

            // Criar alunos
            var aluno1 = new Aluno
            {
                Nome = "Pedro Santos",
                Email = "aluno@strongfit.com",
                Data_Nascimento = DateTime.Now.AddYears(-25),
                Telefone = "(11) 98765-4321",
                Instagram = "@pedrosantos",
                PersonalID = personal1.PersonalID,
                Observacoes = "Aluno iniciante"
            };

            _context.Alunos.Add(aluno1);
            await _context.SaveChangesAsync();

            // Criar exercícios
            var exercicios = new[]
            {
                new Exercicio { Nome = "Supino Reto", Descricao = "3 séries de 12 repetições", Categoria = "Peito" },
                new Exercicio { Nome = "Agachamento", Descricao = "4 séries de 10 repetições", Categoria = "Pernas" },
                new Exercicio { Nome = "Puxada Frontal", Descricao = "3 séries de 12 repetições", Categoria = "Costas" },
                new Exercicio { Nome = "Rosca Direta", Descricao = "3 séries de 15 repetições", Categoria = "Braços" },
                new Exercicio { Nome = "Elevação Lateral", Descricao = "3 séries de 15 repetições", Categoria = "Ombros" }
            };

            _context.Exercicios.AddRange(exercicios);
            await _context.SaveChangesAsync();

            // Criar treino
            var dataAtual = DateTime.Now;
            var treino = new Treino
            {
                AlunoID = aluno1.AlunoID,
                Data = dataAtual,
                Hora = dataAtual,
                Observacoes = "Treino inicial"
            };

            _context.Treinos.Add(treino);
            await _context.SaveChangesAsync();

            // Adicionar exercícios ao treino
            foreach (var exercicio in exercicios.Take(3))
            {
                exercicio.TreinoID = treino.TreinoID;
            }

            await _context.SaveChangesAsync();
        }
    }
}