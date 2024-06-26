import { baseUrl } from "@/lib/baseUrls";
import { Auction } from "@/models/Auction";
import { Bid } from "@/models/Bid";
import { ChatMessage } from "@/models/ChatMessage";
import { queryOptions } from "@tanstack/react-query";
import axios from "axios";
import { convertUTCToLocalTime } from "./timeUtils";
import { UserInfo } from "@/models/UserInfo";
import { AuctionWithBid } from "@/models/AuctionWithBid";

export const auctionQueryOptions = (auctionId: string) =>
  queryOptions<Auction>({
    queryKey: ["auction", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_auction_by_id?auction_Id=${auctionId}`
      );
      const auction = res.data as Auction;
      auction.Created_At = convertUTCToLocalTime(auction.Created_At);
      auction.End_Date = convertUTCToLocalTime(auction.End_Date);

      console.log("auction", auction);
      return auction;
    },
  });

export const allAuctionQueryOptions = () =>
  queryOptions<Auction[]>({
    queryKey: ["allAuctions"],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_all_auctions?sortByDate=true`
      );
      const auctions = res.data;
      auctions.forEach((auction: Auction) => {
        auction.Created_At = convertUTCToLocalTime(auction.Created_At);
        auction.End_Date = convertUTCToLocalTime(auction.End_Date);
      });
      console.log("auctions", auctions);
      return auctions;
    },
  });

export const auctionBidsQueryOptions = (auctionId: string) =>
  queryOptions<Bid[]>({
    queryKey: ["auctionBids", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_bid_messages_for_auction?auction_Id=${auctionId}`
      );
      const bids = res.data;
      bids.forEach((bid: Bid) => {
        bid.Timestamp = convertUTCToLocalTime(bid.Timestamp);
      });

      console.log("bids", bids);
      return bids;
    },
  });

export const chatMessagesQueryOptions = (auctionId: string) =>
  queryOptions<ChatMessage[]>({
    queryKey: ["chatMessages", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/chat/get_messages_for_auction?auction_Id=${auctionId}&sortByDate=true`
      );
      const chatMessages = res.data;
      chatMessages.forEach((message: ChatMessage) => {
        message.Created_Timestamp = convertUTCToLocalTime(
          message.Created_Timestamp
        );
      });

      console.log("chatMessages", chatMessages);
      return chatMessages;
    },
  });

export const getUserInfoQueryOptions = () =>
  queryOptions<UserInfo>({
    queryKey: ["auth", "loginInfo"],
    queryFn: async () => {
      const username = localStorage.getItem("username");
      const role = localStorage.getItem("role");
      const accessToken = localStorage.getItem("accessToken");
      const refreshToken = localStorage.getItem("refreshToken");
      if (accessToken && refreshToken && username && role) {
        return {
          status: "loggedIn",
          username: username,
          role: role,
        };
      }
      return {
        status: "loggedOut",
        username: "",
        role: "",
      };
    },
  });

export const getAllBidsQueryOptions = () =>
  queryOptions<AuctionWithBid[]>({
    queryKey: ["allAuctionsWithBids"],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_all_auctions_with_bids?sortByDate=true`
      );
      const auctionWithBids = res.data;
      auctionWithBids.forEach((message: AuctionWithBid) => {
        message.Timestamp = convertUTCToLocalTime(message.Timestamp);
      });

      console.log("Auction With Bid", auctionWithBids);
      return auctionWithBids;
    },
  });
