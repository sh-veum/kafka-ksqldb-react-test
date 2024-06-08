import { WebSocketSubscription } from "@/Enums/webSocketSubscription";

const baseUrl = `${import.meta.env.VITE_API_WEBSOCKET_URL}`;

class WebSocketService {
  private auctionOverviewSocket: WebSocket | null = null;
  private bidSocket: WebSocket | null = null;
  private chatSocket: WebSocket | null = null;
  private allRecentBidsSocket: WebSocket | null = null;

  connectAuctionOverview(
    onMessage: (data: any) => void,
    onError: (err: string) => void
  ) {
    if (this.auctionOverviewSocket) {
      this.auctionOverviewSocket.close();
    }

    this.auctionOverviewSocket = new WebSocket(
      `${baseUrl}?auctionId=none&webSocketSubscription=${WebSocketSubscription.AuctionOverview}`
    );

    this.auctionOverviewSocket.onopen = () => {
      console.log("Auction overview WebSocket connection opened");
    };

    this.auctionOverviewSocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Auction overview WebSocket message received:", data);
      onMessage(data);
    };

    this.auctionOverviewSocket.onclose = () => {
      console.log("Auction overview WebSocket connection closed");
    };

    this.auctionOverviewSocket.onerror = (error) => {
      console.error("Auction overview WebSocket error:", error);
    };

    this.auctionOverviewSocket.onerror = (error) => {
      console.error("Auction overview WebSocket error:", error);
      onError("Failed to connect to the WebSocket.");
    };
  }

  connectBids(
    auctionId: string,
    onMessage: (data: any) => void,
    onError: (err: string) => void
  ) {
    if (this.bidSocket) {
      this.bidSocket.close();
    }

    this.bidSocket = new WebSocket(
      `${baseUrl}?auctionId=${auctionId}&webSocketSubscription=${WebSocketSubscription.SpesificAuction}`
    );

    this.bidSocket.onopen = () => {
      console.log("Bid WebSocket connection opened");
    };

    this.bidSocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Bid WebSocket message received:", data);
      onMessage(data);
    };

    this.bidSocket.onclose = () => {
      console.log("Bid WebSocket connection closed");
    };

    this.bidSocket.onerror = (error) => {
      console.error("Bid WebSocket error:", error);
      onError("Failed to connect to the WebSocket.");
    };
  }

  connectChat(
    auctionId: string,
    onMessage: (data: any) => void,
    onError: (err: string) => void
  ) {
    if (this.chatSocket) {
      this.chatSocket.close();
    }

    this.chatSocket = new WebSocket(
      `${baseUrl}?auctionId=${auctionId}&webSocketSubscription=${WebSocketSubscription.Chat}`
    );

    this.chatSocket.onopen = () => {
      console.log("Chat WebSocket connection opened");
    };

    this.chatSocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Chat WebSocket message received:", data);
      onMessage(data);
    };

    this.chatSocket.onclose = () => {
      console.log("Chat WebSocket connection closed");
    };

    this.chatSocket.onerror = (error) => {
      console.error("Chat WebSocket error:", error);
      onError("Failed to connect to the WebSocket.");
    };
  }

  connectAllRecentBids(
    onMessage: (data: any) => void,
    onError: (err: string) => void
  ) {
    if (this.allRecentBidsSocket) {
      this.allRecentBidsSocket.close();
    }

    this.allRecentBidsSocket = new WebSocket(
      `${baseUrl}?auctionId=none&webSocketSubscription=${WebSocketSubscription.AllRecentBids}`
    );

    this.allRecentBidsSocket.onopen = () => {
      console.log("All recent bids WebSocket connection opened");
    };

    this.allRecentBidsSocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("All recent bids WebSocket message received:", data);
      onMessage(data);
    };

    this.allRecentBidsSocket.onclose = () => {
      console.log("All recent bids WebSocket connection closed");
    };

    this.allRecentBidsSocket.onerror = (error) => {
      console.error("All recent bids WebSocket error:", error);
    };

    this.allRecentBidsSocket.onerror = (error) => {
      console.error("All recent bids WebSocket error:", error);
      onError("Failed to connect to the WebSocket.");
    };
  }

  connectUserLocation(
    page: string,
    onMessage: (data: any) => void,
    onError: (err: string) => void
  ) {
    if (this.chatSocket) {
      this.chatSocket.close();
    }

    this.chatSocket = new WebSocket(`${baseUrl}?page=${page}`);

    this.chatSocket.onopen = () => {
      console.log("User location WebSocket connection opened");
    };

    this.chatSocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("User location WebSocket message received:", data);
      onMessage(data);
    };

    this.chatSocket.onclose = () => {
      console.log("User location WebSocket connection closed");
    };

    this.chatSocket.onerror = (error) => {
      console.error("User location WebSocket error:", error);
      onError("Failed to connect to the WebSocket.");
    };
  }

  disconnectAuctionOverview() {
    if (this.auctionOverviewSocket) {
      this.auctionOverviewSocket.close();
    }
  }

  disconnectBids() {
    if (this.bidSocket) {
      this.bidSocket.close();
    }
  }

  disconnectChat() {
    if (this.chatSocket) {
      this.chatSocket.close();
    }
  }

  disconnectAllRecentBids() {
    if (this.allRecentBidsSocket) {
      this.allRecentBidsSocket.close();
    }
  }

  disconnectUserLocation() {
    if (this.chatSocket) {
      this.chatSocket.close();
    }
  }
}

export const webSocketService = new WebSocketService();
