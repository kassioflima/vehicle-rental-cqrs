namespace vehicle_rental.api.Models;

public class ValidationErrorResponse
{
    public string Type { get; set; } = "ValidationError";
    public string Title { get; set; } = "Erro de validação";
    public int Status { get; set; } = 400;
    public string TraceId { get; set; } = string.Empty;
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
