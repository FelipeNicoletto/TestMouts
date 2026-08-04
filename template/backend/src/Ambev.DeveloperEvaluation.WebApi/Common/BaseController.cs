using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullReferenceException());

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? throw new NullReferenceException();

    protected IActionResult Ok(string? message = null) =>
        base.Ok(new ApiResponse { Success = true, Message = message ?? string.Empty });

    protected IActionResult Ok<T>(T data, string? message = null) =>
        base.Ok(new ApiResponseWithData<T> { Data = data, Success = true, Message = message ?? string.Empty });

    protected IActionResult Created<T>(string? uri, T data, string? message = null) =>
        base.Created(uri, new ApiResponseWithData<T> { Data = data, Success = true, Message = message ?? string.Empty });

    protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult BadRequest(string message) =>
        base.BadRequest(new ApiResponse { Message = message, Success = false });

    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(new ApiResponse { Message = message, Success = false });

    protected IActionResult OkPaginated<T>(PaginatedList<T> pagedList) =>
        base.Ok(new PaginatedResponse<T>
        {
            Data = pagedList,
            CurrentPage = pagedList.CurrentPage,
            TotalPages = pagedList.TotalPages,
            TotalCount = pagedList.TotalCount,
            Success = true
        });
}
