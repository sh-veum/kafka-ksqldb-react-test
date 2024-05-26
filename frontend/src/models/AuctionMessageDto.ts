export interface AuctionMessageDto {
  auction_Id: string;
  title: string;
  description: string;
  starting_Price: number;
  current_Price: number;
  number_Of_Bids: number;
  winner: string;
  created_At: string;
}
