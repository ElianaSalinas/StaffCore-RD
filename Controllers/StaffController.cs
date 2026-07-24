using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffCore_RD.Data;
using StaffCore_RD.Models;

namespace StaffCore_RD.Controllers
{
    [Authorize] // Toda ruta protegida → redirige a /Account/Login si no hay sesión
    public class StaffController : Controller
    {
        private readonly StaffDbContext _context;

        public StaffController(StaffDbContext context)
        {
            _context = context;
        }

        // ── INDEX ─────────────────────────────────────────────────────────────
        // Roles con acceso: Administrador, RRHH, Viewer

        [Authorize(Roles = "Administrador,RRHH,Viewer")]
        public async Task<IActionResult> Index(string? buscar)
        {
            // Filtra por Activo = true y ordena por Nombre (siempre)
            var query = _context.Personal
                .Where(s => s.Activo);

            // Buscador en tiempo real (bonus HU-10): filtro parcial por nombre
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(s => s.Nombre.Contains(buscar));
            }

            var personal = await query
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            ViewData["BuscarActual"] = buscar;
            return View(personal);
        }

        // ── CREATE ────────────────────────────────────────────────────────────
        // Roles con acceso: Administrador, RRHH

        [Authorize(Roles = "Administrador,RRHH")]
        public IActionResult Create()
        {
            // Devuelve View con new Staff() para que Razor pre-enlace los campos vacíos
            return View(new Staff());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,RRHH")]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (!ModelState.IsValid)
            {
                // Si inválido: devuelve View con errores (no pierde datos)
                return View(staff);
            }

            _context.Add(staff);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Empleado creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── EDIT ──────────────────────────────────────────────────────────────
        // Roles con acceso: Administrador, RRHH

        [Authorize(Roles = "Administrador,RRHH")]
        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _context.Personal.FindAsync(id);
            if (staff == null)
                return NotFound();

            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,RRHH")]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            // Verificación de consistencia id de ruta vs id del modelo
            if (id != staff.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(staff);

            _context.Update(staff);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Cambios guardados correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        // Rol con acceso: Solo Administrador

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Personal.FindAsync(id);
            if (staff == null)
                return NotFound();

            // GET: solo muestra confirmación, NUNCA elimina (error fatal si se borra aquí)
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Personal.FindAsync(id);
            if (staff != null)
            {
                _context.Personal.Remove(staff);
                await _context.SaveChangesAsync();
            }

            TempData["Exito"] = "Empleado eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // ── DETAILS (bonus HU-11) ─────────────────────────────────────────────
        // Roles con acceso: Administrador, RRHH, Viewer

        [Authorize(Roles = "Administrador,RRHH,Viewer")]
        public async Task<IActionResult> Details(int id)
        {
            var staff = await _context.Personal.FindAsync(id);
            if (staff == null)
                return NotFound();

            return View(staff);
        }

        // ── RESUMEN (bonus HU-12) ─────────────────────────────────────────────
        // Roles con acceso: Administrador, RRHH

        [Authorize(Roles = "Administrador,RRHH")]
        public async Task<IActionResult> Resumen()
        {
            // Los 4 departamentos fijos del sistema (siempre aparecen aunque tengan 0 empleados)
            var departamentos = new[] { "Tecnología", "Recursos Humanos", "Finanzas", "Operaciones" };

            var datos = await _context.Personal
                .Where(s => s.Activo)
                .GroupBy(s => s.Departamento)
                .Select(g => new ResumenDepartamentoViewModel
                {
                    Departamento = g.Key,
                    TotalEmpleados = g.Count(),
                    TotalNomina = g.Sum(s => s.Salario)
                })
                .ToListAsync();

            // Asegura que los 4 departamentos siempre aparezcan (incluso con 0 empleados)
            var resumen = departamentos.Select(d =>
                datos.FirstOrDefault(x => x.Departamento == d)
                ?? new ResumenDepartamentoViewModel { Departamento = d, TotalEmpleados = 0, TotalNomina = 0 }
            ).ToList();

            return View(resumen);
        }
    }
}
