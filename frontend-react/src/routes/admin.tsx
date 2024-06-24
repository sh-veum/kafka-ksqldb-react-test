import { useEffect, useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute, redirect } from "@tanstack/react-router";
import { getAllBidsQueryOptions } from "@/utils/queryOptions";
import { isAdmin } from "@/lib/isAdmin";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
} from "recharts";
import { AuctionWithBid } from "@/models/AuctionWithBid";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import useAllAuctionWithBidsWebSocket from "@/utils/web-socket/useOverviewWebSocket";
import { ScrollArea } from "@radix-ui/react-scroll-area";
import { formatCurrency } from "@/utils/currencyFormatter";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

export const Route = createFileRoute("/admin")({
  beforeLoad: async ({ location }) => {
    if (!isAdmin()) {
      throw redirect({
        to: "/",
        search: {
          redirect: location.href,
        },
      });
    }
  },
  loader: (opts) => {
    opts.context.queryClient.ensureQueryData(getAllBidsQueryOptions());
  },
  component: AdminPage,
});

interface TooltipProps {
  active?: boolean;
  payload?: {
    payload: {
      Bid_Amount: number;
      Username: string;
      Timestamp: string;
    };
  }[];
}

function AdminPage() {
  const auctionWithBidsQuery = useSuspenseQuery(getAllBidsQueryOptions());
  const auctionWithBids = auctionWithBidsQuery.data;
  const reversedAuctionsWithBids = auctionWithBids?.slice().reverse();

  const [isDataLoaded, setIsDataLoaded] = useState(false);
  const [selectedTitle, setSelectedTitle] = useState<string>(
    auctionWithBids[0].Title
  );

  useEffect(() => {
    if (auctionWithBids && !isDataLoaded) {
      console.log("Data loaded");
      setIsDataLoaded(true);
    }
  }, [auctionWithBids, isDataLoaded]);

  useAllAuctionWithBidsWebSocket(isDataLoaded);

  // Group bids by title
  const bidsByTitle: { [key: string]: AuctionWithBid[] } = auctionWithBids
    ? auctionWithBids.reduce(
        (acc: { [key: string]: AuctionWithBid[] }, bid: AuctionWithBid) => {
          if (!acc[bid.Title]) {
            acc[bid.Title] = [];
          }
          acc[bid.Title].push(bid);
          return acc;
        },
        {}
      )
    : {};

  // Custom tooltip to show username and bid amount
  const CustomTooltip = ({ active = false, payload = [] }: TooltipProps) => {
    if (active && payload && payload.length) {
      const { Bid_Amount, Username, Timestamp } = payload[0].payload;
      return (
        <Card>
          <CardHeader>
            <CardTitle>{Username}</CardTitle>
            <CardDescription>{Timestamp}</CardDescription>
          </CardHeader>
          <CardContent>Bid Amount: ${Bid_Amount}</CardContent>
        </Card>
      );
    }
    return null;
  };

  return (
    <div>
      <p className="text-2xl">Admin Page</p>
      <div className="my-2">
        <Select onValueChange={setSelectedTitle} defaultValue={selectedTitle}>
          <SelectTrigger className="w-[280px]">
            <SelectValue placeholder="Select an auction title" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              <SelectLabel>Auction Titles</SelectLabel>
              {Object.keys(bidsByTitle).map((title) => (
                <SelectItem key={title} value={title}>
                  {title}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>
      </div>
      <div className="flex w-screen space-x-4">
        <div>
          {selectedTitle && bidsByTitle[selectedTitle] && (
            <div>
              <LineChart
                width={700}
                height={700}
                data={bidsByTitle[selectedTitle]}
                margin={{ top: 5, right: 20, bottom: 5, left: 0 }}
              >
                <Line type="monotone" dataKey="Bid_Amount" stroke="#8884d8" />
                <CartesianGrid stroke="#ccc" strokeDasharray="5 5" />
                <XAxis dataKey="Timestamp" />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
              </LineChart>
            </div>
          )}
        </div>
        <div>
          <p className="text-xl mb-2">All Bids Live Feed</p>
          <ScrollArea className="max-h-[650px] min-w-max rounded-md border p-4 flex-1 overflow-auto">
            <ul>
              {reversedAuctionsWithBids &&
                reversedAuctionsWithBids.map((auction, index) => (
                  <li
                    key={auction.Bid_Id}
                    className={index % 2 === 0 ? "bg-secondary p-1" : "p-1"}
                  >
                    {auction.Timestamp} - {auction.Username} bid
                    {" " + formatCurrency(auction.Bid_Amount)} on{" "}
                    {auction.Title}
                  </li>
                ))}
            </ul>
          </ScrollArea>
        </div>
      </div>
    </div>
  );
}

export default AdminPage;
