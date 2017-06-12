import { platformBrowser } from '@angular/platform-browser';
import { enableProdMode } from "@angular/core";
import { AppModule } from './app/app.module';
declare var process;
if (process.env.ENV === 'production') {
    enableProdMode();
}

platformBrowser().bootstrapModule(AppModule);