import { Auction } from "@/models/Auction";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "./ui/card";
import { Link } from "@tanstack/react-router";

export default function AuctionCard(auctionProps: Auction) {
  return (
    <>
      <Link to={`/auction/${auctionProps.Auction_Id}`}>
        <Card className="p-1 transition-colors hover:bg-accent hover:text-accent-foreground">
          <CardHeader className="mb-[-16px]">
            <CardTitle>{auctionProps.Title}</CardTitle>
            <CardDescription>{auctionProps.Created_At}</CardDescription>
          </CardHeader>
          <CardContent>
            <p className="mb-2">{auctionProps.Description}</p>
            <p>Current Price: {auctionProps.Current_Price} NOK</p>
            <p className="ml-4">
              <i>{auctionProps.Number_Of_Bids} bids</i>
            </p>
          </CardContent>
        </Card>
      </Link>
    </>
  );
}
