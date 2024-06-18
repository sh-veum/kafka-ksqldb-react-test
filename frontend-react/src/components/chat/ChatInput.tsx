import { Input } from "@/components/ui/input";
import { useForm } from "@tanstack/react-form";
import { Button } from "@/components/ui/button";
import { useMutation } from "@tanstack/react-query";
import { ChatMessageCreator } from "@/models/ChatMessageCreator";
import axios from "axios";
import { baseUrl } from "@/lib/baseUrls";
import { Spinner } from "@/components/Spinner";
import { Alert, AlertDescription, AlertTitle } from "../ui/alert";
import { AlertCircle } from "lucide-react";

interface ChatInputProps {
  auctionId: string;
}

export function ChatInput({ auctionId }: ChatInputProps) {
  const mutation = useMutation({
    mutationFn: (chatMessage: ChatMessageCreator) => {
      return axios.post(`${baseUrl}/api/chat/insert_message`, chatMessage);
    },
  });

  const form = useForm({
    defaultValues: {
      Username: "",
      Message_Text: "",
    },
    onSubmit: (values) => {
      mutation.mutate({
        Auction_Id: auctionId,
        Username: values.value.Username,
        Message_Text: values.value.Message_Text,
      });
    },
  });

  if (mutation.isPending) {
    return <Spinner />;
  }

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
                    defaultValue={"Anon"}
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
              <Button
                type="submit"
                disabled={!canSubmit || isValidating}
                onClick={form.handleSubmit}
              >
                {mutation.isPending ? "Sending..." : "Send"}
              </Button>
            )}
          />
        </div>
        {mutation.isError && (
          <div className="text-red-500">{mutation.error.message}</div>
        )}
        {/* {mutation.isSuccess && (
          <div className="text-green-500">Message sent successfully!</div>
        )} */}
      </form>
    </>
  );
}
