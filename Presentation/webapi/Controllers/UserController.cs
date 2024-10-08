using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Features.Users;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Models.Exceptions;
using Application.Models.Responses;
using Application.Models.Wrappers;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Webapi.Swagger;

namespace Webapi.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;
        private readonly IRoleFeatureService _roleFeatureService;
        private readonly IJwtService _jwtService;

        public UserController(ILogger<UserController> logger, IMediator mediator, IRoleFeatureService roleFeatureService, IJwtService jwtService)
        {
            _logger = logger;
            _mediator = mediator;
            _roleFeatureService = roleFeatureService;
            _jwtService = jwtService;
        }


        /// <summary>
        ///     Get all users
        /// </summary>
        /// <remarks>
        /// 
        ///     Require ADMIN role
        ///
        /// </remarks>
        /// <returns>List of all users</returns>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResponseWrapper<List<UserWithRoleResponse>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!await UserHasRole("ADMIN") && !await UserHasRole("SUPERVISOR")) return Forbid();
            var users = await _mediator.Send(new GetAllUsersQuery{
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            return Ok(users);
        }


        /// <summary>
        ///     Get user info by email
        /// </summary>
        /// <remarks>
        /// 
        ///     Require email on token are the same of the email query
        ///
        /// </remarks>
        /// <returns> The user data </returns>
        [Authorize]
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResponseWrapper<UserWithRoleResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserByEmail([FromBody] GetUserByEmailQuery query)
        {
            var fromTokenEmail = 
                _jwtService.GetEmailFromToken(
                    Request.Headers["Authorization"].ToString()["Bearer ".Length..].Trim()
                );
            if (query.Email != fromTokenEmail) return Forbid();
            var user = await _mediator.Send(query);

            return Ok(user);
        }


        /// <summary>
        ///     Create new user
        /// </summary>
        /// <remarks>
        ///     Require ADMIN role
        /// 
        ///     Email must be a valid email format
        ///     RoleId must be a valid role Id
        ///
        ///     All field are required
        /// </remarks>
        /// <returns>Return the id of the created user</returns>
        [Authorize]
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResponseWrapper<int>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand user) {
            if (!await UserHasRole("ADMIN")) return Forbid();
            var createdId = await _mediator.Send(user);
            return Ok(createdId);
        }


        /// <summary>
        ///     Update name or password of user
        /// </summary>
        /// <remarks>
        ///
        ///     Require ADMIN role
        /// 
        ///     Name and password must be provided
        /// 
        ///     Id on route and on body mus be provided and must be the same
        /// 
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="command"></param>
        /// <returns> The updated user id, name and email </returns>
        [Authorize]
        [HttpPut]
        [Route("[action]/{userId}")]
        [ProducesResponseType(typeof(ResponseWrapper<UserResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUser([FromRoute] int userId, [FromBody] UpdateUserCommand command){
            if (!await UserHasRole("ADMIN")) return Forbid();
            if (userId != command.Id) throw new ApiException("Id on body and route are different");

            var modifiedUser = await _mediator.Send(command);
            return Ok(modifiedUser);
        }


        /// <summary>
        ///     Delete user with the user id
        /// </summary>
        /// <remarks> 
        ///     
        ///     Require ADMIN role
        /// 
        ///     Require the user id on route
        /// 
        /// </remarks>
        /// <param name="userId"></param>
        /// <returns>The id of deleted user</returns>
        [Authorize]
        [HttpDelete]
        [Route("[action]/{userId}")]
        [ProducesResponseType(typeof(ResponseWrapper<int>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId){
            if(!await UserHasRole("ADMIN")) return Forbid();
            var command = new DeleteUserCommand{UserId=userId};
            var idToDeletedUser = await _mediator.Send(command);
            return Ok(idToDeletedUser);
        }


        private async Task<bool> UserHasRole(string role){
            return await _roleFeatureService.UserHasValidRoleFromToken(
                Request.Headers["Authorization"].ToString(),
                role
            );
        }
    }
}