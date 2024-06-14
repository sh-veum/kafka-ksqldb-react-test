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
        <Card className="p-2">
          <CardHeader>
            <CardTitle>{auctionProps.Title}</CardTitle>
            <CardDescription>{auctionProps.Created_At}</CardDescription>
          </CardHeader>
          <CardContent>
            <p>
              <i>{auctionProps.Description}</i>
            </p>
            <p>{auctionProps.Number_Of_Bids} bids</p>
            <p>Current Price: {auctionProps.Current_Price} NOK</p>
          </CardContent>
        </Card>
      </Link>
    </>
  );
}
