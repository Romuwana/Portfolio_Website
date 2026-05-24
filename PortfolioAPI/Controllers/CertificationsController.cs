using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Data;
using PortfolioAPI.Models;

namespace PortfolioAPI.Controllers
{
    [Route("api/certs")]
    [ApiController]
    public class CertificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CertificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certification>>> GetCertifications()
        {
            return await _context.Certifications.OrderByDescending(c => c.DateIssued).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Certification>> CreateCertification(Certification certification)
        {
            certification.DateIssued = DateTime.SpecifyKind(certification.DateIssued, DateTimeKind.Utc);
            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCertifications), new { id = certification.Id }, certification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCertification(int id, Certification certification)
        {
            if (id != certification.Id) return BadRequest();
            certification.DateIssued = DateTime.SpecifyKind(certification.DateIssued, DateTimeKind.Utc);
            _context.Entry(certification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            var cert = await _context.Certifications.FindAsync(id);
            if (cert == null) return NotFound();
            _context.Certifications.Remove(cert);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}