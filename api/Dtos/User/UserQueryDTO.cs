﻿namespace api.Dtos.User
{
    public class UserQueryDTO
    {
        public string? Name { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
