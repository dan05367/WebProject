using Microsoft.EntityFrameworkCore;

namespace MyApi.Models;

public class AppDbContext : DbContext
{
    // 建構子：用來接收外部傳進來的資料庫設定 (例如我們在 appsettings.json 寫的連線字串)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet 代表資料庫裡的一張資料表。
    // 這行告訴 EF Core：「我要在資料庫裡面建一張名為 UserTasks 的表，裡面的欄位結構請參考 UserTask 類別。」
    public DbSet<UserTask> UserTasks { get; set; }
}