using Microsoft.EntityFrameworkCore;
using Planificador.Application.Interfaces;
using Planificador.Domain.Entities;
using Planificador.Infrastructure.Persistence;

namespace Planificador.Infrastructure.Services;

public class PlanningService : IPlanningService
{
    private readonly ApplicationDbContext _context;

    public PlanningService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPlanForYearAsync(string userId, int year)
    {
        return await _context.AnnualPlans
            .AnyAsync(p => p.UserId == userId && p.Year == year);
    }

    public async Task<AnnualPlan?> GetPlanAsync(string userId, int year)
    {
        return await _context.AnnualPlans
            .Include(p => p.Roles) // Cargamos los roles también
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Year == year);
    }

    public async Task<AnnualPlan> InitiatePlanAsync(string userId, int year)
    {
        // 1. Verificar si ya existe (por seguridad)
        var existing = await GetPlanAsync(userId, year);
        if (existing != null) return existing;

        // 2. Crear nuevo
        var plan = new AnnualPlan(year, userId);
        
        _context.AnnualPlans.Add(plan);
        await _context.SaveChangesAsync();
        
        return plan;
    }

    public async Task UpdateYearReviewAsync(Guid planId, string review)
    {
        var plan = await _context.AnnualPlans.FindAsync(planId);
        if (plan != null)
        {
            plan.YearReviewSummary = review;
            plan.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateIdentityAsync(Guid planId, string mission, string vision)
    {
        var plan = await _context.AnnualPlans.FindAsync(planId);
        if (plan != null)
        {
            plan.UpdateIdentity(mission, vision); // Usamos el método del Dominio
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddRoleAsync(Guid planId, string name, string color, string icon)
{
    var plan = await _context.AnnualPlans
        .Include(p => p.Roles)
        .FirstOrDefaultAsync(p => p.Id == planId);

    if (plan != null)
    {
        // Usamos el método del dominio que tiene la validación de 7 roles
        plan.AddRole(name, color, icon, null);
        await _context.SaveChangesAsync();
    }
}

public async Task DeleteRoleAsync(Guid roleId)
{
    var role = await _context.Roles.FindAsync(roleId);
    if (role != null)
    {
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
    }
}

    
}