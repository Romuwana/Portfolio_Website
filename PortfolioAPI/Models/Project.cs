using System;
using System.Collections.Generic; // Required for List

namespace PortfolioAPI.Models // Ensure this matches your actual namespace
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Restored original properties!
        public List<string> TechStack { get; set; } = new List<string>();
        public List<string> ImageUrls { get; set; } = new List<string>();

        // The master toggle: "Data Science" or "Software Development"
        public string Category { get; set; } = string.Empty;

        // ==========================================
        // AUDIT TRAIL (TIMESTAMPS)
        // ==========================================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ==========================================
        // SHARED FIELDS
        // ==========================================
        public string? GithubUrl { get; set; }

        // ==========================================
        // DATA SCIENCE FIELDS
        // ==========================================
        public string? DatasetUrl { get; set; }
        public string? PresentationUrl { get; set; }

        // ==========================================
        // SOFTWARE DEVELOPMENT FIELDS
        // ==========================================
        public bool IsPublished { get; set; }
        public string? LiveWebsiteUrl { get; set; }
    }
}