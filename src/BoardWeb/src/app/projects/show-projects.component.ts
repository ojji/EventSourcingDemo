import { Component, OnInit } from '@angular/core';
import { Project } from './project';
import { ProjectService } from './project.service';

@Component({
	selector: 'show-projects',
	templateUrl: './show-projects.component.html',
	styleUrls: [ './show-projects.component.scss' ]
})
export class ShowProjectsComponent implements OnInit {
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