import { defineStore } from "pinia";
import { ref } from "vue";
import { AuctionService } from "@/services/auctionService";
import type { AuctionMessage } from "@/models/AuctionMessage";
import type { AuctionBidsMessage } from "@/models/AuctionBidsMessage";
import type { RecentBidMessage } from "@/models/RecentBidMessage";
import { AuctionWebSocketHandler } from "@/services/auctionWebSocketHandler";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useAuctionStore = defineStore("auction", () => {
  const auctionMessages = ref<Map<string, AuctionMessage>>(new Map());
  const auctionBidsMessages = ref<AuctionBidsMessage[]>([]);
  const allRecentBidsMessages = ref<RecentBidMessage[]>([]);

  const auctionService = new AuctionService(baseUrl);
  const webSocketHandler = new AuctionWebSocketHandler(
    auctionMessages,
    auctionBidsMessages,
    allRecentBidsMessages
  );

  const getAllTables = async () => {
    const { data, loading, error } = await auctionService.getAllTables();
    if (data) {
      auctionMessages.value.clear();
      data.forEach((message: AuctionMessage) => {
        auctionMessages.value.set(message.Auction_Id, message);
      });
    }
    return { loading, error };
  };

  const getAuctionBidsForAuction = async (auctionId: string) => {
    const { data, loading, error } =
      await auctionService.getAuctionBidsForAuction(auctionId);
    if (data) {
      auctionBidsMessages.value = data;
    }
    return { loading, error };
  };

  return {
    auctionMessages,
    auctionBidsMessages,
    allRecentBidsMessages,
    createTables: auctionService.createTables.bind(auctionService),
    createStreams: auctionService.createStreams.bind(auctionService),
    insertAuction: auctionService.insertAuction.bind(auctionService),
    insertAuctionBid: auctionService.insertAuctionBid.bind(auctionService),
    dropTables: auctionService.dropTables.bind(auctionService),
    getAllTables,
    getAuctionById: auctionService.getAuctionById.bind(auctionService),
    getAuctionBidsForAuction,
    connectAuctionOverviewWebSocket:
      webSocketHandler.connectAuctionOverviewWebSocket.bind(webSocketHandler),
    connectBidsWebSocket:
      webSocketHandler.connectBidsWebSocket.bind(webSocketHandler),
    connectAllRecentBidsWebSocket:
      webSocketHandler.connectAllRecentBidsWebSocket.bind(webSocketHandler),
    disconnectAuctionOverviewWebSocket:
      webSocketHandler.disconnectAuctionOverviewWebSocket.bind(
        webSocketHandler
      ),
    disconnectBidsWebSocket:
      webSocketHandler.disconnectBidsWebSocket.bind(webSocketHandler),
    disconnectAllRecentBidsWebSocket:
      webSocketHandler.disconnectAllRecentBidsWebSocket.bind(webSocketHandler),
  };
});
