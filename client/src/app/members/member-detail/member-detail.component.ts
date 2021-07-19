import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/member';
import { Message } from 'src/app/models/message';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member;
  isUserLiked: boolean;
  likeButton: string;
  isUserBlocked: boolean;
  blockButton: string;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] =[];
  user: User;

  constructor(public presence: PresenceService, private route: ActivatedRoute, private memberService: MembersService, private toastr: ToastrService,
    private messageService: MessageService, private accountService: AccountService, private router: Router) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    this.getLike();
    this.getBlockedUser();

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      }
    ]

    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for(const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imageUrls;
  }
  
  loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }

  getLike() {
    this.memberService.getLike(this.member.username).subscribe(member => {
      if (member == null){this.isUserLiked = false, this.likeButton = 'Like'} 
      else {this.isUserLiked = true, this.likeButton = 'Unlike'};
    })
  }
  getBlockedUser() {
    this.memberService.getBlockedUser(this.member.username).subscribe(member => {
      if (member == null){this.isUserBlocked = false, this.blockButton = 'Block'} 
      else {this.isUserBlocked = true, this.blockButton = 'Unblock'};
    })
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.username);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  addRemoveLike(member: Member){
    if(this.isUserLiked == false){
      this.memberService.addLike(member.username).subscribe(() => {
        this.toastr.success('You have liked ' + member.knownAs);
      })
      this.isUserLiked = true;
      this.likeButton = "Unlike";
    }
    else if(this.isUserLiked == true) {
      this.memberService.removeLike(member.username).subscribe(() => {
        this.toastr.success('You have unliked ' + member.knownAs);
      })
      this.isUserLiked = false;
      this.likeButton = "Like";
    } 
  }
  blockUser(member: Member){
   if(this.isUserBlocked == false){
      this.memberService.block(member.username).subscribe(() => {
        this.toastr.success('You have blocked ' + member.knownAs);
       })
       this.isUserBlocked = true;
       this.blockButton = "Unblock";
     }
    else if(this.isUserBlocked == true) {
       this.memberService.unblock(member.username).subscribe(() => {
         this.toastr.success('You have unblocked ' + member.knownAs);
       })
       this.isUserBlocked = false;
       this.blockButton = "Block";
     } 
   }
}
