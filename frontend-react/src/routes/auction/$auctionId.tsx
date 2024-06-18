import { BidsTable } from "@/components/bids/BidsTable";
import { bidColumns } from "@/components/bids/bidColumns";
import { ChatInput } from "@/components/chat/ChatInput";
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
      console.log("Data loaded");
      setIsDataLoaded(true);
    }
  }, [auction, bids, chatMessages, isDataLoaded]);

  useBidsWebSocket(params.auctionId, isDataLoaded);
  useChatWebSocket(params.auctionId, isDataLoaded);

  return (
    <div className="max-w-full">
      <h1 className="text-2xl font-bold">{auction.Title}</h1>
      <p className="italic break-words">{auction.Description}</p>
      <div className="flex flex-1 space-x-4 m-2">
        <div className="max-w-prose">
          <p className="text-xl font-bold">Bids</p>
          <BidsTable columns={bidColumns} data={bids} />
        </div>
        <div className="max-w-prose w-dvw">
          <p className="text-xl font-bold">Chat</p>
          <ChatInput auctionId={params.auctionId} />
          <div className="mt-2">
            <ChatTable columns={chatColumns} data={chatMessages} />
          </div>
        </div>
      </div>
    </div>
  );
}

export default SpecificAuctionComponent;
