using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.DTOs;

namespace MyApi.Controllers;

// 定義 API 的網址路由，[controller] 會自動替換成類別名稱的前綴 (UserTasks)
// 所以這個 Controller 的基礎網址會是：api/UserTasks
[Route("api/[controller]")]
[ApiController]
public class UserTasksController : ControllerBase
{
    private readonly AppDbContext _context;

    // 建構子：透過「依賴注入 (DI)」取得我們在 Program.cs 註冊好的資料庫上下文
    public UserTasksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/UserTasks (查詢資料庫裡所有的任務)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserTask>>> GetUserTasks([FromQuery] string? keyword)
    {
        var query = _context.UserTasks.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t => t.Title.Contains(keyword) || (t.Content != null && t.Content.Contains(keyword)));
        }
        // ToListAsync() 會將 SQL 查詢結果轉換成 C# 的 List
        // return await _context.UserTasks.ToListAsync();
        var tasks = await query
            .OrderByDescending(t => t.TargetDate) 
            .ToListAsync();
        return tasks;
    }

    // POST: api/UserTasks (新增一筆任務到資料庫)
    [HttpPost]
    public async Task<ActionResult<UserTask>> PostUserTask(UserTask userTask)
    {
        if (userTask.TargetDate.HasValue)
        {
            // 將前端傳來的本地時間，自動扣除時差轉換為 UTC (例如台灣會自動 -8 小時) 
            // 這樣存進 PostgreSQL 就不會報錯了
            userTask.TargetDate = userTask.TargetDate.Value.ToUniversalTime();
        }
    
    // 確保建立時間也絕對是 UTC
    userTask.CreatedAt = DateTime.UtcNow;
        // 告訴 EF Core 我們要新增這筆資料
        _context.UserTasks.Add(userTask);
        
        // 實際執行 SQL 的 INSERT 語法，把資料寫入 PostgreSQL
        await _context.SaveChangesAsync();

        // 回傳 HTTP 201 (Created) 狀態碼，並回傳剛存入資料庫的完整物件
        return CreatedAtAction(nameof(GetUserTasks), new { id = userTask.Id }, userTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMemo(int id, UpdateMemoDTO updateDto)
    {
        var task = await _context.UserTasks.FindAsync(id);
        if (task == null) return NotFound();

        // 覆蓋文字內容
        task.Title = updateDto.Title;
        task.Content = updateDto.Content;
        task.ImageUrl = updateDto.ImageUrl;
        
        // 處理時間 (轉為 UTC)
        if (updateDto.TargetDate.HasValue)
            task.TargetDate = updateDto.TargetDate.Value.ToUniversalTime();
        else
            task.TargetDate = null;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPut("{id}/status")]
    public async Task<IActionResult> PutUserTask(int id, UpdateTaskStatusDTO updateDto)
    {
        // 1. 先從資料庫把這筆資料「完整的」撈出來
        var task = await _context.UserTasks.FindAsync(id);

        if (task == null)
        {
            return NotFound(); // 找不到就回傳 404
        }

        // 2. 「只」覆蓋我們要修改的欄位
        task.IsCompleted = updateDto.IsCompleted;
        // 注意：我們完全沒有動到 task.Title, task.Content 或 task.TargetDate
        // 它們會維持資料庫裡原本的值！

        // 3. 儲存變更
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/UserTasks/1 (刪除特定 ID 的任務)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserTask(int id)
    {
        // 1. 先從資料庫找出這筆資料
        var userTask = await _context.UserTasks.FindAsync(id);
        if (userTask == null)
        {
            return NotFound(); // 如果找不到，回傳 HTTP 404
        }

        // 2. 告訴 EF Core 要刪除這筆資料
        _context.UserTasks.Remove(userTask);
        
        // 3. 實際執行 SQL 的 DELETE 語法
        await _context.SaveChangesAsync();

        return NoContent(); // 回傳 HTTP 204，代表刪除成功
    }
}