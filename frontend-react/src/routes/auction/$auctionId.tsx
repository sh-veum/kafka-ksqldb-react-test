import { BidInput } from "@/components/bids/BidInput";
import { BidsTable } from "@/components/bids/BidsTable";
import { bidColumns } from "@/components/bids/bidColumns";
import { ChatInput } from "@/components/chat/ChatInput";
import { ChatTable } from "@/components/chat/ChatTable";
import { chatColumns } from "@/components/chat/chatColumns";
import { Button } from "@/components/ui/button";
import {
  auctionBidsQueryOptions,
  auctionQueryOptions,
  chatMessagesQueryOptions,
} from "@/utils/queryOptions";
import useBidsWebSocket from "@/utils/web-socket/useBidsWebSocket";
import useChatWebSocket from "@/utils/web-socket/useChatWebSocket";
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
  const [showDescription, setShowDescription] = useState(false);

  useEffect(() => {
    if (auction && bids && chatMessages && !isDataLoaded) {
      console.log("Data loaded");
      setIsDataLoaded(true);
    }
  }, [auction, bids, chatMessages, isDataLoaded]);

  useBidsWebSocket(params.auctionId, isDataLoaded);
  useChatWebSocket(params.auctionId, isDataLoaded);

  const handleShowDescription = () => {
    setShowDescription(!showDescription);
  };

  return (
    <div className="max-w-full">
      <h1 className="text-2xl font-bold">{auction.Title}</h1>
      <p className="text-gray-500 text-sm my-2">
        Starting bid: ${auction.Starting_Price}
      </p>
      <Button variant="outline" size="sm" onClick={handleShowDescription}>
        {showDescription ? "Hide description" : "Show description"}
      </Button>
      {showDescription && (
        <p className="italic break-words">{auction.Description}</p>
      )}
      <div className="flex flex-1 space-x-4 mt-2 w-dvw">
        <div className="max-w-prose w-2/3">
          <p className="text-xl font-bold">Bids</p>
          <div className="mt-2">
            <BidInput auctionId={params.auctionId} />
            <div className="mt-2">
              <BidsTable columns={bidColumns} data={bids} />
            </div>
          </div>
        </div>
        <div className="max-w-prose w-1/3">
          <p className="text-xl font-bold">Chat</p>
          <div className="mt-2">
            <ChatInput auctionId={params.auctionId} />
            <div className="mt-2">
              <ChatTable columns={chatColumns} data={chatMessages} />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default SpecificAuctionComponent;
