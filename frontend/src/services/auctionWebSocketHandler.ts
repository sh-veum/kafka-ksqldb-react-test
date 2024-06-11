import { type Ref } from "vue";
import type { AuctionMessage } from "@/models/AuctionMessage";
import type { AuctionBidsMessage } from "@/models/AuctionBidsMessage";
import type { RecentBidMessage } from "@/models/RecentBidMessage";
import { webSocketService } from "@/lib/webSocket";

export class AuctionWebSocketHandler {
  private auctionMessages: Ref<Map<string, AuctionMessage>>;
  private auctionBidsMessages: Ref<AuctionBidsMessage[]>;
  private allRecentBidsMessages: Ref<RecentBidMessage[]>;

  constructor(
    auctionMessages: Ref<Map<string, AuctionMessage>>,
    auctionBidsMessages: Ref<AuctionBidsMessage[]>,
    allRecentBidsMessages: Ref<RecentBidMessage[]>
  ) {
    this.auctionMessages = auctionMessages;
    this.auctionBidsMessages = auctionBidsMessages;
    this.allRecentBidsMessages = allRecentBidsMessages;
  }

  connectAuctionOverviewWebSocket(onError: (err: string) => void) {
    webSocketService.connectAuctionOverview((data: AuctionMessage) => {
      this.auctionMessages.value.set(data.Auction_Id, data);
    }, onError);
  }

  connectBidsWebSocket(auctionId: string, onError: (err: string) => void) {
    webSocketService.connectBids(
      auctionId,
      (data) => {
        this.auctionBidsMessages.value.push(data);
      },
      onError
    );
  }

  connectAllRecentBidsWebSocket(onError: (err: string) => void) {
    webSocketService.connectAllRecentBids((data) => {
      this.allRecentBidsMessages.value.push(data);
    }, onError);
  }

  disconnectAuctionOverviewWebSocket() {
    webSocketService.disconnectAuctionOverview();
  }

  disconnectBidsWebSocket() {
    webSocketService.disconnectBids();
  }

  disconnectAllRecentBidsWebSocket() {
    webSocketService.disconnectAllRecentBids();
  }
}
