﻿namespace CardsServer.BLL.Dto
{
    public class CreateUser
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
