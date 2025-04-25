using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StrongFitApp.Models;

namespace StrongFitApp.Data
{
    // IMPORTANTE: tem q herdar do IdentityDbContext
    public class StrongFitContext : IdentityDbContext
    {
        public StrongFitContext(DbContextOptions<StrongFitContext> options)
            : base(options)
        {
        }

        public DbSet<Personal> Personals { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Treino> Treinos { get; set; }
        public DbSet<Exercicio> Exercicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANTE:  chamada base cria as tabelas do Identity
            base.OnModelCreating(modelBuilder);

            // tabelas da aplicação
            modelBuilder.Entity<Personal>().ToTable("Personals");
            modelBuilder.Entity<Aluno>().ToTable("Alunos");
            modelBuilder.Entity<Treino>().ToTable("Treinos");
            modelBuilder.Entity<Exercicio>().ToTable("Exercicios");

            //relacionamento entre Treino e Exercicio
            modelBuilder.Entity<Exercicio>()
                .HasOne(e => e.Treino)
                .WithMany(t => t.Exercicios)
                .HasForeignKey(e => e.TreinoID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}