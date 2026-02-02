using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects.User;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileNova.Presentation.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserControllers : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IServiceManager _service;
        private readonly RoleManager<Role> _roleManager;

        public UserControllers(UserManager<User> userManager, IServiceManager service, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _service = service;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserParameters userParameters)
       {
            try
            {
                var result = await _service.UserService.GetAllUsers(userParameters, trackChanges: false);

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));

                return Ok(new
                {
                    data = result.users,
                    meta = result.metaData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    stack = ex.StackTrace
                });
            }
        }




        [HttpGet("{id}/permisos")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetPermisosDelUser(string id)
        {
            var permisos = await _service.permisosService.GetPermissionsByUser(id);
            if (permisos == null)
            {
                return NotFound(new { message = "No se encontraron permisos para este usuario" });
            }
            return Ok(permisos);
        }

        [HttpGet("{id}", Name ="GetUser")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _service.UserService.GetUserById(id, trackChanges: false);
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActualizarUser(string id, [FromBody] UserForUpdateDto dto)
        {
            var userActualizado = await _service.UserService.ActualizarUser(id, dto, true);
            return Ok(userActualizado);
        }



        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateUser([FromBody] UserForRegistrationDto dto)
        {
            if (dto == null)
                return BadRequest("El usuario no puede ser nulo");

            var user = await _service.UserService.CreateUser(dto, trackChanges: false);

            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }




        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _service.UserService.DeleteUser(id, trackChanges: true);
            return NoContent();
        }
    }
}
