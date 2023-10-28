using CrossCutting.Enums;

namespace Domain.DTO;

public class CreatePlanRequest
{
    public string Name { get; set; }
    public PlanType Type { get; set; }
    public decimal Value { get; set; }
    public int AccessLimit { get; set; }
}
