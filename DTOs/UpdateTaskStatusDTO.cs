namespace MyApi.DTOs;

public class UpdateTaskStatusDTO
{
    // 裡面只放我們「允許」前端更新的欄位
    public bool IsCompleted { get; set; }
}