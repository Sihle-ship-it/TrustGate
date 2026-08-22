import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin',
  imports: [FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin implements OnInit{
  requests: any[] = [];
  admin = "admin";

  constructor(private api: ApiService) { }

  ngOnInit(): void {  
    this.api.getRequests().subscribe((r) => (this.requests = r));
  }

  pending() {
    return this.requests.filter(r => r.status === 'Pending');
  }

  decide(id: number, approve: boolean) {
    const call = approve ? this.api.approve(id, this.admin) 
                         : this.api.reject(id, this.admin);
    call.subscribe(() => 
    this.api.getRequests().subscribe((r) => (this.requests = r)));
  }
}
