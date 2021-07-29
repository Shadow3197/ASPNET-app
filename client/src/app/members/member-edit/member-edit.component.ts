import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { Member } from 'src/app/models/member';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { take } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  @ViewChild('passwordForm') passwordForm: NgForm;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
    if (this.passwordForm.dirty) {
      $event.returnValue = true;
    }
  }
  model: any = {};
  member: Member;
  user: User;

  constructor(private accountService: AccountService, private memberService: MembersService,
    private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe(member => {
      this.member = member;
    })
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toastr.success('Profile updated successfully');
      this.editForm.reset(this.member);
    })
  }
  
  updatePassword() {
    if (this.model.currentPassword == " " || this.model.newPassword == " " || this.model.newReEnteredPassword == " ") {
      this.toastr.error('Please fill out all password fields');
    }
     else if (this.model.newPassword === this.model.newReEnteredPassword) {
      if (this.model.newPassword.match(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{4,15}$/))
        this.accountService.updatePassword(this.model).subscribe(() => {
          this.toastr.success('Password changed successfully');
          this.passwordForm.reset(this.model);
        })
        else {
          this.toastr.error('New Password must contain more than 4 characters and at least one lowercase letter, one uppercase letter, one numeric digit, and one special character.')
        }
      }
      else {
        this.toastr.error('Incorrect reentered password');
      }
    }
  }
