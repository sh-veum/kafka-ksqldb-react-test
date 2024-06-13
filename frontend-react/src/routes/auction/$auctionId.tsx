import { auctionQueryOptions } from "@/utils/queryOptions";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/auction/$auctionId")({
  parseParams: (params) => ({
    auctionId: params.auctionId,
  }),
  stringifyParams: ({ auctionId }) => ({ auctionId: `${auctionId}` }),
  loader: (opts) =>
    opts.context.queryClient.ensureQueryData(
      auctionQueryOptions(opts.params.auctionId)
    ),
  component: SpecificAuctionComponent,
});

function SpecificAuctionComponent() {
  const params = Route.useParams();
  const auctionQuery = useSuspenseQuery(auctionQueryOptions(params.auctionId));
  const auction = auctionQuery.data;

  return (
    <div>
      <pre className="text-sm whitespace-pre-wrap">
        {JSON.stringify(auction, null, 2)}
      </pre>
    </div>
  );
}
