using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Features.Tasks.Commands;
using Application.Features.Tasks.Queries;
using Application.Models.Exceptions;
using Application.Models.Requests;
using Application.Models.Responses;
using Application.Models.Wrappers;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace webapi.Controllers
{
    [Route("[controller]")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly IMediator _mediator;
        private readonly IRoleFeatureService _roleFeatureService;
        private readonly IJwtService _jwtService;

        public TaskController(ILogger<TaskController> logger, IMediator mediator, IRoleFeatureService roleFeatureService, IJwtService jwtService)
        {
            _logger = logger;
            _mediator = mediator;
            _roleFeatureService = roleFeatureService;
            _jwtService = jwtService;
        }


        /// <summary>
        ///     Get all tasks
        /// </summary>
        /// <remarks>
        /// 
        ///     Require ADMIN or SUPERVISOR role
        /// 
        /// </remarks>
        /// <returns> The list of all tasks </returns>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResponseWrapper<PaginationWrapper<List<TaskResponse>>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10){
            if(!await UserHasRole("ADMIN") && !await UserHasRole("SUPERVISOR")) 
                return Forbid();

            var query = new GetAllTasksQuery{PageNumber = pageNumber, PageSize = pageSize};
            var tasks = await _mediator.Send(query);
            return Ok(tasks);
        }

        /// <summary>
        ///     Get tasks by user id
        /// </summary>
        /// <remarks>
        /// 
        ///     Requires authorization from 2 ways:
        ///     - ADMIN or SUPERVISOR role
        ///     - The requestor user from the token are the same of the user assigned id
        /// 
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns> The task assigned to the user selected from the user id </returns>
        [Authorize]
        [HttpGet]
        [Route("[action]/{userId}")]
        [ProducesResponseType(typeof(ResponseWrapper<List<TaskResponse>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTasksByUser([FromRoute] int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) {
            bool checkSameUser = false;
            if(!await UserHasRole("ADMIN") && !await UserHasRole("SUPERVISOR")) checkSameUser = true;

            var fromTokenEmail = 
                _jwtService.GetEmailFromToken(
                    Request.Headers["Authorization"].ToString()["Bearer ".Length..].Trim()
                );

            var query = new GetTasksByUserQuery{
                PageNumber = pageNumber,
                PageSize = pageSize,
                CheckSameUser = checkSameUser,
                UserId = userId,
                FromTokenEmail = fromTokenEmail
            };

            var tasks = await _mediator.Send(query);

            return Ok(tasks);
        }



        /// <summary>
        ///     Create a new task
        /// </summary>
        /// <remarks>
        ///     Require ADMIN role
        ///     
        ///     Require all fields
        /// </remarks>
        /// <param name="command"></param>
        /// <returns> The id of the generated task </returns>
        [Authorize]
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResponseWrapper<int>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command){
            if(!await UserHasRole("ADMIN")) return Forbid();

            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        ///     Update task name or assignation
        /// </summary>
        /// <remarks>
        /// 
        ///     Require ADMIN or SUPERVISOR role
        /// 
        /// </remarks>
        /// <param name="idTask"></param>
        /// <param name="command"></param>
        /// <returns> The modified data of the task </returns>
        [Authorize]
        [HttpPut]
        [Route("[action]/{idTask}")]
        [ProducesResponseType(typeof(ResponseWrapper<TaskResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateTaskNameOrAssignation([FromRoute] int idTask, [FromBody] UpdateTaskNameOrAssignationCommand command){
            if(idTask != command.TaskId) throw new ApiException("Id on route and body are different");
            if(!await UserHasRole("ADMIN") && !await UserHasRole("SUPERVISOR")) return Forbid();

            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        ///     Update the state of taks
        /// </summary>
        /// <remarks>
        /// 
        ///     Requires authorization from 2 ways:
        ///     - ADMIN or SUPERVISOR role
        ///     - The requestor user from the token are the same of the user assigned to task
        /// 
        /// </remarks>
        /// <param name="idTask"></param>
        /// <param name="body"></param>
        /// <returns> The modified data of the task </returns>
        [Authorize]
        [HttpPut]
        [Route("[action]/{idTask}")]
        [ProducesResponseType(typeof(ResponseWrapper<TaskResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateTaskState([FromRoute] int idTask, [FromBody] UpdateTaskStateRequest body){
            if(idTask != body.TaskId) throw new ApiException("Id on route and body are different");
            bool checkSameUser = false;
            if(!await UserHasRole("ADMIN") && !await UserHasRole("SUPERVISOR")) checkSameUser = true;

            var fromTokenEmail = 
                _jwtService.GetEmailFromToken(
                    Request.Headers["Authorization"].ToString()["Bearer ".Length..].Trim()
                );

            var command = new UpdateTaskStateCommand {
                TaskId = idTask,
                StateId = body.StateId,
                CheckSameUser = checkSameUser,
                FromTokenEmail = fromTokenEmail
            };


            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        ///     Delete task by id
        /// </summary>
        /// <remarks>
        ///     Require ADMIN role
        /// </remarks>
        /// <param name="taskId"></param>
        /// <returns> the id of the deleted task </returns>
        [Authorize]
        [HttpDelete]
        [Route("[action]/{taskId}")]
        [ProducesResponseType(typeof(ResponseWrapper<int>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteTask([FromRoute] int taskId)
        {
            if(!await UserHasRole("ADMIN")) return Forbid();

            var command = new DeleteTaskCommand { TaskId = taskId };
            var result = await _mediator.Send(command);
            
            return Ok(result);
        }



        private async Task<bool> UserHasRole(string role){
            return await _roleFeatureService.UserHasValidRoleFromToken(
                Request.Headers["Authorization"].ToString(),
                role
            );
        }

    }
}