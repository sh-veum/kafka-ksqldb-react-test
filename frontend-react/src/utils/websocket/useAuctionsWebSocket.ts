import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { Auction } from "@/models/Auction";
import { addOrUpdateData } from "@/utils/addOrUpdateData";
import useWebSocket from "@/utils/webSocket/useWebSocket";
import { baseWebsocketUrl } from "@/lib/baseUrls";

/**
 * Custom hook to manage the WebSocket connection for auctions.
 *
 * @param {boolean} isEnabled - Whether the WebSocket should be enabled.
 */
const useAuctionsWebSocket = (isEnabled: boolean) => {
  const queryClient = useQueryClient();

  const getWebSocketUrl = () =>
    `${baseWebsocketUrl}?auctionId=none&webSocketSubscription=${WebSocketSubscription.AuctionOverview}`;

  const onMessage = (event: MessageEvent) => {
    const data = JSON.parse(event.data);
    console.log("Auctions WebSocket message received:", data);

    queryClient.setQueryData(["allAuctions"], (oldData: Auction[]) => {
      return addOrUpdateData(oldData, data, (key) => key.Auction_Id);
    });
  };

  useWebSocket(isEnabled, getWebSocketUrl, onMessage);
};

export default useAuctionsWebSocket;
