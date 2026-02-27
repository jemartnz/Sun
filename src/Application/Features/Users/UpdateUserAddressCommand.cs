using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Users;

public sealed record UpdateUserAddressCommand(
    Guid UserId,
    string Street,
    string City,
    string Country,
    string? ZipCode) : IRequest<Result<UserResponse>>;
