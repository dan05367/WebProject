using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    // 注入 IWebHostEnvironment 來取得 wwwroot 的實體路徑
    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(List<IFormFile> files)
    {
        if (files == null || files.Count == 0) return BadRequest("沒有檔案");

        // 1. 確保 wwwroot/uploads 資料夾存在
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
        var imageUrls = new List<string>();
        // 2. 產生隨機檔名 (避免使用者上傳同名檔案互相覆蓋)
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                imageUrls.Add($"/uploads/{uniqueFileName}");
            }
        }

        // 4. 回傳圖片的網址給前端
        return Ok(new { imageUrls = imageUrls });
    }
}