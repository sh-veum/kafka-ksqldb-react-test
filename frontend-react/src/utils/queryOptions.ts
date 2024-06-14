import { queryOptions } from "@tanstack/react-query";

const baseUrl = import.meta.env.VITE_API_URL;

export const auctionQueryOptions = (auctionId: string) =>
  queryOptions({
    queryKey: ["auction", auctionId],
    queryFn: async () => {
      const res = await fetch(
        `${baseUrl}/api/auction/get_auction_by_id?auction_Id=${auctionId}`
      );
      return res.json();
    },
  });

export const allAuctionQueryOptions = () =>
  queryOptions({
    queryKey: ["allAuctions"],
    queryFn: async () => {
      const res = await fetch(
        `${baseUrl}/api/auction/get_all_auctions?sortByDate=true`
      );
      return res.json();
    },
  });

export const auctionBidsQueryOptions = (auctionId: string) =>
  queryOptions({
    queryKey: ["auctionBids", auctionId],
    queryFn: async () => {
      const res = await fetch(
        `${baseUrl}/api/auction/get_bid_messages_for_auction?auction_Id=${auctionId}`
      );
      return res.json();
    },
  });
