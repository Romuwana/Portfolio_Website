export interface Project {
    id?: number; 
    title: string;
    description: string;

    
    // Restored properties (Assuming they are arrays of strings)
    techStack: string[]; 
    imageUrls?: string[];

    // The master toggle
    category: string; 

    // ==========================================
    // AUDIT TRAIL (TIMESTAMPS)
    // ==========================================
    createdAt?: Date | string; 
    updatedAt?: Date | string;

    // ==========================================
    // SHARED FIELDS
    // ==========================================
    githubUrl?: string; 

    // ==========================================
    // DATA SCIENCE FIELDS
    // ==========================================
    datasetUrl?: string;
    presentationUrl?: string;

    // ==========================================
    // SOFTWARE DEVELOPMENT FIELDS
    // ==========================================
    isPublished?: boolean;
    liveWebsiteUrl?: string;
    status?: string;
}