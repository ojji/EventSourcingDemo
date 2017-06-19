import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ShowProjectsComponent } from './projects/show-projects.component';
import { CreateProjectComponent } from './projects/create-project.component';

const appRoutes: Routes = [
	{ path: 'show-projects', component: ShowProjectsComponent },
	{ path: 'create-project', component: CreateProjectComponent },
	{ path: '', redirectTo: 'show-projects', pathMatch: 'full' }
];

@NgModule({
	imports: [ RouterModule.forRoot(appRoutes) ],
	exports: [ RouterModule ]
})
export class AppRoutesModule { }