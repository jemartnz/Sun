using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Users;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserResponse>>;
