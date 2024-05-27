import type { AuctionBid } from "@/models/AuctionBid";
import axios from "axios";
import { defineStore } from "pinia";
import { ref } from "vue";
import { webSocketService } from "@/lib/webSocket";
import type { AuctionCreator } from "@/models/AuctionCreator";
import type { AuctionMessage } from "@/models/AuctionMessage";
import type { AuctionBidsMessage } from "@/models/AuctionBidsMessage";
import type { AuctionMessageDto } from "@/models/AuctionMessageDto";
import { normalizeAuctionMessages } from "@/lib/normalizeAuctionMessages";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useAuctionStore = defineStore("auction", () => {
  const loading = ref(false);
  const error = ref<string | null>(null);
  const auctionMessages = ref<AuctionMessage[]>([]);
  const auctionMessagesFromREST = ref<AuctionMessageDto[]>([]);
  const auctionBidsMessages = ref<AuctionBidsMessage[]>([]);

  const createTables = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post(`${baseUrl}/api/auction/create_tables`);
      return response.data;
    } catch (err) {
      error.value = "Failed to create tables";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const createStreams = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post(
        `${baseUrl}/api/auction/create_streams`
      );
      return response.data;
    } catch (err) {
      error.value = "Failed to create streams";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const insertAuction = async (auction: AuctionCreator) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post(
        `${baseUrl}/api/auction/insert_auction`,
        auction
      );
      return response.data;
    } catch (err) {
      if (axios.isAxiosError(err) && err.response) {
        error.value = err.response.data;
      } else {
        error.value = "Failed to insert auction bid";
      }
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const insertAuctionBid = async (bid: AuctionBid) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.post(
        `${baseUrl}/api/auction/insert_auction_bid`,
        bid
      );
      return response.data;
    } catch (err) {
      if (axios.isAxiosError(err) && err.response) {
        error.value = err.response.data;
      } else {
        error.value = "Failed to insert auction bid";
      }
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const dropTables = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.delete(`${baseUrl}/api/auction/drop_tables`);
      return response.data;
    } catch (err) {
      error.value = "Failed to drop tables";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  function connectAuctionOverviewWebSocket(onError: (err: string) => void) {
    webSocketService.connectAuctionOverview((data) => {
      auctionMessages.value.push(data);
    }, onError);
  }

  const getAllTables = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(
        `${baseUrl}/api/auction/get_all_auctions`
      );
      const normalizedData = response.data.map((dto: AuctionMessageDto) =>
        normalizeAuctionMessages(dto)
      );
      auctionMessages.value = normalizedData;
    } catch (err) {
      error.value = "Failed to retrieve tables";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  function connectBidsWebSocket(
    auctionId: string,
    onFirstMessage: () => void,
    onError: (err: string) => void
  ) {
    webSocketService.connectBids(
      auctionId,
      (data) => {
        if (auctionBidsMessages.value.length === 0) {
          onFirstMessage();
        }
        auctionBidsMessages.value.push(data);
      },
      onError
    );
  }

  function disconnectAuctionOverviewWebSocket() {
    webSocketService.disconnectAuctionOverview();
  }

  function disconnectBidsWebSocket() {
    webSocketService.disconnectBids();
  }

  return {
    loading,
    error,
    createTables,
    createStreams,
    insertAuction,
    insertAuctionBid,
    dropTables,
    auctionMessages,
    auctionBidsMessages,
    connectAuctionOverviewWebSocket,
    connectBidsWebSocket,
    disconnectAuctionOverviewWebSocket,
    disconnectBidsWebSocket,
    getAllTables,
    auctionMessagesFromREST,
  };
});

export const useOverviewStore = defineStore("overview", () => {
  const loading = ref(false);
  const error = ref<string | null>(null);

  const checkTables = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(`${baseUrl}/api/overview/check_tables`);
      return response.data;
    } catch (err) {
      error.value = "Failed to check tables";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const checkStreams = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(`${baseUrl}/api/overview/check_streams`);
      return response.data;
    } catch (err) {
      error.value = "Failed to check streams";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const dropTable = async (tableName: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.delete(
        `${baseUrl}/api/overview/drop_table`,
        {
          params: { tableName },
        }
      );
      return response.data;
    } catch (err) {
      error.value = "Failed to drop table";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const dropStream = async (streamName: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.delete(
        `${baseUrl}/api/overview/drop_stream`,
        {
          params: { streamName },
        }
      );
      return response.data;
    } catch (err) {
      error.value = "Failed to drop stream";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  return {
    loading,
    error,
    checkTables,
    checkStreams,
    dropTable,
    dropStream,
  };
});
