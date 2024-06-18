import { AuctionCreator } from "@/models/AuctionCreator";
import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import { baseUrl } from "@/lib/baseUrls";
import { Auction } from "@/models/Auction";

export const useInsertAuctionMutation = (auction: AuctionCreator) => {
  return useMutation<Auction, Error, AuctionCreator>({
    mutationFn: async () => {
      const response = await axios.post(
        `${baseUrl}/api/auction/insert_auction`,
        auction
      );
      return response.data as Auction;
    },
  });
};
