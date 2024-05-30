import type { AuctionBid } from "@/models/AuctionBid";
import axios from "axios";
import { defineStore } from "pinia";
import { ref } from "vue";
import { webSocketService } from "@/lib/webSocket";
import type { AuctionCreator } from "@/models/AuctionCreator";
import type { AuctionMessage } from "@/models/AuctionMessage";
import type { AuctionBidsMessage } from "@/models/AuctionBidsMessage";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useAuctionStore = defineStore("auction", () => {
  // is this even used?
  const loading = ref(false);
  const error = ref<string | null>(null);
  const auctionMessages = ref<AuctionMessage[]>([]);
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

  const getAllTables = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(
        `${baseUrl}/api/auction/get_all_auctions?sortByDate=true`
      );
      auctionMessages.value = response.data;
    } catch (err) {
      error.value = "Failed to retrieve tables";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const getAuctionBidsForAuction = async (
    auctionId: string
  ): Promise<boolean> => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(
        `${baseUrl}/api/auction/get_bid_messages_for_auction?auction_Id=${auctionId}`
      );
      auctionBidsMessages.value = response.data;
      console.log(auctionBidsMessages.value);
      return true;
    } catch (err) {
      error.value = "Failed to retrieve auction bids";
      console.error(err);
      return false;
    } finally {
      loading.value = false;
    }
  };

  function connectAuctionOverviewWebSocket(onError: (err: string) => void) {
    webSocketService.connectAuctionOverview((data) => {
      auctionMessages.value.push(data);
    }, onError);
  }

  function connectBidsWebSocket(
    auctionId: string,
    onError: (err: string) => void
  ) {
    webSocketService.connectBids(
      auctionId,
      (data) => {
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
    getAuctionBidsForAuction,
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
