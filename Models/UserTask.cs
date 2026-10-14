namespace MyApi.Models;

public class UserTask
{
    // Id 會自動被 EF Core 辨識為主鍵 (Primary Key)，而且會自動遞增
    public int Id { get; set; }
    
    // 任務標題
    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty; // 備忘錄詳細內容
    public DateTime? TargetDate { get; set; }           // 預定時間 (加問號代表可以不填)

    // 任務是否已完成
    public bool IsCompleted { get; set; }
    public string? ImageUrl { get; set; }
    // 任務建立時間，預設為當下時間
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}