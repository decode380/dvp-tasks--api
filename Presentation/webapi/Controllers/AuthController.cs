using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Features.Auth.Queries;
using Application.Models.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace webapi.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(ILogger<AuthController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        ///     Login with a user
        /// </summary>
        /// <remarks>
        ///     Email and password are required
        /// </remarks>
        /// <returns>The authentication token</returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromBody] LoginQuery login){
            string token = await _mediator.Send(login);
            return Ok(token);
        }
    }
}