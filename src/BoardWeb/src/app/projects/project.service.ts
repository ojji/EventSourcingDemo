import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';

import { Project } from './project';
import { PROJECTS } from './mock-projects';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

interface ProjectNameAvailableResponse {
	projectName: string;
	isAvailable: boolean;
}

interface ProjectAbbreviationAvailableResponse {
	projectAbbreviation: string;
	isAvailable: boolean;
}

@Injectable()
export class ProjectService {
	serviceUrl: string = 'http://localhost:5001/api/projects';

	constructor(private http: Http) { }

	getProjects(): Observable<Project[]> {
		return this.http.get(this.serviceUrl)
			.map(res => res.json())
			.catch(this.handleError);
	}

	createProject(project: Project): Observable<string> {
		let createProjectDto = {
					projectName: project.projectName,
					projectAbbreviation: project.projectAbbreviation,
					description: project.description || '',
					projectType: project.projectType
			};

		let headers = new Headers({ 'Content-Type': 'application/json' });
		let requestOptions = new RequestOptions({headers: headers});

		return this.http.post(this.serviceUrl, createProjectDto, requestOptions)
				.map(this.getProjectIdFromHeader)
				.catch(this.handleError);
	}

	getProjectIdFromHeader(res: Response): string {
		let location = res.headers.get('Location');
		if (location) {
			return location.split('/').pop();
		}
	}

	getProject(id: string): Observable<Project> {
		return this.http.get(`${this.serviceUrl}/${id}`)
			.map(res => res.json() as Project)
			.catch(this.handleError);
	}

	isProjectNameTaken(projectName: string): Observable<boolean> {
		let checkUrl: string = this.serviceUrl + '/checkprojectname?projectName=' + this.fixedEncodeURIComponent(projectName);
		return this.http.get(checkUrl)
			.map(result => {
				let response: ProjectNameAvailableResponse = result.json();
				if (projectName === response.projectName && !response.isAvailable) {
					return true;
				} else {
					return false;
				}
			})
			.catch(this.handleError);
	}

	private fixedEncodeURIComponent(str: string) {
	  return encodeURIComponent(str).replace(/[!'()*]/g, function(c) {
	    return '%' + c.charCodeAt(0).toString(16);
	  });
	}

	isProjectAbbreviationTaken(projectAbbreviation: string): Observable<boolean> {
		let checkUrl: string = this.serviceUrl + '/checkprojectabbreviation?projectAbbreviation=' + this.fixedEncodeURIComponent(projectAbbreviation);
		return this.http.get(checkUrl)
			.map(result => {
				let response: ProjectAbbreviationAvailableResponse = result.json();
				if (projectAbbreviation === response.projectAbbreviation && !response.isAvailable) {
					return true;
				} else {
					return false;
				}
			})
			.catch(this.handleError);
	}

	handleError(error: Response | any) {
		console.error(error);
		return Observable.throw(error);
	}
}