import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { ProjectService } from './project'; 

describe('ProjectService', () => {
  let service: ProjectService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      // We must provide testing HTTP so the service can be built
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    // Inject the correctly named service
    service = TestBed.inject(ProjectService); 
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});