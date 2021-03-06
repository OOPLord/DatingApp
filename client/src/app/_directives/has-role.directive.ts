import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user.model';
import { AccountService } from '../_services/account.service';

@Directive({
    selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
    @Input() appHasRole: string[];
    user: User;

    constructor(private viewContainerRef: ViewContainerRef,
        private templateRef: TemplateRef<any>,
        private acountService: AccountService) {
            this.acountService.currentUser$.pipe(take(1))
                .subscribe(user => {
                    this.user = user;
                });
    }

    ngOnInit() {
        if (this.user?.roles || this.user == null) {
            this.viewContainerRef.clear();
        }

        if (this.user?.roles.some(role => this.appHasRole.includes(role))) {
            this.viewContainerRef.createEmbeddedView(this.templateRef);
        } else {
            this.viewContainerRef.clear();
        }
    }
}
