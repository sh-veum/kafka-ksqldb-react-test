import { Link } from "@tanstack/react-router";
import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuList,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";
import { auth } from "@/lib/auth";
import { useQuery } from "@tanstack/react-query";
import { getUserInfoQueryOptions } from "@/utils/queryOptions";

export const NavBar = () => {
  const { data: authInfo } = useQuery(getUserInfoQueryOptions());

  const handleLogout = () => {
    auth.logout();
  };

  return (
    <NavigationMenu>
      <NavigationMenuList>
        {authInfo?.username ? (
          <NavigationMenuItem>
            <p>Welcome, {authInfo.username}!</p>
          </NavigationMenuItem>
        ) : null}
        <NavigationMenuItem>
          {authInfo?.status === "loggedOut" ? (
            <Link to="/auth/login" className={navigationMenuTriggerStyle()}>
              Login
            </Link>
          ) : (
            <Link
              onClick={handleLogout}
              className={navigationMenuTriggerStyle()}
            >
              Logout
            </Link>
          )}
        </NavigationMenuItem>
      </NavigationMenuList>
    </NavigationMenu>
  );
};
