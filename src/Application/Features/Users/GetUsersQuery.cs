using Application.Common;
using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Users;

public sealed record GetUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortOrder = null) : IRequest<Result<PagedResponse<UserResponse>>>;
