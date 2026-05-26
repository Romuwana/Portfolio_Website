using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IConfiguration configuration, ILogger<UploadController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("health")]
    [HttpHead("health")]
    public IActionResult KeepAlive()
    {
        return Ok(new { status = "Awake" });
    }

    [HttpPost]
    public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0) return BadRequest("No files uploaded.");

        try
        {
            // 1. Connect to Cloudinary using your secure environment variables
            var account = new Account(
                _configuration["CLOUDINARY_CLOUD_NAME"],
                _configuration["CLOUDINARY_API_KEY"],
                _configuration["CLOUDINARY_API_SECRET"]
            );
            
            var cloudinary = new Cloudinary(account);
            var savedUrls = new List<string>();

            // 2. Loop through and upload each file
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream),
                            Folder = "portfolio_projects", // Creates a neat folder in your Cloudinary account
                            Transformation = new Transformation().Quality("auto").FetchFormat("auto") // Automatically optimizes the image size!
                        };

                        var uploadResult = await cloudinary.UploadAsync(uploadParams);

                        // 3. Grab the permanent URL from Cloudinary
                        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            savedUrls.Add(uploadResult.SecureUrl.ToString());
                        }
                        else
                        {
                            _logger.LogError($"Cloudinary upload failed: {uploadResult.Error?.Message}");
                            return StatusCode(500, "Failed to upload to cloud storage.");
                        }
                    }
                }
            }

            // 4. Send the permanent Cloudinary URLs back to Angular
            return Ok(savedUrls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CRITICAL ERROR UPLOADING TO CLOUDINARY!");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
