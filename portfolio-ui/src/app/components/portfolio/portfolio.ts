import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ProjectService } from '../../services/project';
import { Project } from '../../models/project';
import { Skill } from '../../models/skill';
import { Certification } from '../../models/certification';

@Component({
  selector: 'app-portfolio',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './portfolio.html',
  styleUrl: './portfolio.css'
})
export class PortfolioComponent implements OnInit {
  // --- UI State ---
  isScrolled: boolean = false;
  menuOpen: boolean = false;
  
  // --- Dynamic Database Arrays ---
  projects: Project[] = [];
  skills: Skill[] = [];
  certifications: Certification[] = [];

  // ==========================================
  // AUTOMATIC UI FILTERS
  // ==========================================
  get technicalSkills() { return this.skills.filter(s => s.category === 'Technical Skills'); }
  get softSkills() { return this.skills.filter(s => s.category === 'Soft Skills'); }

  // NEW: Only show Data Science projects OR Software projects that are explicitly Published
  get visibleProjects() { 
    return this.projects.filter(p => 
      p.category === 'Data Science' || 
      p.status === 'Published' || 
      p.isPublished === true
    ); 
  }

  // --- AI Chat State ---
  isChatOpen: boolean = false;
  chatInput: string = '';
  isChatTyping: boolean = false;
  chatMessages: { role: string, content: string }[] = [
    { role: 'ai', content: "Hi! I'm the portfolio AI. I'm connected directly to the live PostgreSQL database. Ask me about Nare's skills, certs, ML models, or web projects!" }
  ];

  constructor(
    private router: Router,
    private http: HttpClient,
    private projectService: ProjectService
  ) {}

  ngOnInit(): void {
    // 1. Fetch Projects safely
    this.projectService.getProjects().subscribe({
      next: (data: Project[]) => this.projects = data,
      error: (err: any) => console.error('Failed to load projects', err)
    });

    // 2. Fetch Skills safely
    this.projectService.getSkills().subscribe({
      next: (data: Skill[]) => this.skills = data,
      error: (err: any) => console.error('Failed to load skills', err)
    });

    // 3. Fetch Certifications safely
    this.projectService.getCertifications().subscribe({
      next: (data: Certification[]) => this.certifications = data,
      error: (err: any) => console.error('Failed to load certifications', err)
    });
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    this.isScrolled = window.scrollY > 50;
  }

  // --- UI Methods ---
  scrollTo(elementId: string): void {
    const element = document.getElementById(elementId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  downloadCV(): void {
    const link = document.createElement('a');
    link.href = 'assets/CV.pdf'; 
    link.download = 'CV.pdf';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  // --- AI Chat Methods ---
  toggleChat(): void {
    this.isChatOpen = !this.isChatOpen;
  }

  sendChatMessage(): void {
    if (!this.chatInput.trim()) return;

    const userMsg = this.chatInput.trim();
    this.chatMessages.push({ role: 'user', content: userMsg });
    this.chatInput = '';
    this.isChatTyping = true;

    this.http.post<{reply: string}>('https://portfolio-website-2prc.onrender.com/api/ai/chat', { message: userMsg }).subscribe({
      next: (res) => {
        this.chatMessages.push({ role: 'ai', content: res.reply });
        this.isChatTyping = false;
      },
      error: (err: any) => {
        console.error(err);
        this.chatMessages.push({ role: 'ai', content: "System Error: Unable to reach the C# AI endpoint." });
        this.isChatTyping = false;
      }
    });
  }
}
