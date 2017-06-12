import { Component } from '@angular/core';
import { Location } from '@angular/common';

import { Project, ProjectType } from './project';

@Component({
	selector: 'create-project-form',
	templateUrl: './create-project.component.html',
	styleUrls: [ './create-project.component.scss' ]
})
export class CreateProjectComponent {
	project: Project = new Project();
	projectTypes = [ 'Scrum', 'Kanban' ];

	constructor(private location: Location) { }

	onCreate(model: Project, isValid: boolean): void {
		if (isValid) {
			console.log(JSON.stringify(model));
		}
	}

	onBack(): void {
		this.location.back();
	}
}