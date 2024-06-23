import { AddAuctionDialog } from "@/components/AddAuctionDialog";
import AuctionCard from "@/components/AuctionCard";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Auction } from "@/models/Auction";
import { allAuctionQueryOptions } from "@/utils/queryOptions";
import useAuctionsWebSocket from "@/utils/web-socket/useAuctionsWebSocket";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

export const Route = createFileRoute("/")({
  loader: (opts) =>
    opts.context.queryClient.ensureQueryData(allAuctionQueryOptions()),
  component: Index,
});

function Index() {
  const allAuctionsQuery = useSuspenseQuery(allAuctionQueryOptions());
  const allAuctions = allAuctionsQuery.data;
  const allAuctionsFetched = allAuctionsQuery.isFetchedAfterMount;

  const [isDataLoaded, setIsDataLoaded] = useState(false);
  const [sortOrder, setSortOrder] = useState("desc"); // Default sort order

  useEffect(() => {
    if (allAuctionsFetched) {
      setIsDataLoaded(true);
    }
  }, [allAuctionsFetched]);

  useAuctionsWebSocket(isDataLoaded);

  const sortedAuctions = [...allAuctions].sort((a, b) => {
    const dateA = new Date(a.Created_At).getTime();
    const dateB = new Date(b.Created_At).getTime();
    return sortOrder === "asc" ? dateA - dateB : dateB - dateA;
  });

  return (
    <div className="flex flex-col space-y-2">
      <div className="flex space-x-4">
        <AddAuctionDialog />
        <Select
          onValueChange={(value) => setSortOrder(value)}
          defaultValue={sortOrder}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Sort by Date" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              <SelectLabel>Sort Order</SelectLabel>
              <SelectItem value="asc">Oldest First</SelectItem>
              <SelectItem value="desc">Newest First</SelectItem>
            </SelectGroup>
          </SelectContent>
        </Select>
      </div>
      <ScrollArea className="max-h-[790px] min-w-max rounded-md border p-4 flex-1 overflow-auto">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {sortedAuctions.map((auction: Auction) => (
            <AuctionCard key={auction.Auction_Id} {...auction} />
          ))}
        </div>
      </ScrollArea>
    </div>
  );
}
