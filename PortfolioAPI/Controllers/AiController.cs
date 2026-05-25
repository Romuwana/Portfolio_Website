using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using PortfolioAPI.Data; // Ensure this matches your namespace for ApplicationDbContext

namespace PortfolioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private IConfiguration _configuration;

        // 1. Declare the database context variable
        private readonly ApplicationDbContext _dbContext;

        // 2. The Constructor: ASP.NET injects the live database connection here
        public AiController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return BadRequest(new { reply = "I didn't quite catch that. Could you repeat your question?" });
            }

            // PASTE YOUR ACTUAL GROQ API KEY HERE
            string apiKey = _configuration["OpenAI:ApiKey"];

            try
            {
                // ====================================================================
                // 3. FETCH LIVE DATA FROM POSTGRESQL
                // ====================================================================

                var liveProjects = _dbContext.Projects.ToList();
                var liveCerts = _dbContext.Certifications.ToList();
                var liveSkills = _dbContext.Skills.ToList();

                // Format the lists into readable strings for the AI
                // (Note: If your C# models use different property names like 'Title' instead of 'Name', update them here!)
                string projectsText = string.Join(" | ", liveProjects.Select(p => p.Title + ": " + p.Description));
                string certsText = string.Join(" | ", liveCerts.Select(c => c.Name + " (Issued by: " + c.Issuer + ")"));
                string skillsText = string.Join(", ", liveSkills.Select(s => s.Name));

                // ====================================================================
                // 4. BUILD THE SYSTEM PROMPT
                // ====================================================================

                var groqPayload = new
                {
                    model = "llama-3.1-8b-instant", // The updated, fully supported model
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = $@"You are a helpful, professional AI assistant built into the portfolio of R. Nare, a Computer Science and Informatics student at the University of the Free State (UFS). R. Nare is a full-stack developer based in South Africa who builds digital solutions for regional businesses.

                            CRITICAL INSTRUCTION: Here is R. Nare's actual, live data straight from the database. When asked about projects, portfolio items, skills, or certifications, you MUST ONLY mention the items listed below. DO NOT invent, guess, or hallucinate any other data.

                            LIVE PROJECTS: {projectsText}
                            LIVE CERTIFICATIONS: {certsText}
                            LIVE SKILLS: {skillsText}

                            Answer questions concisely and intelligently. Do not break character."
                        },
                        new { role = "user", content = request.Message }
                    }
                };

                // ====================================================================
                // 5. FIRE THE REQUEST TO GROQ
                // ====================================================================

                var content = new StringContent(JsonSerializer.Serialize(groqPayload), Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
                {
                    Content = content
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.SendAsync(httpRequest);

                // If Groq rejects the request (like a bad key or wrong model), send the error to the chat UI
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    return Ok(new { reply = $"[CONNECTION FAILED]: Groq responded with {response.StatusCode}. Details: {errorDetail}" });
                }

                // Parse the successful response
                var jsonString = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(jsonString);

                string replyText = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "I received an empty thought process.";

                return Ok(new { reply = replyText });
            }
            catch (Exception ex)
            {
                // If C# crashes before or during the database/API calls, print it in the UI
                return Ok(new { reply = $"[C# SYSTEM CRASH]: {ex.Message}" });
            }
        }
    }

    public class ChatRequest
    {
        public string? Message { get; set; }
    }
}