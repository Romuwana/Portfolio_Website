using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0) return BadRequest("No files uploaded.");

        // FIX: Safely check for WebRootPath, fallback to ContentRootPath if wwwroot doesn't exist yet
        var basePath = string.IsNullOrWhiteSpace(_env.WebRootPath)
            ? Path.Combine(_env.ContentRootPath, "wwwroot")
            : _env.WebRootPath;

        var uploadPath = Path.Combine(basePath, "uploads");

        // Ensure the directories actually exist before saving!
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        var savedUrls = new List<string>();

        foreach (var file in files)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the URL path
            savedUrls.Add($"https://localhost:44391/uploads/{fileName}");
        }

        return Ok(savedUrls);
    }
}