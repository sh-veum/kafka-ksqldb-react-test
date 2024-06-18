import { baseUrl } from "@/lib/baseUrls";
import { ChatMessage } from "@/models/ChatMessage";
import { ChatMessageCreator } from "@/models/ChatMessageCreator";
import { useMutation } from "@tanstack/react-query";
import axios from "axios";

export const useInsertChatMessageMutation = (
  chatMessage: ChatMessageCreator
) => {
  return useMutation<ChatMessage, Error, ChatMessageCreator>({
    mutationFn: async () => {
      const response = await axios.post(
        `${baseUrl}/api/chat/insert_message`,
        chatMessage
      );
      return response.data as ChatMessage;
    },
  });
};
