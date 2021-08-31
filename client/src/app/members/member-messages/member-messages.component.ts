import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message.model';
import { MessageService } from 'src/app/_services/message.service';

@Component({
    selector: 'app-member-messages',
    templateUrl: './member-messages.component.html',
    styleUrls: ['./member-messages.component.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MemberMessagesComponent implements OnInit {
    @ViewChild('messageForm') messageForm: NgForm;
    @Input() messages: Message[];
    @Input() userName: string;
    messageContent: string;
    loading = false;

    constructor(public messageService: MessageService) {
    }

    ngOnInit() {
    }

    sendMessage() {
        this.loading = true;
        this.messageService.sendMessage(this.userName, this.messageContent)
            .then(() => {
                this.messageForm.reset();
            })
            .finally(() => this.loading = false);
    }
}
