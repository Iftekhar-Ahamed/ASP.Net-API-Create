using Assignment.DTO;
using Assignment.Helper;
using System;
using System.Threading.Tasks;

namespace Assignment.IRepository
{
    public interface IJwtToken
    {
        string GenerateToken(UserDto user);
    }
}
