<template>
  <div>
    <h1>Demo Page</h1>
    <v-container>
      <v-row>
        <v-col cols="12" sm="6" md="3">
          <CreateTables />
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <CreateStreams />
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <CheckTables />
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <CheckStreams />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" sm="6" md="3">
          <InsertAuction />
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <InsertAuctionBid />
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <DropTables />
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <DropStreams />
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import CheckStreams from "@/components/debugComponents/CheckStreams.vue";
import CheckTables from "@/components/debugComponents/CheckTables.vue";
import CreateStreams from "@/components/debugComponents/CreateStreams.vue";
import CreateTables from "@/components/debugComponents/CreateTables.vue";
import DropStreams from "@/components/debugComponents/DropStreams.vue";
import DropTables from "@/components/debugComponents/DropTables.vue";
import InsertAuction from "@/components/debugComponents/InsertAuction.vue";
import InsertAuctionBid from "@/components/debugComponents/InsertAuctionBid.vue";
</script>
