using System.ComponentModel.DataAnnotations;

namespace PortfolioAPI.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Level { get; set; } // 0 to 100 for the progress bar
    }
}