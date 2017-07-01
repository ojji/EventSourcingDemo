import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { Project } from './project';
import { ProjectService } from './project.service';
import 'rxjs/add/operator/switchMap';

@Component({
	selector: 'project-dashboard',
	templateUrl: './project-dashboard.component.html',
	styleUrls: [ './project-dashboard.component.scss' ]
})
export class ProjectDashboardComponent implements OnInit {
	project: Project = null;
	loading: boolean = true;

	constructor(private route: ActivatedRoute,
				private projectService: ProjectService) { }

	ngOnInit() : void {
		this.route.params
			.switchMap((params: Params) => 
				this.projectService.getProject(params['id']))
			.subscribe((project: Project) => {
				this.project = project;
				this.loading = false;				
			});
	}
}