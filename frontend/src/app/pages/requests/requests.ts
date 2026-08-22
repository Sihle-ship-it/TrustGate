import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-requests',
  imports: [CommonModule, FormsModule],
  templateUrl: './requests.html',
  styleUrl: './requests.css',
})
export class Requests implements OnInit {
  users: any[] = [];
  entitlements: any[] = [];
  requests: any[] = [];
  userId = 0;
  entitlementId = 0;
  reason = "";

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.load();
  }

  load() {{
    this.api.getUsers().subscribe((u) => (this.users = u));
    this.api.getEntitlements().subscribe((e) => (this.entitlements = e));
    this.api.getRequests().subscribe((r) => (this.requests = r));
    }
  }

  submit(){
    this.api.createRequest(+this.userId, +this.entitlementId, this.reason).subscribe(() => this.load());
  }
  
}
