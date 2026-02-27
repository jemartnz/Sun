using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users;

public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository) =>
        _userRepository = userRepository;

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        await _userRepository.RemoveAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        return Result.Success();
    }
}
