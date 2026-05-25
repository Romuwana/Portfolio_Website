import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjectService } from '../../services/project'; // Ensure the path matches
import { AuthService } from '../../services/auth.service';
import { Project } from '../../models/project';
import { Skill } from '../../models/skill';
import { Certification } from '../../models/certification';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class AdminComponent implements OnInit {
  activeTab: 'dashboard' | 'projects' | 'skills' | 'certs' = 'dashboard';
  
  projects: Project[] = [];
  skills: Skill[] = [];
  certifications: Certification[] = [];

  // --- Dashboard Stats ---
  softwareCount = 0;
  dataScienceCount = 0;
  topTech: string = 'N/A';
  pieChartStyle = '';
  activityData = [40, 70, 45, 90, 60, 100, 85]; 

  // --- File Upload State ---
  selectedFiles: File[] = [];
  isUploading = false;

  // --- Modal States ---
  isProjectModalOpen = false;
  isSkillModalOpen = false;
  isCertModalOpen = false;

  editingProject: Project = this.getEmptyProject();
  editingSkill: Skill = this.getEmptySkill();
  editingCert: Certification = this.getEmptyCert();
  
  techStackInput = ''; 

  constructor(
    private router: Router,
    private projectService: ProjectService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  // Fetch everything from the C# PostgreSQL database
  loadData(): void {
    this.projectService.getProjects().subscribe(data => {
      this.projects = data || [];
      this.calculateStats();
    });
    
    this.projectService.getSkills().subscribe(data => {
      this.skills = data || [];
      this.calculateStats(); // Recalculate top tech once skills load
    });
    
    this.projectService.getCertifications().subscribe(data => {
      this.certifications = data || [];
    });
  }

  calculateStats(): void {
    this.softwareCount = this.projects.filter(p => p.category === 'Software' || !p.category).length;
    this.dataScienceCount = this.projects.filter(p => p.category === 'Data Science').length;
    
    const total = this.softwareCount + this.dataScienceCount || 1;
    const softwarePercent = (this.softwareCount / total) * 100;
    this.pieChartStyle = `conic-gradient(var(--syntax-blue) 0% ${softwarePercent}%, var(--syntax-green) ${softwarePercent}% 100%)`;

    if (this.skills && this.skills.length > 0) {
      const topSkill = [...this.skills].sort((a, b) => b.level - a.level)[0];
      this.topTech = topSkill.name;
    } else {
      this.topTech = 'N/A';
    }
  }

  // --- Global Actions ---
  switchTab(tab: 'dashboard' | 'projects' | 'skills' | 'certs'): void {
    this.activeTab = tab;
  }

  logout(): void {
    this.authService.logout();
  }

  closeModal(): void {
    this.isProjectModalOpen = false;
    this.isSkillModalOpen = false;
    this.isCertModalOpen = false;
    this.selectedFiles = []; // Clear any files if they cancel
  }

  onFilesSelected(event: any, maxFiles: number): void {
    const files: FileList = event.target.files;
    if (files.length > maxFiles) {
      alert(`You can only upload a maximum of ${maxFiles} images.`);
      event.target.value = ''; 
      return;
    }
    this.selectedFiles = Array.from(files);
  }

  // ==========================================
  // 1. PROJECT CRUD LOGIC
  // ==========================================
  getEmptyProject(): Project {
    return { title: '', description: '', category: 'Software', techStack: [], status: 'Draft', createdAt: new Date().toISOString().split('T')[0], githubUrl: '', datasetUrl: '' };
  }

  openNewProjectModal(): void {
    this.editingProject = this.getEmptyProject();
    this.techStackInput = '';
    this.isProjectModalOpen = true;
  }

  editProject(project: Project): void {
    this.editingProject = { ...project }; 
    this.techStackInput = this.editingProject.techStack?.join(', ') || '';
    this.isProjectModalOpen = true;
  }

  saveProject(): void {
    this.isUploading = true;
    
    // Convert string input back into the array for C#
    this.editingProject.techStack = this.techStackInput
      ? this.techStackInput.split(',').map(s => s.trim()).filter(s => s !== '')
      : [];

    if (this.selectedFiles.length > 0) {
      this.projectService.uploadImages(this.selectedFiles).subscribe({
        next: (urls) => {
          this.editingProject.imageUrls = [...(this.editingProject.imageUrls || []), ...urls];
          this.selectedFiles = [];
          this.finalizeSaveProject();
        },
        error: (err) => {
          console.error('Image upload failed', err);
          alert('Failed to upload images.');
          this.isUploading = false;
        }
      });
    } else {
      this.finalizeSaveProject();
    }
  }

  finalizeSaveProject(): void {
    if (this.editingProject.id) {
      // UPDATE Existing Project
      this.projectService.updateProject(this.editingProject.id, this.editingProject).subscribe({
        next: () => {
          this.isUploading = false;
          this.closeModal();
          this.loadData(); // Refresh UI
        },
        error: (err) => {
          console.error('Failed to update project', err);
          this.isUploading = false;
        }
      });
    } else {
      // CREATE New Project
      this.projectService.createProject(this.editingProject).subscribe({
        next: () => {
          this.isUploading = false;
          this.closeModal();
          this.loadData(); // Refresh UI
        },
        error: (err) => {
          console.error('Failed to create project', err);
          this.isUploading = false;
        }
      });
    }
  }

  deleteProject(id?: number): void {
    if (id && confirm('Are you sure you want to delete this project?')) {
      this.projectService.deleteProject(id).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error('Failed to delete project', err)
      });
    }
  }

  // ==========================================
  // 2. SKILL CRUD LOGIC
  // ==========================================
  getEmptySkill(): Skill {
    return { name: '', category: 'Technical Skills', level: 50 };
  }

  openNewSkillModal(): void {
    this.editingSkill = this.getEmptySkill();
    this.isSkillModalOpen = true;
  }

  editSkill(skill: Skill): void {
    this.editingSkill = { ...skill };
    this.isSkillModalOpen = true;
  }

  saveSkill(): void {
    if (this.editingSkill.id) {
      this.projectService.updateSkill(this.editingSkill.id, this.editingSkill).subscribe({
        next: () => {
          this.closeModal();
          this.loadData();
        },
        error: (err) => console.error('Failed to update skill', err)
      });
    } else {
      this.projectService.createSkill(this.editingSkill).subscribe({
        next: () => {
          this.closeModal();
          this.loadData();
        },
        error: (err) => console.error('Failed to create skill', err)
      });
    }
  }

  deleteSkill(id?: number): void {
    if (id && confirm('Are you sure you want to delete this skill?')) {
      this.projectService.deleteSkill(id).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error('Failed to delete skill', err)
      });
    }
  }

  // ==========================================
  // 3. CERTIFICATION CRUD LOGIC
  // ==========================================
  getEmptyCert(): Certification {
    return { name: '', issuer: '', dateIssued: new Date().toISOString().split('T')[0] };
  }

  openNewCertModal(): void {
    this.editingCert = this.getEmptyCert();
    this.isCertModalOpen = true;
  }

  editCert(cert: Certification): void {
    this.editingCert = { ...cert };
    this.isCertModalOpen = true;
  }

  saveCert(): void {
    this.isUploading = true;
    
    if (this.selectedFiles.length > 0) {
      this.projectService.uploadImages(this.selectedFiles).subscribe({
        next: (urls) => {
          this.editingCert.imageUrl = urls[0]; 
          this.selectedFiles = [];
          this.finalizeSaveCert();
        },
        error: (err) => {
          console.error('Image upload failed', err);
          this.isUploading = false;
        }
      });
    } else {
      this.finalizeSaveCert();
    }
  }

  finalizeSaveCert(): void {
    if (this.editingCert.id) {
      this.projectService.updateCertification(this.editingCert.id, this.editingCert).subscribe({
        next: () => {
          this.isUploading = false;
          this.closeModal();
          this.loadData();
        },
        error: (err) => {
          console.error('Failed to update cert', err);
          this.isUploading = false;
        }
      });
    } else {
      this.projectService.createCertification(this.editingCert).subscribe({
        next: () => {
          this.isUploading = false;
          this.closeModal();
          this.loadData();
        },
        error: (err) => {
          console.error('Failed to create cert', err);
          this.isUploading = false;
        }
      });
    }
  }

  deleteCert(id?: number): void {
    if (id && confirm('Are you sure you want to delete this certification?')) {
      this.projectService.deleteCertification(id).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error('Failed to delete cert', err)
      });
    }
  }
}