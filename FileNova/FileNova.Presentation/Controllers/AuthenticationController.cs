using FileNova.Presentation.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNova.Presentation.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthenticationController(IServiceManager service) => _service = service;

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var result = await _service.AuthenticationService.RegisterUser(userForRegistration);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            if (userForAuthentication == null)
                return BadRequest("Datos de login no proporcionados");

            if (!await _service.AuthenticationService.ValidateUser(userForAuthentication))
                return Unauthorized("Usuario o contraseña incorrectos");

            //var user = _service.AuthenticationService.GetCurrentUser();

            //if (user.MustChangePassword)
            //{
            //    return Ok(new
            //    {
            //        mustChangePassword = true,
            //        userId = user.Id
            //    });
            //}

            var tokenDto = await _service.AuthenticationService.CreateToken(populateExpiry: true);

            // Devuelve un JSON con los tokens
            return Ok(new
            {
                accessToken = tokenDto.AccessToken,
                refreshToken = tokenDto.RefreshToken
            });
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var result = await _service.AuthenticationService.ChangePassword(dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Contraseña actualizada correctamente" });
        }


    }
}
