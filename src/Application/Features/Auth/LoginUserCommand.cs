using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Auth;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;
