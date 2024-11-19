namespace Application.SpecParams.ProductSpecification;

public class ProductLogSpecParams : BaseSpecParam
{
    public int? CourtCluster { get; set; }
    public int? LogType { get; set; }
    public string? fromDate { get; set; }
    public string? toDate { get; set; }
}