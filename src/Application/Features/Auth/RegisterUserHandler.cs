using Application.DTOs;
using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Auth;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        // 1. Crear Value Object Email (se valida solo)
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result<AuthResponse>.Failure(emailResult.Error);

        // 2. Crear Value Object Password (se valida solo)
        var passwordResult = Password.Create(request.Password);
        if (passwordResult.IsFailure)
            return Result<AuthResponse>.Failure(passwordResult.Error);

        // 3. Verificar que no exista otro usuario con ese email
        var existingUser = await _userRepository.GetByEmailAsync(emailResult.Value, ct);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure(UserErrors.EmailAlreadyExists);

        // 4. Hashear password
        var hash = _passwordHasher.Hash(passwordResult.Value.Value);

        // 5. Crear entidad User
        var userResult = User.Create(request.FirstName, request.LastName, emailResult.Value, hash);
        if (userResult.IsFailure)
            return Result<AuthResponse>.Failure(userResult.Error);

        // 6. Persistir
        await _userRepository.AddAsync(userResult.Value, ct);
        await _userRepository.SaveChangesAsync(ct);

        // 7. Generar token
        var token = _tokenGenerator.Generate(userResult.Value);

        return Result<AuthResponse>.Success(
            new AuthResponse(token, userResult.Value.Id, emailResult.Value.Value));
    }
}
