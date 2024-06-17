import { baseUrl } from "@/lib/baseUrls";
import { queryOptions } from "@tanstack/react-query";
import axios from "axios";

export const auctionQueryOptions = (auctionId: string) =>
  queryOptions({
    queryKey: ["auction", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_auction_by_id?auction_Id=${auctionId}`
      );
      return res.data;
    },
  });

export const allAuctionQueryOptions = () =>
  queryOptions({
    queryKey: ["allAuctions"],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_all_auctions?sortByDate=true`
      );
      return res.data;
    },
  });

export const auctionBidsQueryOptions = (auctionId: string) =>
  queryOptions({
    queryKey: ["auctionBids", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_bid_messages_for_auction?auction_Id=${auctionId}`
      );
      return res.data;
    },
  });

export const chatMessagesQueryOptions = (auctionId: string) =>
  queryOptions({
    queryKey: ["chatMessages", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/chat/get_messages_for_auction?auction_Id=${auctionId}&sortByDate=true`
      );
      return res.data;
    },
  });
