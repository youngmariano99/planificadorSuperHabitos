namespace Planificador.Domain.Entities;

public class AnnualPlan : BaseEntity
{
    public int Year { get; private set; }
    public string UserId { get; private set; } = string.Empty; 

    // Identidad
    public string Mission { get; private set; } = string.Empty;
    public string Vision { get; private set; } = string.Empty;

    // Encuesta previa
    public string? YearReviewSummary { get; set; }

    // Colección protegida
    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private AnnualPlan() { } // Constructor vacío para EF Core

    public AnnualPlan(int year, string userId)
    {
        Year = year;
        UserId = userId;
    }

    public void UpdateIdentity(string mission, string vision)
    {
        Mission = mission;
        Vision = vision;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // Regla de negocio: Máximo 7 roles
    public void AddRole(string name, string color, string icon, string? description)
    {
        if (_roles.Count >= 7)
        {
            // Usamos Exception normal para no complicarnos ahora
            throw new Exception("No puedes agregar más de 7 roles. Priorizar es clave.");
        }

        _roles.Add(new Role
        {
            Name = name,
            ColorHex = color,
            IconName = icon,
            Description = description,
            AnnualPlanId = this.Id
        });
    }
    
    public void UpdateRole(Guid roleId, string name, string color, string icon)
    {
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null) throw new Exception("El rol no existe.");
        
        role.Name = name;
        role.ColorHex = color;
        role.IconName = icon;
    }
}