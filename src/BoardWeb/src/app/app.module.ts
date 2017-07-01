import { NgModule} from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MomentModule } from 'angular2-moment';

import { AppRoutesModule } from './app-routes.module';

import { AppComponent } from './app.component';
import { MessagesComponent } from './messages.component';
import { ShowProjectsComponent } from './projects/show-projects.component';
import { ProjectDashboardComponent } from './projects/project-dashboard.component';
import { CreateProjectComponent } from './projects/create-project.component';

import { UniqueNameValidatorDirective } from './projects/unique-name.directive';
import { UniqueAbbreviationValidatorDirective } from './projects/unique-abbreviation.directive';

import { ProjectService } from './projects/project.service'

@NgModule({
    imports: [ 
    	BrowserModule, 
    	CommonModule,
    	FormsModule,
        HttpModule,
    	MomentModule,    	
    	AppRoutesModule ],
    providers: [ ProjectService ],
    declarations: [ 
    	AppComponent,
    	MessagesComponent,
        ShowProjectsComponent,
    	ProjectDashboardComponent,
        CreateProjectComponent,
        UniqueNameValidatorDirective,
        UniqueAbbreviationValidatorDirective
        ],
    bootstrap: [ AppComponent ]
})
export class AppModule { }