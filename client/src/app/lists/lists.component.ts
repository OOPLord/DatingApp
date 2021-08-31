import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Member } from '../_models/member.model';
import { Pagination } from '../_models/pagination.model';
import { MemberService } from '../_services/member.service';

@Component({
    selector: 'app-lists',
    templateUrl: './lists.component.html',
    styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
    members: Partial<Member[]>;
    predicate = 'liked';
    pageNumber = 1;
    pageSize = 5;
    pagination: Pagination;

    constructor(private memberService: MemberService) {

    }

    ngOnInit() {
        this.loadLikes();
    }

    loadLikes() {
        this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize)
            .subscribe(responce => {
                this.members = responce.result;
                this.pagination = responce.pagination;
            });
    }

    pageChanged(event: any) {
        this.pageNumber = event.page;
        this.loadLikes();
    }
}
