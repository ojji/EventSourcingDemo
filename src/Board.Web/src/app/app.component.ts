import { Component } from '@angular/core';
import 'foundation-sites';

@Component({
    selector: 'my-app',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.scss' ]
})
export class AppComponent {
    title: string = 'Hello from angular!';
}