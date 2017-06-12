import { Component } from '@angular/core';
import 'foundation-sites';

import '../style/style.scss';

@Component({
    selector: 'board-app',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.scss' ]
})
export class AppComponent {
    isSideBarInDefaultState: boolean = true;

    toggleSidebar(): void {
    	this.isSideBarInDefaultState = !this.isSideBarInDefaultState;
    }
}