import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";

/**
 * Custom hook to manage WebSocket connection.
 *
 * @param {boolean} isEnabled - Whether the WebSocket should be enabled.
 * @param {() => string} getWebSocketUrl - A function to get the WebSocket URL.
 * @param {(event: MessageEvent) => void} onMessage - A callback function to handle incoming WebSocket messages.
 */
const useWebSocket = (
  isEnabled: boolean,
  getWebSocketUrl: () => string,
  onMessage: (event: MessageEvent) => void
) => {
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!isEnabled) return;

    const webSocketUrl = getWebSocketUrl();
    console.log("WebSocket URL:", webSocketUrl);
    const websocket = new WebSocket(webSocketUrl);

    websocket.onopen = () => {
      console.log("connected to websocket");
    };

    websocket.onmessage = (event) => {
      onMessage(event);
    };

    websocket.onerror = (error) => {
      console.error("WebSocket error:", error);
    };

    websocket.onclose = (event) => {
      console.log("WebSocket closed:", event);
    };

    return () => {
      websocket.close();
    };
  }, [queryClient, isEnabled, getWebSocketUrl, onMessage]);
};

export default useWebSocket;
