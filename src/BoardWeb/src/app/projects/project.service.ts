import { Injectable } from '@angular/core';

import { Project } from './project';
import { PROJECTS } from './mock-projects';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';

@Injectable()
export class ProjectService {
	getProjects(): Observable<Project[]> {
		return Observable.of(PROJECTS);
	}
}