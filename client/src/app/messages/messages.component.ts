import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message.model';
import { Pagination } from '../_models/pagination.model';
import { ConfirmService } from '../_services/confirm.service';
import { MessageService } from '../_services/message.service';

@Component({
    selector: 'app-messages',
    templateUrl: './messages.component.html',
    styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
    messages: Message[] = [];
    pagination: Pagination;
    container = 'Inbox';
    pageNumber = 1;
    pageSize = 5;
    loading: boolean;
    
    constructor(private messageService: MessageService, private confirmService: ConfirmService) {

    }

    ngOnInit() {
        this.loadMessages();
    }

    loadMessages() {
        this.loading = true;
        this.messageService.getMessages(this.pageNumber, this.pageSize, this.container)
            .subscribe(responce => {
                this.messages = responce.result;
                this.pagination = responce.pagination;
                this.loading = false;
            });
    }

    deleteMessage(id: number) {
        this.confirmService.confirm('Confirm delete message')
            .subscribe(result => {
                if (result) {
                    this.messageService.deleteMessage(id)
                        .subscribe(() => {
                            this.messages.splice(this.messages.findIndex(message => message.id === id), 1)
                        });
                }
            });
    }

    pageChanged(event: any) {
        this.pageNumber = event.page;
        this.loadMessages();
    }
}
