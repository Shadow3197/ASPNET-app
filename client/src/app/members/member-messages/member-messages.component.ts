import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/models/message';
import { MessageService } from 'src/app/_services/message.service';
import { MembersService } from 'src/app/_services/members.service';
import { Member } from 'src/app/models/member';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() username: string;
  @Input() messages: Message[];
  @Input() userBlocked: boolean;
  userHasBlocked: boolean;
  predicate = 'blockedby';
  member: Member;
  messageContent: string;
  loading = false;

  constructor(public messageService: MessageService, private memberService: MembersService, 
    private route: ActivatedRoute, private toastr: ToastrService) { }

  ngOnInit(): void {

    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    this.getUserHasBlocked();
  }

  getUserHasBlocked() {
    this.memberService.getUserHasBlocked(this.predicate, this.username).subscribe(response => {
      if (response.result == null){this.userHasBlocked = false} 
      else {this.userHasBlocked = true};
    })
  }

  sendMessage() {
      if(this.userBlocked == false || this.userHasBlocked == false) {
        this.loading = true;
      this.messageService.sendMessage(this.username, this.messageContent).then(() => {
        this.messageForm.reset();
      }).finally(() => this.loading = false);
    }
    else{
      this.toastr.error('This user is blocked');
    }
    } 
}
