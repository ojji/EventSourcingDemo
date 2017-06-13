import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';

import { Project } from './project';
import { PROJECTS } from './mock-projects';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class ProjectService {
	serviceUrl: string = 'http://localhost:5001/api/projects';

	constructor(private http: Http) { }

	getProjects(): Observable<Project[]> {
		return this.http.get(this.serviceUrl)
			.map(res => res.json())
			.catch(this.handleError);
	}

	createProject(project: Project) {
		let createProjectDto = {
					projectName: project.projectName,
					description: project.description || '',
					projectType: project.projectType
			};

		let headers = new Headers({ 'Content-Type': 'application/json' });
		let requestOptions = new RequestOptions({headers: headers});

		return this.http.post(this.serviceUrl, createProjectDto, requestOptions)
				.catch(this.handleError);
	}

	handleError(error: Response | any) {
		console.error(error);
		return Observable.throw(error);
	}
}