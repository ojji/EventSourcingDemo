import { Component, Renderer2, ElementRef } from '@angular/core';

import { Message } from './message';

const MESSAGES: Message[] = [
	{sender: 'Support team', dateSent: new Date(Date.now() + -       10*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
	{sender: 'Support team', dateSent: new Date(Date.now() + -5*24*3600*1000), content: 'Morbi dui lorem, suscipit vel justo a, ultrices ultrices sapien.' },
];

@Component({
	selector: 'board-messages',
	templateUrl: './messages.component.html',
	styleUrls: [ './messages.component.scss' ]
})
export class MessagesComponent {
	messages: Message[] = MESSAGES;
	isOpen: boolean = false;
	addBodyClose: boolean = true;
	private closeListener: () => void;

	constructor(private renderer: Renderer2, private parentElement: ElementRef) { }

	toggle(): void {		
		if (this.isOpen) {
			this.close();
		}
		else {
			this.open();
		}
	}

	private open(): void {
		this.isOpen = true;
		if (this.addBodyClose) {
			this.closeListener = this.renderer.listen('body', 'click', (evt) => {
				if (!this.isDescendant(this.parentElement.nativeElement, evt.target)) {
					this.close();
				}
			});
		}		
	}

	private close(): void {
		this.isOpen = false;
		if (this.addBodyClose) {
			this.closeListener();
		}
	}

	private isDescendant(parent: any, element: any): boolean {
		let node = element;
		while (node !== null) {
			if (node === parent) {
				return true;
			}
			node = node.parentNode;
		}
		return false;
	}
}