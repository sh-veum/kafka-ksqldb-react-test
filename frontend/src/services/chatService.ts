import { HttpService } from "@/lib/httpService";

export class ChatService {
  private baseUrl: string;
  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  async insertChatMessage(message: {
    username: string;
    messageText: string;
    auction_Id: string;
  }) {
    return HttpService.performPostRequest(
      `${this.baseUrl}/api/chat/insert_message`,
      message
    );
  }

  async getChatMessagesForAuction(auctionId: string) {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/chat/get_messages_for_auction?auction_Id=${auctionId}&sortByDate=true`
    );
  }

  async getChatMessagesForAuctionAlt(auctionId: string) {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/chat/get_messages_for_auction_push_query?auction_Id=${auctionId}`
    );
  }
}
