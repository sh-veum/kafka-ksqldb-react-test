import { isAdmin } from "@/lib/isAdmin";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/admin")({
  beforeLoad: async ({ location }) => {
    if (!isAdmin()) {
      throw redirect({
        to: "/",
        search: {
          // Use the current location to power a redirect after login
          // (Do not use `router.state.resolvedLocation` as it can
          // potentially lag behind the actual current location)
          redirect: location.href,
        },
      });
    }
  },
  component: AdminPage,
});

function AdminPage() {
  return (
    <div>
      <h1>Admin Page</h1>
    </div>
  );
}
