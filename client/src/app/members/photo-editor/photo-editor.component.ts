import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member.model';
import { Photo } from 'src/app/_models/photo.model';
import { User } from 'src/app/_models/user.model';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-photo-editor',
    templateUrl: './photo-editor.component.html',
    styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
    @Input() member: Member;
    uploader: FileUploader;
    hasBaseDropZoneOver = false;
    baseUrl = environment.apiUrl;
    user: User;

    constructor(private accountService: AccountService, private memberService: MemberService) {
        this.accountService.currentUser$
            .pipe(take(1)).subscribe(user => this.user = user);
    }

    ngOnInit() {
        this.initializeUploader();
    }

    fileOverBase(event: any) {
        this.hasBaseDropZoneOver = event;
    }

    setMainPhoto(photo: Photo) {
        this.memberService.setMainPhoto(photo.id)
            .subscribe(() => {
                this.user.photoUrl = photo.url;
                this.accountService.setCurentUser(this.user);
                this.member.photoUrl = photo.url;

                this.member.photos.forEach(p => {
                    if (p.isMain) {
                        p.isMain = false;
                    }

                    if (p.id === photo.id) {
                        p.isMain = true;
                    }
                });
            });
    }

    deletePhoto(photoId: number) {
        this.memberService.deletePhoto(photoId)
            .subscribe(() => {
                this.member.photos = this.member.photos.filter(photo => photo.id !== photoId);
            });
    }

    initializeUploader() {
        this.uploader = new FileUploader({
            url: this.baseUrl + 'users/add-photo',
            authToken: 'Bearer ' + this.user.token,
            isHTML5: true,
            allowedFileType: ['image'],
            removeAfterUpload: true,
            autoUpload: false,
            maxFileSize: 10 * 1024 * 1024
        });

        this.uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        }

        this.uploader.onSuccessItem = (item, response, status, headers) => {
            if (response) {
                const photo: Photo = JSON.parse(response);
                this.member.photos.push(photo);

                if (photo.isMain) {
                    this.user.photoUrl = photo.url;
                    this.member.photoUrl = photo.url;
                    this.accountService.setCurentUser(this.user);
                }
            }
        }
    }
}
