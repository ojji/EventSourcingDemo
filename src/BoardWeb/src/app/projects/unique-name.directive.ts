import { Directive } from '@angular/core';
import { AbstractControl, NG_ASYNC_VALIDATORS, Validator } from '@angular/forms';

import { ProjectService } from './project.service';

@Directive({
	selector: '[uniquename]',
	providers: [{provide: NG_ASYNC_VALIDATORS, useExisting: UniqueNameValidatorDirective, multi: true}]
})
export class UniqueNameValidatorDirective implements Validator {
	timer: number;
	debounceTime: number = 500;

	constructor(private projectService: ProjectService) { }

	validate(c: AbstractControl): Promise<{[key: string]: any}> {		
		let projectName = c.value;

		if (this.timer) {
			clearTimeout(this.timer);
		}

		return new Promise((resolve, reject) => {
			this.timer = setTimeout(() => {
				this.projectService.isProjectNameTaken(projectName)
					.subscribe(isTaken => {
						if (isTaken) {
							resolve({ 'unique': true });
						} else {
							resolve(null);
						}
					},
					error => reject(error));
			}, this.debounceTime);
		});
	}
}