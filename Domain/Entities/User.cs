﻿namespace Domain.Entities;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string CompanyRef { get; set; }
}
