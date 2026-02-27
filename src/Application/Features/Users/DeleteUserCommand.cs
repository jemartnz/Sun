using Domain.Commons;
using MediatR;

namespace Application.Features.Users;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;
