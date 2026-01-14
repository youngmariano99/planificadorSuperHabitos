namespace Planificador.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ColorHex { get; set; } = "#3b82f6"; // Un azul por defecto
    public string IconName { get; set; } = "fa-user";
    public string? ImageUrl { get; set; }

    // Relación
    public Guid AnnualPlanId { get; set; }
    public AnnualPlan AnnualPlan { get; set; } = null!;
}