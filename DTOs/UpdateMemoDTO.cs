namespace MyApi.DTOs;

public class UpdateMemoDTO
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
    public string? ImageUrl { get; set; }
}