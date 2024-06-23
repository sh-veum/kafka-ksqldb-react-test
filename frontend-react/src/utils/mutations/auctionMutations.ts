import { baseUrl } from "@/lib/baseUrls";
import { Auction } from "@/models/Auction";
import { AuctionCreator } from "@/models/AuctionCreator";
import { Bid } from "@/models/Bid";
import { BidCreator } from "@/models/BidCreator";
import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import { toast } from "sonner";

export const useInsertAuctionMutation = () => {
  const toastId = "insertAuctionMutation";

  const mutation = useMutation<Auction, Error, AuctionCreator, unknown>({
    mutationFn: async (auction: AuctionCreator) => {
      const response = await axios.post(
        `${baseUrl}/api/auction/insert_auction`,
        auction
      );
      return response.data as Auction;
    },
  });

  if (mutation.isPending && !mutation.isError && !mutation.isSuccess) {
    toast.loading("Adding auction...", { id: toastId });
  }

  if (mutation.isSuccess) {
    toast.success("SUCCESS Auction added!", { id: toastId });
  }

  if (mutation.isError) {
    toast.error(`ERROR Failed to add auction!`, {
      id: toastId,
      description: mutation.error?.message,
    });
  }

  return mutation;
};

export const useInsertBidMutation = () => {
  const toastId = "insertBidMutation";

  const mutation = useMutation<Bid, Error, BidCreator, unknown>({
    mutationFn: async (bid: BidCreator) => {
      const response = await axios.post(
        `${baseUrl}/api/auction/insert_auction_bid`,
        bid
      );

      if (
        typeof response.data === "string" &&
        response.data.includes("Bid must be higher than")
      ) {
        throw new Error(response.data);
      }

      return response.data as Bid;
    },
  });

  if (mutation.isPending && !mutation.isError && !mutation.isSuccess) {
    toast.loading("Adding bid...", { id: toastId, description: null });
  }

  if (mutation.isSuccess) {
    toast.success("SUCCESS Bid added!", {
      id: toastId,
      description: `You bid: ${mutation.data?.Bid_Amount}`,
    });
  }

  if (mutation.isError) {
    toast.error(`ERROR Failed to add bid!`, {
      id: toastId,
      description: mutation.error?.message,
    });
  }

  return mutation;
};
