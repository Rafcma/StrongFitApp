using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StrongFitApp.Data;
using StrongFitApp.Models;

namespace StrongFitApp.Controllers
{
    [Authorize(Roles = "Personal")]
    public class AlunosController : Controller
    {
        private readonly StrongFitContext _context;

        public AlunosController(StrongFitContext context)
        {
            _context = context;
        }

        // GET: Alunos
        public async Task<IActionResult> Index()
        {
            var alunos = await _context.Alunos
                .Include(a => a.Personal)
                .ToListAsync();
            return View(alunos);
        }

        // GET: Alunos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Alunos
                .Include(a => a.Personal)
                .Include(a => a.Treinos)
                .FirstOrDefaultAsync(m => m.AlunoID == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        // GET: Alunos/Create
        public IActionResult Create()
        {
            ViewData["PersonalID"] = new SelectList(_context.Personals, "PersonalID", "Nome");
            return View();
        }

        // POST: Alunos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Data_Nascimento,Email,Telefone,Instagram,Observacoes,PersonalID")] Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aluno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonalID"] = new SelectList(_context.Personals, "PersonalID", "Nome", aluno.PersonalID);
            return View(aluno);
        }

        // GET: Alunos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }
            ViewData["PersonalID"] = new SelectList(_context.Personals, "PersonalID", "Nome", aluno.PersonalID);
            return View(aluno);
        }

        // POST: Alunos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlunoID,Nome,Data_Nascimento,Email,Telefone,Instagram,Observacoes,PersonalID")] Aluno aluno)
        {
            if (id != aluno.AlunoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aluno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlunoExists(aluno.AlunoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonalID"] = new SelectList(_context.Personals, "PersonalID", "Nome", aluno.PersonalID);
            return View(aluno);
        }

        // GET: Alunos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Alunos
                .Include(a => a.Personal)
                .FirstOrDefaultAsync(m => m.AlunoID == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        // POST: Alunos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno != null)
            {
                // Verificar se o aluno tem treinos
                var temTreinos = await _context.Treinos.AnyAsync(t => t.AlunoID == id);
                if (temTreinos)
                {
                    // Excluir os treinos do aluno
                    var treinos = await _context.Treinos.Where(t => t.AlunoID == id).ToListAsync();
                    foreach (var treino in treinos)
                    {
                        // Excluir os exercícios do treino
                        var exercicios = await _context.Exercicios.Where(e => e.TreinoID == treino.TreinoID).ToListAsync();
                        _context.Exercicios.RemoveRange(exercicios);
                    }
                    _context.Treinos.RemoveRange(treinos);
                }

                _context.Alunos.Remove(aluno);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AlunoExists(int id)
        {
            return _context.Alunos.Any(e => e.AlunoID == id);
        }
    }
}