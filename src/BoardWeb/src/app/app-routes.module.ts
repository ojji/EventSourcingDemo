import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ProjectsDashboardComponent } from './projects/projects-dashboard.component';
import { CreateProjectComponent } from './projects/create-project.component';

const appRoutes: Routes = [
	{ path: 'projects-dashboard', component: ProjectsDashboardComponent },
	{ path: 'create-project', component: CreateProjectComponent },
	{ path: '', redirectTo: 'projects-dashboard', pathMatch: 'full' }
];

@NgModule({
	imports: [ RouterModule.forRoot(appRoutes) ],
	exports: [ RouterModule ]
})
export class AppRoutesModule { }