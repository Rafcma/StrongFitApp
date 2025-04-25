using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StrongFitApp.Data;
using StrongFitApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StrongFitApp.Controllers
{
    [Authorize]
    public class ExerciciosController : Controller
    {
        private readonly StrongFitContext _context;

        public ExerciciosController(StrongFitContext context)
        {
            _context = context;
        }

        // GET: Exercicios
        public async Task<IActionResult> Index()
        {
            var exercicios = await _context.Exercicios
                .Include(e => e.Treino)
                .ToListAsync();
            return View(exercicios);
        }

        // GET: Exercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios
                .Include(e => e.Treino)
                .FirstOrDefaultAsync(m => m.ExercicioID == id);
            if (exercicio == null)
            {
                return NotFound();
            }

            return View(exercicio);
        }

        // GET: Exercicios/Create
        public IActionResult Create(int? treinoId)
        {
            ViewData["TreinoID"] = new SelectList(_context.Treinos, "TreinoID", "TreinoID");

            if (treinoId.HasValue)
            {
                var exercicio = new Exercicio { TreinoID = treinoId.Value };
                return View(exercicio);
            }

            return View();
        }

        // POST: Exercicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExercicioID,Nome,Descricao,Categoria,TreinoID")] Exercicio exercicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercicio);
                await _context.SaveChangesAsync();

                if (exercicio.TreinoID.HasValue)
                {
                    return RedirectToAction("Details", "Treinos", new { id = exercicio.TreinoID.Value });
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["TreinoID"] = new SelectList(_context.Treinos, "TreinoID", "TreinoID", exercicio.TreinoID);
            return View(exercicio);
        }

        // GET: Exercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios.FindAsync(id);
            if (exercicio == null)
            {
                return NotFound();
            }

            ViewData["TreinoID"] = new SelectList(_context.Treinos, "TreinoID", "TreinoID", exercicio.TreinoID);
            return View(exercicio);
        }

        // POST: Exercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExercicioID,Nome,Descricao,Categoria,TreinoID")] Exercicio exercicio)
        {
            if (id != exercicio.ExercicioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exercicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExercicioExists(exercicio.ExercicioID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (exercicio.TreinoID.HasValue)
                {
                    return RedirectToAction("Details", "Treinos", new { id = exercicio.TreinoID.Value });
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["TreinoID"] = new SelectList(_context.Treinos, "TreinoID", "TreinoID", exercicio.TreinoID);
            return View(exercicio);
        }

        // GET: Exercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios
                .Include(e => e.Treino)
                .FirstOrDefaultAsync(m => m.ExercicioID == id);
            if (exercicio == null)
            {
                return NotFound();
            }

            return View(exercicio);
        }

        // POST: Exercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercicio = await _context.Exercicios.FindAsync(id);
            var treinoId = exercicio.TreinoID;

            _context.Exercicios.Remove(exercicio);
            await _context.SaveChangesAsync();

            if (treinoId.HasValue)
            {
                return RedirectToAction("Details", "Treinos", new { id = treinoId.Value });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ExercicioExists(int id)
        {
            return _context.Exercicios.Any(e => e.ExercicioID == id);
        }
    }
}