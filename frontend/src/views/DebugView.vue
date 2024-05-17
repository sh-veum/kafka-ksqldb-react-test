<script setup lang="ts">
import CheckStreams from "@/components/auction/CheckStreams.vue";
import CheckTables from "@/components/auction/CheckTables.vue";
import CreateStreams from "@/components/auction/CreateStreams.vue";
import CreateTables from "@/components/auction/CreateTables.vue";
import DropStreams from "@/components/auction/DropStreams.vue";
import DropTables from "@/components/auction/DropTables.vue";
import InsertAuction from "@/components/auction/InsertAuction.vue";
import InsertAuctionBid from "@/components/auction/InsertAuctionBid.vue";
</script>

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
