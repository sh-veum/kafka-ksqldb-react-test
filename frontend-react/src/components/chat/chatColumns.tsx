import { ChatMessage } from "@/models/ChatMessage";
import { ColumnDef } from "@tanstack/react-table";
import { format, isToday, parseISO } from "date-fns";

export const chatColumns: ColumnDef<ChatMessage>[] = [
  {
    accessorKey: "Username",
    header: "Username",
    cell: ({ row }) => {
      const username = row.getValue("Username") as string;
      const timestamp = row.original.Created_Timestamp as string;
      const date = parseISO(timestamp);

      let formattedDate;
      if (isToday(date)) {
        formattedDate = format(date, "HH:mm:ss");
      } else {
        formattedDate = format(date, "MMMM d, yyyy 'at' HH:mm");
      }
      return (
        <div>
          <div className="text-xs italic color">{formattedDate}</div>
          <div className="text-base">{username}</div>
        </div>
      );
    },
    sortingFn: (rowA, rowB) => {
      const dateA = new Date(rowA.original.Created_Timestamp).getTime();
      const dateB = new Date(rowB.original.Created_Timestamp).getTime();
      return dateA - dateB;
    },
    minSize: 50,
    size: 50,
    maxSize: 200,
  },
  {
    accessorKey: "Message_Text",
    header: "Message",
    cell: ({ row }) => {
      const message = row.getValue("Message_Text") as string;
      const isEdited = row.original.Is_Edited as boolean;

      return (
        <div>
          {message} {isEdited && <span className="italic">(edited)</span>}
        </div>
      );
    },
    footer: (info) => info.column.id,
    minSize: 200,
    size: 200,
    maxSize: 300,
  },
];
