using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using StrongFitApp.Models;
using Microsoft.EntityFrameworkCore;
using StrongFitApp.Data;

namespace StrongFitApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StrongFitContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            StrongFitContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O email é obrigatório")]
            [EmailAddress(ErrorMessage = "Email inválido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória")]
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar senha")]
            [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não correspondem.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "O tipo de usuário é obrigatório")]
            [Display(Name = "Tipo de Usuário")]
            public string UserType { get; set; }

            [Required(ErrorMessage = "O nome é obrigatório")]
            [Display(Name = "Nome")]
            public string Nome { get; set; }

            [Display(Name = "Especialidade")]
            public string Especialidade { get; set; }

            [Display(Name = "Data de Nascimento")]
            [DataType(DataType.Date)]
            public DateTime? DataNascimento { get; set; }

            [Display(Name = "Telefone")]
            public string Telefone { get; set; }

            [Display(Name = "Instagram")]
            public string Instagram { get; set; }

            [Display(Name = "Personal")]
            public int? PersonalID { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            await CarregarPersonals();
        }

        private async Task CarregarPersonals()
        {
            ViewData["PersonalID"] = new SelectList(await _context.Personals.ToListAsync(), "PersonalID", "Nome");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Recarregar a lista de personals para o caso de erro e precisar mostrar a página novamente
            await CarregarPersonals();

            // Validação manual para o tipo de usuário Aluno que precisa de um Personal
            if (Input.UserType == "Aluno" && !Input.PersonalID.HasValue)
            {
                ModelState.AddModelError("Input.PersonalID", "É necessário selecionar um Personal.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, EmailConfirmed = true };
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        // Verificar se a role existe, se não, criar
                        if (!await _roleManager.RoleExistsAsync(Input.UserType))
                        {
                            var roleResult = await _roleManager.CreateAsync(new IdentityRole(Input.UserType));
                            if (!roleResult.Succeeded)
                            {
                                foreach (var error in roleResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, $"Erro ao criar role: {error.Description}");
                                }
                                return Page();
                            }
                        }

                        // Adicionar usuário à role
                        var addToRoleResult = await _userManager.AddToRoleAsync(user, Input.UserType);
                        if (!addToRoleResult.Succeeded)
                        {
                            foreach (var error in addToRoleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, $"Erro ao adicionar à role: {error.Description}");
                            }
                            // Remover o usuário criado já que não conseguimos adicionar à role
                            await _userManager.DeleteAsync(user);
                            return Page();
                        }

                        // Criar entidade correspondente (Personal ou Aluno)
                        if (Input.UserType == "Personal")
                        {
                            var personal = new Personal
                            {
                                Nome = Input.Nome,
                                Especialidade = Input.Especialidade ?? "",
                                Email = Input.Email,
                                Telefone = Input.Telefone ?? "",
                                Instagram = Input.Instagram ?? ""
                            };

                            _context.Personals.Add(personal);
                            await _context.SaveChangesAsync();

                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect("~/Home/Index");
                        }
                        else if (Input.UserType == "Aluno")
                        {
                            var aluno = new Aluno
                            {
                                Nome = Input.Nome,
                                Email = Input.Email,
                                Data_Nascimento = Input.DataNascimento ?? DateTime.Now,
                                Telefone = Input.Telefone ?? "",
                                Instagram = Input.Instagram ?? "",
                                PersonalID = Input.PersonalID.Value,
                                Observacoes = ""
                            };

                            _context.Alunos.Add(aluno);
                            await _context.SaveChangesAsync();

                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect("~/Treinos/MeusTreinos");
                        }
                        else if (Input.UserType == "Admin")
                        {
                            // Para Admin, não precisamos criar entidade adicional
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect("~/Home/Index");
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    // Log da exceção
                    _logger.LogError(ex, "Erro ao registrar usuário");
                    ModelState.AddModelError(string.Empty, $"Ocorreu um erro: {ex.Message}");
                }
            }

            // Se chegamos até aqui, algo falhou, redisplay form
            return Page();
        }
    }
}
