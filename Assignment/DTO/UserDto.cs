using System;

namespace Assignment.DTO
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public decimal UserId { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
