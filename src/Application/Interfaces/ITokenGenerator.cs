using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenGenerator
{
    string Generate(User user);
}
