import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: () => import("@/views/HomeView.vue"),
    },
    {
      path: "/alt",
      name: "alt home",
      component: () => import("@/views/HomeViewAlt.vue"),
    },
    {
      path: "/debug",
      name: "debug",
      component: () => import("@/views/DebugView.vue"),
    },
    {
      path: "/login",
      name: "login",
      component: () => import("@/views/LoginView.vue"),
    },
    {
      path: "/register",
      name: "register",
      component: () => import("@/views/RegisterView.vue"),
    },
    {
      path: "/auction/:auctionId",
      name: "auction",
      component: () => import("@/views/AuctionView.vue"),
      props: true,
    },
  ],
});

export default router;
