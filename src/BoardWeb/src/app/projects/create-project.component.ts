import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';

import { Project, ProjectType } from './project';
import { ProjectService } from './project.service';

@Component({
	selector: 'create-project-form',
	templateUrl: './create-project.component.html',
	styleUrls: [ './create-project.component.scss' ]
})
export class CreateProjectComponent {
	project: Project = new Project();
	projectTypes = [ 'Scrum', 'Kanban' ];

	constructor(private location: Location, 
				private router: Router,
				private projectService: ProjectService) { }

	onCreate(model: Project, isValid: boolean): void {
		if (isValid) {
			this.projectService.createProject(model)
				.subscribe(
					newProjectId => 
						setTimeout(() => this.router.navigate(['/project-dashboard', newProjectId], 1500)),
					error => 
						console.error(error));
		}
	}

	onBack(): void {
		this.location.back();
	}
}