import { baseUrl } from "@/lib/baseUrls";
import { ChatMessage } from "@/models/ChatMessage";
import { ChatMessageCreator } from "@/models/ChatMessageCreator";
import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import { toast } from "sonner";

export const useInsertChatMessageMutation = () => {
  const toastId = "insertChatMessageMutation";

  const mutation = useMutation<ChatMessage, Error, ChatMessageCreator>({
    mutationFn: async (chatMessage: ChatMessageCreator) => {
      const response = await axios.post(
        `${baseUrl}/api/chat/insert_message`,
        chatMessage
      );
      return response.data as ChatMessage;
    },
  });

  if (mutation.isPending && !mutation.isError && !mutation.isSuccess) {
    toast.loading("Sending chat message...", {
      id: toastId,
      closeButton: true,
    });
  }

  if (mutation.isSuccess) {
    toast.success("Chat message sent!", { id: toastId, closeButton: true });
  }

  if (mutation.isError) {
    toast.error(`Error! Failed to add message!`, {
      id: toastId,
      closeButton: true,
      description: mutation.error?.message,
    });
  }

  return mutation;
};
