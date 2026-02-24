using Application.DTOs;
using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Auth;

public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        // 1. Validar formato email
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);

        // 2. Buscar usuario
        var user = await _userRepository.GetByEmailAsync(emailResult.Value, ct);
        if (user is null)
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);

        // 3. Verificar password (contra el hash almacenado)
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);

        // 4. Generar token
        var token = _tokenGenerator.Generate(user);

        return Result<AuthResponse>.Success(
            new AuthResponse(token, user.Id, user.Email.Value));
    }
}
