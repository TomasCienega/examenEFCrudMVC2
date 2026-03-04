using examenEFCrudMVC2.Context;
using examenEFCrudMVC2.Models;
using examenEFCrudMVC2.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace examenEFCrudMVC2.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly examenEFCrudMVC2Context _context;
        public EmpleadoController(examenEFCrudMVC2Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int idDep)
        {
            var vm = new EmpleadoVM();
            ViewBag.IdSeleccionado = idDep;
            try
            {
                vm.ListaDepartamentos = await _context.Departamentos.ToListAsync();
                if (idDep>0)
                {
                    vm.ListaEmpleados = await _context.Empleados.
                        FromSqlRaw("exec sp_ListarEmplPorIdDep {0}",idDep).
                        ToListAsync();
                }
                else
                {
                    vm.ListaEmpleados = await _context.Empleados.
                        Include(tD => tD.IdDepartamentoNavigation).
                        OrderByDescending(a => a.Activo).
                        ToListAsync();
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                vm.ListaEmpleados = new List<Empleado>();
                vm.ListaDepartamentos = new List<Departamento>();
                Console.WriteLine(ex.Message);
                return View(vm);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Guardar(EmpleadoVM vm)
        {
            try
            {
                await _context.Empleados.AddAsync(vm.EmpleadoModelReference);
                await _context.SaveChangesAsync();

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int idEmp)
        {
            var vm = new EmpleadoVM();
            try
            {
                vm.ListaDepartamentos = await _context.Departamentos.ToListAsync();
                var empleado = await _context.Empleados.FindAsync(idEmp);
                if (empleado==null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    vm.EmpleadoModelReference = empleado;
                    return View(vm);
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(EmpleadoVM vm)
        {
            try
            {
                _context.Empleados.Update(vm.EmpleadoModelReference);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idEmp)
        {
            try
            {
                var empleado = await _context.Empleados.FindAsync(idEmp);
                if (empleado==null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _context.Empleados.Remove(empleado);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Estado(int idEmp, int idDep)
        {
            try
            {
                await _context.Database.
                    ExecuteSqlRawAsync("exec sp_EstadoEmpleado {0}",idEmp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index", new {idDep = idDep });
        }
    }
}
