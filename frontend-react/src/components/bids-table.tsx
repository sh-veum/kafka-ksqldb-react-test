import { Bid } from "@/models/Bid";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

interface BidsTableProps {
  bids: Bid[];
}

export default function BidsTable({ bids }: BidsTableProps) {
  return (
    <Table>
      <TableCaption>All bids</TableCaption>
      <TableHeader>
        <TableRow>
          <TableHead className="w-[100px]">Username</TableHead>
          <TableHead>Time</TableHead>
          <TableHead className="text-right">Amount</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {[...bids].reverse().map((bid, index) => (
          <TableRow key={index}>
            <TableCell className="font-medium">{bid.Username}</TableCell>
            <TableCell>{bid.Timestamp}</TableCell>
            <TableCell className="text-right">{bid.Bid_Amount}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}
