import { Directive } from '@angular/core';
import { AbstractControl, NG_ASYNC_VALIDATORS, Validator } from '@angular/forms';

import { ProjectService } from './project.service';

@Directive({
	selector: '[uniqueabbreviation]',
	providers: [{provide: NG_ASYNC_VALIDATORS, useExisting: UniqueAbbreviationValidatorDirective, multi: true}]
})
export class UniqueAbbreviationValidatorDirective implements Validator {
	timer: number;
	debounceTime: number = 500;

	constructor(private projectService: ProjectService) { }

	validate(c: AbstractControl): Promise<{[key: string]: any}> {		
		let projectAbbreviation = c.value;

		if (this.timer) {
			clearTimeout(this.timer);
		}

		return new Promise((resolve, reject) => {
			this.timer = setTimeout(() => {
				this.projectService.isProjectAbbreviationTaken(projectAbbreviation)
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