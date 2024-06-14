import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/auction/")({
  component: () => <div>Hello /auction/!</div>,
});
