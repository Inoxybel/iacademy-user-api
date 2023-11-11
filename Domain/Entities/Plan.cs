using CrossCutting.Enums;

namespace Domain.Entities;

public class Plan
{
    public string Id { get; set; }
    public string Name { get; set; }
    public PlanType Type { get; set; }
    public decimal Value { get; set; }
    public int AccessLimit { get; set; }
}
