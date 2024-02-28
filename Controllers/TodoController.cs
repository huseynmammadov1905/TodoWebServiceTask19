﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoWebService.Models.DTOs.Pagination;
using TodoWebService.Models.DTOs.Todo;
using TodoWebService.Providers;
using TodoWebService.Services.Todo;

namespace TodoWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly IRequestUserProvider _provider;
        public TodoController(ITodoService todoService, IRequestUserProvider provider)
        {
            _todoService = todoService;
            _provider = provider;
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<TodoItemDto>> Get(int id)
        {
            UserInfo? userInfo = _provider.GetUserInfo();
            var item = await _todoService.GetTodoItem(id);





            return item is not null
                ? item
                : NotFound();
        }

        [HttpGet("all")]
        public async Task<PaginatedListDto<TodoItemDto>?> All(PaginationRequest request, bool? isCompleted)
        {

            var result = await _todoService.GetAll(request.Page, request.PageSize, isCompleted);
            return result is not null ? result : null;
        }

        [HttpPost("create")]
        public async Task<ActionResult<TodoItemDto>> Create(CreateTodoItemRequest request)
        {
            var userInfo = _provider.GetUserInfo();
            var result = await _todoService.CreateTodo(userInfo!.Id, request);
            return result is not null ? result : BadRequest("Something went wrong");
        }

        [HttpPost("change/{id}")]
        public async Task<ActionResult<TodoItemDto>> ChangeTodo(ChangeTodoItemRequest request, int id)
        {
            var userInfo = _provider.GetUserInfo();
            var result = await _todoService.ChangeTodoItemStatus(id, request.IsCompleted);
            return result is not null ? result : BadRequest("Something went wrong");
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> DeleteTodo(int id)
        {

            var result = await _todoService.DeleteTodo(id);
            if (!result)
                return BadRequest();
            return Ok();
        }
    }
}
