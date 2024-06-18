import { AuctionCreator } from "@/models/AuctionCreator";
import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import { baseUrl } from "@/lib/baseUrls";

export const useInsertAuctionMutation = (auction: AuctionCreator) => {
  return useMutation<AuctionCreator, Error, AuctionCreator>({
    mutationFn: () => {
      return axios.post(`${baseUrl}/api/auction/insert_auction`, auction);
    },
  });
};
