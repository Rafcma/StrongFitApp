using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StrongFitApp.Models
{
    public class Personal
    {
        public int PersonalID { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A especialidade é obrigatória")]
        [StringLength(100, ErrorMessage = "A especialidade deve ter no máximo 100 caracteres")]
        [Display(Name = "Especialidade")]
        public string Especialidade { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = ""; // Inicializar com string vazia

        // Relacionamento com Alunos
        public List<Aluno> Alunos { get; set; } = new List<Aluno>();
    }
}