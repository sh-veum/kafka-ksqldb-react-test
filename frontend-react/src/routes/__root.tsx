import {
  createRootRouteWithContext,
  Link,
  Outlet,
  useRouterState,
} from "@tanstack/react-router";
import {
  ReactQueryDevtools,
  TanStackRouterDevtools,
} from "../components/dev/DevTools";
import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuList,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";
import { Suspense } from "react";
import { QueryClient } from "@tanstack/react-query";
import { Spinner } from "@/components/Spinner";
import { Separator } from "@/components/ui/separator";
import { ThemeProvider } from "@/components/theme/ThemeProvider";
import { ToggleThemeMode } from "@/components/theme/ThemeToggle";

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
    <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
      <div className="min-h-screen flex flex-col">
        <div className="flex items-center gap-2 flex-wrap">
          <h1 className="text-3xl p-2">Auction Site</h1>
          <ToggleThemeMode />
          {/* Show a global spinner when the router is transitioning */}
          <div className="text-3xl">
            <RouterSpinner />
          </div>
        </div>
        <Separator className="my-2" />
        <div className="flex flex-col lg:flex-row flex-1">
          <div className="w-full lg:w-[5vw] min-w-fit">
            <NavigationMenu>
              <NavigationMenuList className="flex flex-row lg:flex-col">
                <NavigationMenuItem>
                  <Link to="/" className={navigationMenuTriggerStyle()}>
                    Home
                  </Link>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <Link to="/about" className={navigationMenuTriggerStyle()}>
                    About
                  </Link>
                </NavigationMenuItem>
              </NavigationMenuList>
            </NavigationMenu>
          </div>
          <Separator className="lg:hidden my-2" orientation="horizontal" />
          <div className="flex w-screen lg:w-[90vw]">
            <Separator
              className="hidden lg:block mx-2"
              orientation="vertical"
            />
            <Outlet />
          </div>
        </div>
      </div>
      <Suspense>
        <ReactQueryDevtools buttonPosition="top-right" />
        <TanStackRouterDevtools position="bottom-right" />
      </Suspense>
    </ThemeProvider>
  );
}
