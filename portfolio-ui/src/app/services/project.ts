import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project } from '../models/project';
import { Skill } from '../models/skill';
import { Certification } from '../models/certification';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
private apiUrl = 'https://portfolio-website-2prc.onrender.com/api/projects';
private skillsUrl = 'https://portfolio-website-2prc.onrender.com/api/skills';
private certsUrl = 'https://portfolio-website-2prc.onrender.com/api/certs';

  constructor(private http: HttpClient) {}

  // ===================== PROJECTS =====================
  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.apiUrl);
  }

  createProject(project: Project): Observable<Project> {
    return this.http.post<Project>(this.apiUrl, project);
  }

  updateProject(id: number, project: Project): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, project);
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  uploadImages(files: File[]): Observable<string[]> {
    const formData = new FormData();
    files.forEach(file => {
      formData.append('files', file, file.name);
    });
    return this.http.post<string[]>('https://localhost:44391/api/upload', formData);
  }

  // ===================== SKILLS =====================
  getSkills(): Observable<Skill[]> {
    return this.http.get<Skill[]>(this.skillsUrl);
  }

  createSkill(skill: Skill): Observable<Skill> {
    return this.http.post<Skill>(this.skillsUrl, skill);
  }

  updateSkill(id: number, skill: Skill): Observable<any> {
    return this.http.put(`${this.skillsUrl}/${id}`, skill);
  }

  deleteSkill(id: number): Observable<any> {
    return this.http.delete(`${this.skillsUrl}/${id}`);
  }

  // ===================== CERTIFICATIONS =====================
  getCertifications(): Observable<Certification[]> {
    return this.http.get<Certification[]>(this.certsUrl);
  }

  createCertification(cert: Certification): Observable<Certification> {
    return this.http.post<Certification>(this.certsUrl, cert);
  }

  updateCertification(id: number, cert: Certification): Observable<any> {
    return this.http.put(`${this.certsUrl}/${id}`, cert);
  }

  deleteCertification(id: number): Observable<any> {
    return this.http.delete(`${this.certsUrl}/${id}`);
  }
}