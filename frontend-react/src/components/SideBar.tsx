import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuList,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";
import { getUserInfoQueryOptions } from "@/utils/queryOptions";
import { useQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";

export const SideBar = () => {
  const { data: userInfo } = useQuery(getUserInfoQueryOptions());

  return (
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
        {userInfo?.role === "ADMIN" && (
          <NavigationMenuItem>
            <Link to="/admin" className={navigationMenuTriggerStyle()}>
              Admin
            </Link>
          </NavigationMenuItem>
        )}
      </NavigationMenuList>
    </NavigationMenu>
  );
};
