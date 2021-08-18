import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  members: Partial<Member[]>;

  constructor(private memberService: MembersService, private toastr: ToastrService, 
    public presence: PresenceService) { }

    ngOnInit(): void {
    }
  
    getLike(member: Member) {
      this.memberService.getLike(this.member.username).subscribe(memberLike => {
        if (memberLike == null)this.addLike(member);
        else this.removeLike(member);
      })
    }
    addLike(member: Member){
      this.memberService.addLike(member.username).subscribe(() => {
        this.toastr.success('You have liked ' + member.knownAs);
      })
    }
    removeLike(member: Member){
    this.memberService.removeLike(member.username).subscribe(() => {
      this.toastr.success('You have unliked ' + member.knownAs);
    })
  }
}
