<template>
  <main>
    <div style="max-height: min-content" class="mx-4">
      <h1>Auction Overview</h1>
    </div>
    <v-container fluid class="parent">
      <div class="div1">
        <div class="mt-4 mx-4">
          <InsertAuction />

          <div class="mt-4">
            <v-select
              v-model="sortOrder"
              :items="['Latest', 'Earliest']"
              label="Sort by"
              variant="outlined"
            ></v-select>
          </div>
        </div>
        <AuctionOverview :sortOrder="sortOrder" />
      </div>

      <div class="div2">
        <h2>Recent Bids</h2>
        <RecentBidsContainer />
      </div>
      <div class="div3">
        <h2>People here:</h2>
        <div v-for="(user, index) in locationStore.usersOnPage" :key="index">
          <p>{{ user }}</p>
        </div>
      </div>
    </v-container>
  </main>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, watch } from "vue";
import InsertAuction from "@/components/debugComponents/InsertAuction.vue";
import RecentBidsContainer from "@/components/bids/RecentBidsContainer.vue";
import AuctionOverview from "@/components/auction/AuctionOverview.vue";
import { useLocationStore } from "@/stores/locationStore";
import type { UserLocationUpdate } from "@/models/UserLocationUpdate";
import { useUserStore } from "@/stores/userStore";

const pageName = "Home";
const userStore = useUserStore();
const locationStore = useLocationStore();
const username = userStore.userInfo?.UserName ?? "anon";
const sortOrder = ref("Latest");

onMounted(async () => {
  await userStore.fetchUserInfo();
  console.log(username);
  const userLocationUpdate: UserLocationUpdate = {
    User_Location_Id: null,
    User_Id: username,
    Page: pageName,
  };
  locationStore.addLocation(userLocationUpdate);
  locationStore.getUsersOnPage("Home");
});

onBeforeUnmount(async () => {
  const userLocationUpdate: UserLocationUpdate = {
    User_Location_Id: null,
    User_Id: username,
    Page: pageName,
  };
  await locationStore.removeLocation(userLocationUpdate);
});

watch(sortOrder, (newOrder) => {
  sortOrder.value = newOrder;
});
</script>

<style scoped>
.parent {
  height: 85vh;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  grid-template-rows: repeat(5, 1fr);
  grid-column-gap: 0px;
  grid-row-gap: 1px;
}

.div1 {
  grid-area: 1 / 1 / 5 / 3;
}
.div2 {
  grid-area: 1 / 3 / 6 / 4;
}
.div3 {
  grid-area: 5 / 1 / 6 / 3;
}
</style>
