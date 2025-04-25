using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrongFitApp.Models
{
    public class Exercicio
    {
        public int ExercicioID { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        [StringLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres")]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; }

        // Adicionando propriedade TreinoID que estava faltando
        [Display(Name = "Treino")]
        public int? TreinoID { get; set; }

        // Adicionando relacionamento com Treino
        [ForeignKey("TreinoID")]
        public Treino Treino { get; set; }
    }
}