using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;

    // Inject ILogger so we can force errors to show up in Render!
    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0) return BadRequest("No files uploaded.");

        try
        {
            var basePath = string.IsNullOrWhiteSpace(_env.WebRootPath)
                ? Path.Combine(_env.ContentRootPath, "wwwroot")
                : _env.WebRootPath;

            var uploadPath = Path.Combine(basePath, "uploads");

            if (!Directory.Exists(uploadPath)) 
            {
                Directory.CreateDirectory(uploadPath);
            }

            var savedUrls = new List<string>();

            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // DYNAMIC URL FIX: This builds the URL using your live Render domain!
                var finalUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                savedUrls.Add(finalUrl);
            }

            return Ok(savedUrls);
        }
        catch (Exception ex)
        {
            // THIS WILL FORCE THE ERROR TO SHOW IN THE RENDER LOGS
            _logger.LogError(ex, "CRITICAL ERROR SAVING FILE TO RENDER!");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
