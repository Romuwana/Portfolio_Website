using System.ComponentModel.DataAnnotations;

namespace PortfolioAPI.Models
{
    public class Certification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Issuer { get; set; }
        public DateTime DateIssued { get; set; }

        public string? ImageUrl { get; set; }
    }
}