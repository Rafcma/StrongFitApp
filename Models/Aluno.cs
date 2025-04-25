using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StrongFitApp.Models
{
    public class Aluno
    {
        public int AlunoID { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime Data_Nascimento { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        [Display(Name = "Instagram")]
        public string Instagram { get; set; }

        [Required(ErrorMessage = "O personal é obrigatório")]
        [Display(Name = "Personal")]
        public int PersonalID { get; set; }

        // Adicionando propriedade Observacoes que estava faltando
        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        // Relacionamentos
        public Personal Personal { get; set; }
        public List<Treino> Treinos { get; set; } = new List<Treino>();
    }
}