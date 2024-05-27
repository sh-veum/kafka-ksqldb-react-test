import type { AuctionMessage } from "@/models/AuctionMessage";
import type { AuctionMessageDto } from "@/models/AuctionMessageDto";

export function normalizeAuctionMessages(
  dto: AuctionMessageDto
): AuctionMessage {
  return {
    Auction_Id: dto.auction_Id,
    Title: dto.title,
    Description: dto.description,
    Starting_Price: dto.starting_Price,
    Current_Price: dto.current_Price,
    Number_Of_Bids: dto.number_Of_Bids,
    Winner: dto.winner,
    Created_At: dto.created_At,
  };
}
