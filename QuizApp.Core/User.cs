﻿public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; }
    public string Password { get; set; }
    public int Score { get; set; } = 0;
}
