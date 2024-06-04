import type { AuctionBid } from "@/models/AuctionBid";
import type { AuctionCreator } from "@/models/AuctionCreator";
import { HttpService } from "@/lib/httpService";

export class AuctionService {
  private baseUrl: string;
  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  async createTables() {
    return HttpService.performPostRequest(
      `${this.baseUrl}/api/auction/create_tables`
    );
  }

  async createStreams() {
    return HttpService.performPostRequest(
      `${this.baseUrl}/api/auction/create_streams`
    );
  }

  async insertAuction(auction: AuctionCreator) {
    return HttpService.performPostRequest(
      `${this.baseUrl}/api/auction/insert_auction`,
      auction
    );
  }

  async insertAuctionBid(bid: AuctionBid) {
    return HttpService.performPostRequest(
      `${this.baseUrl}/api/auction/insert_auction_bid`,
      bid
    );
  }

  async dropTables() {
    return HttpService.performDeleteRequest(
      `${this.baseUrl}/api/auction/drop_tables`
    );
  }

  async getAllTables() {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/auction/get_all_auctions?sortByDate=true`
    );
  }

  async getAuctionById(auctionId: string) {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/auction/get_auction_by_id?auction_Id=${auctionId}`
    );
  }

  async getAuctionBidsForAuction(auctionId: string) {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/auction/get_bid_messages_for_auction?auction_Id=${auctionId}`
    );
  }
}
