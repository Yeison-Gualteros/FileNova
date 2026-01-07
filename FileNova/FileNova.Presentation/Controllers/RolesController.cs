using Contracts.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.RequestFeatures;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects.Roles;
using Microsoft.AspNetCore.Http.HttpResults;


namespace FileNova.Presentation.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceManager _service;
       

        public RolesController(RoleManager<IdentityRole> roleManager, IServiceManager service)
        {
            _roleManager = roleManager;
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAllRoles([FromQuery] RoleParameters roleParameters)
        {
            var roles = await _service.RoleService.GetAllRoles(roleParameters, trackChanges: false);

            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(roles.metaData);


            return Ok(new
            {
                roles = roles.roles,
                metaData = roles.metaData
            });

        }

        [HttpGet("{id}", Name ="GerRol")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _service.RoleService.GetRoleById(id, trackChanges: false);
            return Ok(role);
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> ActualizarRol(string id, [FromBody] RolForUpdateDto rolForUpdate)
        {
            if (rolForUpdate is null)
                return BadRequest("La actualización es nula");

            var rolAcrualizado = await _service.RoleService.ActualizarRol(id, rolForUpdate, trackChanges: true);

            return Ok(rolAcrualizado);
        }

        // post /api/roles
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateRole([FromBody] RolForCreationDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { message = "El nombre del rol no puede estar vacío." });
            }

            var roleExists = await _roleManager.RoleExistsAsync(dto.Name);
            if (roleExists)
            {
                return Conflict(new { message = $"El rol '{dto.Name}' ya existe." });
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(dto.Name));

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Error al crear el rol", errors });
            }

            return StatusCode(201, new { message = $"Rol '{dto.Name}' creado correctamente" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(string id)
        {
            var rol = await _service.RoleService.GetRoleById(id, trackChanges: true);
            if (rol == null)
                return NotFound();

            await _service.RoleService.DeleteRol(id, trackChanges: true);

            return Ok();

        }

    }
}
