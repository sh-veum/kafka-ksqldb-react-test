import type { UserLocationUpdate } from "./../models/UserLocationUpdate";
import { HttpService } from "@/lib/httpService";

export class LocationService {
  private baseUrl: string;
  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  async createTables() {
    return HttpService.performPostRequest(
      `${this.baseUrl}/api/location/create_location_table`
    );
  }

  async addLocation(userLocationUpdate: UserLocationUpdate) {
    return HttpService.performPostRequest(
      `${this.baseUrl}/api/location/add_location`,
      userLocationUpdate
    );
  }

  async removeLocation(userLocationUpdate: UserLocationUpdate) {
    return HttpService.performDeleteRequest(
      `${this.baseUrl}/api/location/remove_location`,
      userLocationUpdate
    );
  }

  async getUsersOnPage(page: string) {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/location/get_users_on_page?page=${page}`
    );
  }

  async getAllLocations() {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/location/get_all_locations`
    );
  }

  async getPagesForUser(userId?: string) {
    if (!userId) {
      return HttpService.performGetRequest(
        `${this.baseUrl}/api/location/get_pages_for_user`
      );
    } else {
      return HttpService.performGetRequest(
        `${this.baseUrl}/api/location/get_pages_for_user?userLocationId=${userId}`
      );
    }
  }
}
