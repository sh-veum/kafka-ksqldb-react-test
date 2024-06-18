import { ChatMessage } from "@/models/ChatMessage";
import { ColumnDef } from "@tanstack/react-table";
import { format, isToday, parseISO } from "date-fns";

export const chatColumns: ColumnDef<ChatMessage>[] = [
  {
    accessorKey: "Message",
    header: "Message",
    cell: ({ row }) => {
      const username = row.original.Username as string;
      const timestamp = row.original.Created_Timestamp as string;
      const message = row.original.Message_Text as string;
      const isEdited = row.original.Is_Edited as boolean;

      const date = parseISO(timestamp);

      let formattedDate;
      if (isToday(date)) {
        formattedDate = format(date, "HH:mm:ss");
      } else {
        formattedDate = format(date, "MMMM d, yyyy 'at' HH:mm");
      }
      return (
        <div className="flex flex-col">
          <span className="text-ring font-bold">
            {username}{" "}
            <span className="text-gray-500 font-normal text-xs">
              {formattedDate}
            </span>
          </span>
          <span>
            {message}
            {isEdited && (
              <span className="text-gray-500 text-sm ml-1">(edited)</span>
            )}
          </span>
        </div>
      );
    },
    sortingFn: (rowA, rowB) => {
      const dateA = new Date(rowA.original.Created_Timestamp).getTime();
      const dateB = new Date(rowB.original.Created_Timestamp).getTime();
      return dateA - dateB;
    },
  },
  //   {
  //     accessorKey: "Message_Text",
  //     header: "Message",
  //     cell: ({ row }) => {
  //       const message = row.getValue("Message_Text") as string;
  //       const isEdited = row.original.Is_Edited as boolean;

  //       return (
  //         <div>
  //           {message} {isEdited && <span className="italic">(edited)</span>}
  //         </div>
  //       );
  //     },
  //     footer: (info) => info.column.id,
  //     minSize: 200,
  //     size: Number.MAX_SAFE_INTEGER,
  //     maxSize: 300,
  //   },
];
