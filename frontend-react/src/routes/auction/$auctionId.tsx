import BidsTable from "@/components/BidsTable";
import {
  auctionBidsQueryOptions,
  auctionQueryOptions,
} from "@/utils/queryOptions";
import useBidsWebSocket from "@/utils/webSocket/useBidsWebSocket";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";

export const Route = createFileRoute("/auction/$auctionId")({
  parseParams: (params) => ({
    auctionId: params.auctionId,
  }),
  stringifyParams: ({ auctionId }) => ({ auctionId: `${auctionId}` }),
  loader: (opts) => {
    opts.context.queryClient.ensureQueryData(
      auctionQueryOptions(opts.params.auctionId)
    );
    opts.context.queryClient.ensureQueryData(
      auctionBidsQueryOptions(opts.params.auctionId)
    );
  },
  component: SpecificAuctionComponent,
});

function SpecificAuctionComponent() {
  const params = Route.useParams();
  const auctionQuery = useSuspenseQuery(auctionQueryOptions(params.auctionId));
  const auction = auctionQuery.data;
  const bidsQuery = useSuspenseQuery(auctionBidsQueryOptions(params.auctionId));
  const bids = bidsQuery.data;

  const [isDataLoaded, setIsDataLoaded] = useState(false);

  useEffect(() => {
    if (auction && bids) {
      setIsDataLoaded(true);
    }
  }, [auction, bids]);

  useBidsWebSocket(params.auctionId, isDataLoaded);

  return (
    <div className="max-w-full">
      <h1 className="text-2xl font-bold">{auction.Title}</h1>
      <p className="italic break-words">{auction.Description}</p>
      <BidsTable bids={bids} />
    </div>
  );
}

export default SpecificAuctionComponent;
