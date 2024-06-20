import { Input } from "@/components/ui/input";
import { useForm } from "@tanstack/react-form";
import { Button } from "@/components/ui/button";
import { Alert, AlertDescription, AlertTitle } from "../ui/alert";
import { AlertCircle } from "lucide-react";
import { useInsertChatMessageMutation } from "@/utils/mutations/chatMutations";

interface ChatInputProps {
  auctionId: string;
}

export function ChatInput({ auctionId }: ChatInputProps) {
  const { mutate, isPending } = useInsertChatMessageMutation();

  const form = useForm({
    defaultValues: {
      Username: "",
      Message_Text: "",
    },
    onSubmit: (values) => {
      mutate({ ...values.value, Auction_Id: auctionId });
    },
  });

  return (
    <>
      <form>
        <div className="flex flex-1 space-x-2">
          <div className="w-1/3">
            <form.Field
              name="Username"
              validators={{
                onChange: ({ value }) => {
                  if (value.length < 1) {
                    return "Username can't be empty";
                  }
                  if (value.length > 20) {
                    return "Username too long";
                  }
                },
              }}
              children={(field) => (
                <div>
                  <Input
                    id="Username"
                    placeholder="Username"
                    type="text"
                    value={field.state.value}
                    onChange={(e) => field.handleChange(e.target.value)}
                    required
                  />
                  {field.state.meta.errors && (
                    <div className="text-red-500 text-sm mt-1">
                      {field.state.meta.errors}
                    </div>
                  )}
                </div>
              )}
            />
          </div>
          <div className="w-2/3">
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
                  <Input
                    id="Message"
                    placeholder="Message"
                    type="text"
                    value={field.state.value}
                    onChange={(e) => field.handleChange(e.target.value)}
                    required
                  />
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
                {isPending ? (
                  <Button disabled={true} className="font-bold text-foreground">
                    Sending...
                  </Button>
                ) : (
                  <Button
                    type="submit"
                    disabled={!canSubmit || isValidating}
                    onClick={form.handleSubmit}
                    className="font-bold text-foreground"
                  >
                    Send
                  </Button>
                )}
              </div>
            )}
          />
        </div>
      </form>
    </>
  );
}
