import { BidsTable } from "@/components/bids/BidsTable";
import { bidColumns } from "@/components/bids/bidColumns";
import { ChatTable } from "@/components/chat/ChatTable";
import { chatColumns } from "@/components/chat/chatColumns";
import {
  auctionBidsQueryOptions,
  auctionQueryOptions,
  chatMessagesQueryOptions,
} from "@/utils/queryOptions";
import useBidsWebSocket from "@/utils/webSocket/useBidsWebSocket";
import useChatWebSocket from "@/utils/webSocket/useChatWebSocket";
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
    opts.context.queryClient.ensureQueryData(
      chatMessagesQueryOptions(opts.params.auctionId)
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
  const chatMessagesQuery = useSuspenseQuery(
    chatMessagesQueryOptions(params.auctionId)
  );
  const chatMessages = chatMessagesQuery.data;

  const [isDataLoaded, setIsDataLoaded] = useState(false);

  useEffect(() => {
    if (auction && bids && chatMessages && !isDataLoaded) {
      setIsDataLoaded(true);
    }
  }, [auction, bids, chatMessages, isDataLoaded]);

  useBidsWebSocket(params.auctionId, isDataLoaded);
  useChatWebSocket(params.auctionId, isDataLoaded);

  return (
    <div className="max-w-full">
      <h1 className="text-2xl font-bold">{auction.Title}</h1>
      <p className="italic break-words">{auction.Description}</p>
      <div className="flex flex-1">
        <BidsTable columns={bidColumns} data={bids} />
        <ChatTable columns={chatColumns} data={chatMessages} />
      </div>
    </div>
  );
}

export default SpecificAuctionComponent;
