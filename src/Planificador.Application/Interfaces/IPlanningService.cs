using Planificador.Domain.Entities;

namespace Planificador.Application.Interfaces;

public interface IPlanningService
{
    // Verifica si el usuario ya tiene plan para ese año
    Task<bool> HasPlanForYearAsync(string userId, int year);
    
    // Obtiene el plan (si existe)
    Task<AnnualPlan?> GetPlanAsync(string userId, int year);

    // Crea el esqueleto del plan (para cuando arranca el wizard)
    Task<AnnualPlan> InitiatePlanAsync(string userId, int year);
    
    // Guardar los pasos del Wizard
    Task UpdateYearReviewAsync(Guid planId, string review);
    Task UpdateIdentityAsync(Guid planId, string mission, string vision);
    Task AddRoleAsync(Guid planId, string name, string color, string icon);
    Task DeleteRoleAsync(Guid roleId);
}