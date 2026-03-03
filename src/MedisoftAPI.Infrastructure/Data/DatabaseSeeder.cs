using MedisoftAPI.Application.UseCases;
using MedisoftAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedisoftAPI.Infrastructure.Data;

public static class DatabaseSeeder
{
    /// <summary>
    /// Crea la base de datos SQLite automáticamente si no existe (sin migraciones manuales)
    /// e inserta el usuario Admin/123456 si la tabla está vacía.
    /// </summary>
    public static async Task SeedAsync(AppDbContext context)
    {
        // EnsureCreatedAsync crea la BD y las tablas directamente desde el modelo
        // No requiere ejecutar Add-Migration ni Update-Database manualmente.
        await context.Database.EnsureCreatedAsync();

        if (!await context.Users.AnyAsync(u => u.Username == "Admin"))
        {
            context.Users.Add(new AppUser
            {
                Username     = "Admin",
                PasswordHash = AuthService.HashPassword("123456"),
                Role         = "Admin",
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
}
