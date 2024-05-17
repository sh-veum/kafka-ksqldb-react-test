class WebSocketService {
  private socket: WebSocket | null = null;

  connect(auctionId: string, onMessage: (data: any) => void) {
    if (this.socket) {
      this.socket.close();
    }

    this.socket = new WebSocket(
      `${import.meta.env.VITE_API_WEBSOCKET_URL}?auctionId=${auctionId}`
    );

    this.socket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("WebSocket message received:", data);
      onMessage(data);
    };

    this.socket.onclose = () => {
      console.log("WebSocket connection closed");
    };

    this.socket.onerror = (error) => {
      console.error("WebSocket error:", error);
    };
  }

  disconnect() {
    if (this.socket) {
      this.socket.close();
    }
  }
}

export const webSocketService = new WebSocketService();
