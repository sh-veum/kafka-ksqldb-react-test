import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { Bid } from "@/models/Bid";
import useWebSocket from "@/utils/webSocket/useWebSocket";
import { baseWebsocketUrl } from "@/lib/baseUrls";

/**
 * Custom hook to manage the WebSocket connection for bids.
 *
 * @param {string} auctionId - The ID of the auction.
 * @param {boolean} isEnabled - Whether the WebSocket should be enabled.
 */
const useBidsWebSocket = (auctionId: string, isEnabled: boolean) => {
  const queryClient = useQueryClient();

  const getWebSocketUrl = () =>
    `${baseWebsocketUrl}?auctionId=${auctionId}&webSocketSubscription=${WebSocketSubscription.SpecificAuction}`;

  const onMessage = (event: MessageEvent) => {
    const data = JSON.parse(event.data);
    console.log("Bids WebSocket message received:", data);

    queryClient.setQueryData(["auctionBids", auctionId], (oldData: Bid[]) => {
      return [...(oldData || []), data];
    });
  };

  useWebSocket(isEnabled, getWebSocketUrl, onMessage);
};

export default useBidsWebSocket;
