using WMS.Domain.Entities;

namespace WMS.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserLogin user);
}
