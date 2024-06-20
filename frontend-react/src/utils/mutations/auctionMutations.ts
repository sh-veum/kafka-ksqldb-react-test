import { baseUrl } from "@/lib/baseUrls";
import { Auction } from "@/models/Auction";
import { AuctionCreator } from "@/models/AuctionCreator";
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

  //   console.log("mutation", mutation);

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
