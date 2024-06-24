import { Input } from "@/components/ui/input";
import { useForm } from "@tanstack/react-form";
import { Button } from "@/components/ui/button";
import { Alert, AlertDescription, AlertTitle } from "../ui/alert";
import { AlertCircle } from "lucide-react";
import { useInsertChatMessageMutation } from "@/utils/mutations/chatMutations";
import { useQuery } from "@tanstack/react-query";
import { getUserInfoQueryOptions } from "@/utils/queryOptions";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";

interface ChatInputProps {
  auctionId: string;
  hasEnded: boolean;
}

export function ChatInput({ auctionId, hasEnded }: ChatInputProps) {
  const { mutate, isPending } = useInsertChatMessageMutation();
  const { data: userInfo } = useQuery(getUserInfoQueryOptions());

  const form = useForm({
    defaultValues: {
      Message_Text: "",
    },
    onSubmit: (values) => {
      mutate({
        ...values.value,
        Auction_Id: auctionId,
        Username: userInfo?.username || "anon",
      });
    },
  });

  const isLoggedOut = userInfo?.status === "loggedOut";

  return (
    <TooltipProvider>
      <form onSubmit={form.handleSubmit}>
        <div className="flex flex-1 space-x-2">
          <div className="w-full">
            <form.Field
              name="Message_Text"
              validators={{
                onChange: ({ value }) => {
                  if (value.length < 1) {
                    return "Message can't be empty";
                  }
                  if (value.length > 200) {
                    return "Message too long";
                  }
                },
              }}
              children={(field) => (
                <div>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <div>
                        <Input
                          id="Message"
                          placeholder="Message"
                          type="text"
                          value={field.state.value}
                          onChange={(e) => field.handleChange(e.target.value)}
                          required
                          disabled={isLoggedOut}
                        />
                      </div>
                    </TooltipTrigger>
                    {isLoggedOut && (
                      <TooltipContent>
                        <p>only signed-in user can chat</p>
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
            children={([canSubmit, isValidating]) => (
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
                          disabled={!canSubmit || isValidating || isLoggedOut}
                          onClick={form.handleSubmit}
                          className="font-bold text-foreground"
                        >
                          Send
                        </Button>
                      )}
                    </div>
                  </TooltipTrigger>
                  {isLoggedOut && (
                    <TooltipContent>
                      <p>only signed-in user can chat</p>
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
