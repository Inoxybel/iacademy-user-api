using CrossCutting.Enums;

namespace Domain.DTO;

public class UpdatePlanRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public PlanType Type { get; set; }
    public decimal Value { get; set; }
    public int AccessLimit { get; set; }
}
