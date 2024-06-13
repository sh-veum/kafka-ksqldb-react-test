import {
  createRootRouteWithContext,
  Link,
  Outlet,
  useRouterState,
} from "@tanstack/react-router";
import {
  ReactQueryDevtools,
  TanStackRouterDevtools,
} from "../components/DevTools";
import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";
import { Suspense } from "react";
import { QueryClient } from "@tanstack/react-query";
import { Spinner } from "@/components/Spinner";
import { Separator } from "@/components/ui/separator";

function RouterSpinner() {
  const isLoading = useRouterState({ select: (s) => s.status === "pending" });
  return <Spinner show={isLoading} />;
}

export const Route = createRootRouteWithContext<{
  queryClient: QueryClient;
}>()({
  component: RootComponent,
});

function RootComponent() {
  return (
    <>
      <div className={`min-h-screen flex flex-col`}>
        <div className={`flex items-center gap-2`}>
          <h1 className={`text-3xl p-2`}>Auction Site</h1>
          {/* Show a global spinner when the router is transitioning */}
          <div className={`text-3xl`}>
            <RouterSpinner />
          </div>
        </div>
        <Separator className="my-2" />
        <div className={`flex-1 flex`}>
          <div className={`divide-y w-56`}>
            <NavigationMenu>
              <NavigationMenuList>
                <NavigationMenuItem>
                  <Link to="/">
                    <NavigationMenuLink
                      className={navigationMenuTriggerStyle()}
                    >
                      Home
                    </NavigationMenuLink>
                  </Link>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <Link to="/about">
                    <NavigationMenuLink
                      className={navigationMenuTriggerStyle()}
                    >
                      About
                    </NavigationMenuLink>
                  </Link>
                </NavigationMenuItem>
              </NavigationMenuList>
            </NavigationMenu>
          </div>
          <div className="flex space-x-4 text-sm">
            <Separator className="mx-2" orientation="vertical" />
            <Outlet />
          </div>
        </div>
      </div>
      <Suspense>
        <ReactQueryDevtools buttonPosition="top-right" />
        <TanStackRouterDevtools position="bottom-right" />
      </Suspense>
    </>
  );
}
