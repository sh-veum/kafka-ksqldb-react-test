using KafkaAuction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KafkaAuction.Data;

public class MainDbContext : IdentityDbContext<UserModel, IdentityRole, string>
{
    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {
    }
}