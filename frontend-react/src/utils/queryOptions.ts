import { baseUrl } from "@/lib/baseUrls";
import { Auction } from "@/models/Auction";
import { Bid } from "@/models/Bid";
import { ChatMessage } from "@/models/ChatMessage";
import { queryOptions } from "@tanstack/react-query";
import axios from "axios";
import { convertUTCToLocalTime } from "./timeUtils";
import { UserInfo } from "@/models/UserInfo";

export const auctionQueryOptions = (auctionId: string) =>
  queryOptions({
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
  queryOptions({
    queryKey: ["allAuctions"],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_all_auctions?sortByDate=true`
      );
      const auctions = res.data as Auction[];
      auctions.forEach((auction) => {
        auction.Created_At = convertUTCToLocalTime(auction.Created_At);
        auction.End_Date = convertUTCToLocalTime(auction.End_Date);
      });
      console.log("auctions", auctions);
      return auctions;
    },
  });

export const auctionBidsQueryOptions = (auctionId: string) =>
  queryOptions({
    queryKey: ["auctionBids", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/auction/get_bid_messages_for_auction?auction_Id=${auctionId}`
      );
      const bids = res.data as Bid[];
      bids.forEach((bid) => {
        bid.Timestamp = convertUTCToLocalTime(bid.Timestamp);
      });

      console.log("bids", bids);
      return bids;
    },
  });

export const chatMessagesQueryOptions = (auctionId: string) =>
  queryOptions({
    queryKey: ["chatMessages", auctionId],
    queryFn: async () => {
      const res = await axios.get(
        `${baseUrl}/api/chat/get_messages_for_auction?auction_Id=${auctionId}&sortByDate=true`
      );
      const chatMessages = res.data as ChatMessage[];
      chatMessages.forEach((message) => {
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
