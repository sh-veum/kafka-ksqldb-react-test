import { Input } from "@/components/ui/input";
import { useForm } from "@tanstack/react-form";
import { Button } from "@/components/ui/button";
import { Alert, AlertDescription, AlertTitle } from "../ui/alert";
import { AlertCircle } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { getUserInfoQueryOptions } from "@/utils/queryOptions";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { useInsertBidMutation } from "@/utils/mutations/auctionMutations";

interface BidInputProps {
  auctionId: string;
  isOpen: boolean;
}

export function BidInput({ auctionId, isOpen }: BidInputProps) {
  const { mutate, isPending } = useInsertBidMutation();
  const { data: userInfo } = useQuery(getUserInfoQueryOptions());

  const form = useForm({
    defaultValues: {
      Bid_Amount: 0,
    },
    onSubmit: (values) => {
      mutate({
        Auction_Id: auctionId,
        Username: userInfo?.username || "anon",
        Bid_Amount: values.value.Bid_Amount,
      });
    },
  });

  const isLoggedOut = userInfo?.status === "loggedOut";
  const canBid = isOpen && !isLoggedOut;
  const tooltipMessage = isLoggedOut
    ? "Only signed-in users can place bids"
    : !isOpen
      ? "Auction has ended"
      : "";

  return (
    <TooltipProvider>
      <form onSubmit={form.handleSubmit}>
        <div className="flex flex-1 space-x-2">
          <div className="w-full">
            <form.Field
              name="Bid_Amount"
              validators={{
                onChange: ({ value }) => {
                  if (value <= 0) {
                    return "Bid amount must be greater than zero";
                  }
                  if (isNaN(value)) {
                    return "Bid amount must be a number";
                  }
                  if (!/^\d+(\.\d{1,2})?$/.test(value.toString())) {
                    return "Bid amount cannot have more than two decimal places";
                  }
                  if (value > 9999999999999998n) {
                    return "Exceeded maximum bid amount";
                  }
                },
              }}
              children={(field) => (
                <div>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <div>
                        <Input
                          id="Bid_Amount"
                          placeholder="Enter your bid"
                          type="number"
                          step="0.25"
                          value={field.state.value}
                          onChange={(e) =>
                            field.handleChange(parseFloat(e.target.value))
                          }
                          required
                          disabled={!canBid}
                        />
                      </div>
                    </TooltipTrigger>
                    {tooltipMessage && (
                      <TooltipContent>
                        <p>{tooltipMessage}</p>
                      </TooltipContent>
                    )}
                  </Tooltip>
                  {field.state.meta.errors && (
                    <div className="text-red-500 text-sm mt-1">
                      {field.state.meta.errors}
                    </div>
                  )}
                </div>
              )}
            />
          </div>
          <form.Subscribe
            selector={(state) => state.errors}
            children={(errors) =>
              errors.length > 0 && (
                <Alert variant={"destructive"}>
                  <AlertCircle className="h-4 w-4" />
                  <AlertTitle>Error</AlertTitle>
                  <AlertDescription>{errors}</AlertDescription>
                </Alert>
              )
            }
          />
          <form.Subscribe
            selector={(state) => [state.canSubmit, state.isValidating]}
            children={() => (
              <div>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <div>
                      {isPending ? (
                        <Button
                          disabled={true}
                          className="font-bold text-foreground"
                        >
                          Sending...
                        </Button>
                      ) : (
                        <Button
                          type="submit"
                          disabled={!canBid}
                          onClick={form.handleSubmit}
                          className="font-bold text-foreground"
                        >
                          Bid
                        </Button>
                      )}
                    </div>
                  </TooltipTrigger>
                  {tooltipMessage && (
                    <TooltipContent>
                      <p>{tooltipMessage}</p>
                    </TooltipContent>
                  )}
                </Tooltip>
              </div>
            )}
          />
        </div>
      </form>
    </TooltipProvider>
  );
}
