import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
private base = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  getUsers() {
    return this.http.get<any[]>(`${this.base}/users`);
  }
  getEntitlements() {
    return this.http.get<any[]>(`${this.base}/entitlements`);
  }
  getRequests() {
    return this.http.get<any[]>(`${this.base}/requests`);
  }

  createRequest(userId: number, entitlementId: number, reason: string) {
    return this.http.post(`${this.base}/requests`, { userId, entitlementId, reason });
  }

  approve(id: number, admin: string) {
    return this.http.post(`${this.base}/requests/${id}/approve?adminUsername=${admin}`, { admin },{});
  }

  reject(id: number, admin: string) {
    return this.http.post(`${this.base}/requests/${id}/reject?adminUsername=${admin}`, { admin },{});
  }
}
