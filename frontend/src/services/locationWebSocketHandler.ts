import { webSocketService } from "@/lib/webSocket";
import type { Ref } from "vue";

export class LocationWebSocketHandler {
  private usersOnPage: Ref<string[]>;

  constructor(usersOnPage: Ref<string[]>) {
    this.usersOnPage = usersOnPage;
  }

  connectUserLocationWebSocket(page: string, onError: (err: string) => void) {
    webSocketService.connectUserLocation(
      page,
      (data) => {
        this.usersOnPage.value.push(data);
      },
      onError
    );
  }

  disconnectUserLocationWebSocket() {
    webSocketService.disconnectChat();
  }
}
