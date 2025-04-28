﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StrongFitApp.Data;
using StrongFitApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace StrongFitApp.Controllers
{
    [Authorize(Roles = "Admin, Personal")]
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
            return View(await _context.Exercicios.ToListAsync());
        }

        // GET: Exercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Exercicios == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios
                .FirstOrDefaultAsync(m => m.ExercicioID == id);
            if (exercicio == null)
            {
                return NotFound();
            }

            return View(exercicio);
        }

        // GET: Exercicios/Create
        public IActionResult Create()
        {
            // Lista de categorias para o dropdown
            ViewBag.Categorias = new List<string>
            {
                "Peito",
                "Costas",
                "Pernas",
                "Ombros",
                "Braços",
                "Abdômen",
                "Cardio",
                "Funcional",
                "Alongamento"
            };

            return View();
        }

        // POST: Exercicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExercicioID,Nome,Descricao,Categoria,Series,Repeticoes")] Exercicio exercicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Lista de categorias para o dropdown em caso de erro
            ViewBag.Categorias = new List<string>
            {
                "Peito",
                "Costas",
                "Pernas",
                "Ombros",
                "Braços",
                "Abdômen",
                "Cardio",
                "Funcional",
                "Alongamento"
            };

            return View(exercicio);
        }

        // GET: Exercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Exercicios == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios.FindAsync(id);
            if (exercicio == null)
            {
                return NotFound();
            }

            // Lista de categorias para o dropdown
            ViewBag.Categorias = new List<string>
            {
                "Peito",
                "Costas",
                "Pernas",
                "Ombros",
                "Braços",
                "Abdômen",
                "Cardio",
                "Funcional",
                "Alongamento"
            };

            return View(exercicio);
        }

        // POST: Exercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExercicioID,Nome,Descricao,Categoria,Series,Repeticoes")] Exercicio exercicio)
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
                return RedirectToAction(nameof(Index));
            }

            // Lista de categorias para o dropdown em caso de erro
            ViewBag.Categorias = new List<string>
            {
                "Peito",
                "Costas",
                "Pernas",
                "Ombros",
                "Braços",
                "Abdômen",
                "Cardio",
                "Funcional",
                "Alongamento"
            };

            return View(exercicio);
        }

        // GET: Exercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Exercicios == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios
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
            if (_context.Exercicios == null)
            {
                return Problem("Entity set 'StrongFitContext.Exercicios'  is null.");
            }
            var exercicio = await _context.Exercicios.FindAsync(id);
            if (exercicio != null)
            {
                _context.Exercicios.Remove(exercicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExercicioExists(int id)
        {
            return (_context.Exercicios?.Any(e => e.ExercicioID == id)).GetValueOrDefault();
        }
    }
}
