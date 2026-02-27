using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Users;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<Result<UserResponse>>;
