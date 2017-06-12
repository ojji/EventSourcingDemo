import { Component, OnInit } from '@angular/core';
import { Project } from './project';
import { ProjectService } from './project.service';

@Component({
	selector: 'projects-dashboard',
	templateUrl: './projects-dashboard.component.html',
	styleUrls: [ './projects-dashboard.component.scss' ]
})
export class ProjectsDashboardComponent implements OnInit {
	projects: Project[];
	errorMessage: string;

	constructor(private projectService: ProjectService) {
	}

	ngOnInit(): void {
		this.getProjects();
	}

	getProjects(): void {
		this.projectService.getProjects()
			.subscribe(
				projects => this.projects = projects,
				error => this.errorMessage = error);
	}
}