import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { useEffect, useRef } from "react";
import { baseWebsocketUrl } from "@/lib/baseUrls";
import { convertUTCToLocalTime } from "../timeUtils";
import { AuctionWithBid } from "@/models/AuctionWithBid";
import { addOrUpdateData } from "../addOrUpdateData";

/**
 * Custom hook to manage the WebSocket connection for all bids for all auctions.
 *
 * @param {boolean} isEnabled - Whether the WebSocket should be enabled.
 */
export const useAllAuctionWithBidsWebSocket = (isEnabled: boolean) => {
  const queryClient = useQueryClient();
  const websocketRef = useRef<WebSocket | null>(null);

  useEffect(() => {
    if (!isEnabled || websocketRef.current) return;

    const webSocketUrl = `${baseWebsocketUrl}?auctionId=none&webSocketSubscription=${WebSocketSubscription.AllRecentBids}`;
    const websocket = new WebSocket(webSocketUrl);
    websocketRef.current = websocket;

    websocket.onopen = () => {
      console.log("connected to all recent bids LATEST websocket");
    };

    websocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Auction bids LATEST WebSocket message received:", data);

      const auctionWithBid: AuctionWithBid = {
        ...data,
        Timestamp: convertUTCToLocalTime(data.Timestamp),
      };

      queryClient.setQueryData(
        ["allAuctionsWithBids"],
        (oldData: AuctionWithBid[]) => {
          return addOrUpdateData(oldData, auctionWithBid, (key) => key.Bid_Id);
        }
      );
    };

    websocket.onerror = (error) => {
      console.error("WebSocket error:", error);
    };

    websocket.onclose = (event) => {
      console.log("WebSocket closed:", event);
      websocketRef.current = null;
    };

    return () => {
      websocket.close();
    };
  }, [isEnabled, queryClient]);

  // Cleanup WebSocket on component unmount
  useEffect(() => {
    return () => {
      if (websocketRef.current) {
        websocketRef.current.close();
      }
    };
  }, []);
};

export default useAllAuctionWithBidsWebSocket;
