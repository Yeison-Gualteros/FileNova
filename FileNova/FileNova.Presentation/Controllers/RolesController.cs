using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNova.Presentation.Controllers
{
    [Route("api/roles")]
    
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // post /api/roles
        [HttpPost]
        [Authorize("administrador")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            // Validar que el nombre del rol no esté vacío
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest(new { message = "El nombre del rol no puede estar vacío." });
            }

            // Verificar si el rol ya existe
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return Conflict(new { message = $"El rol '{roleName}' ya existe." });
            }

            // Crear el nuevo rol
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            // Verificar si la creación fue exitosa
            if (result.Succeeded)
            {
                return StatusCode(201, new { message = $"Rol '{roleName}' creado exitosamente." });  // 201 Created
            }

            // Si ocurre un error, devolver los detalles del error
            var errors = result.Errors.Select(error => new { error.Code, error.Description }).ToList();
            return BadRequest(new { message = "No se pudo crear el rol.", errors });
        }

    }
}
