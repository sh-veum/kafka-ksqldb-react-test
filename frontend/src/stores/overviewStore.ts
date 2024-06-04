import { defineStore } from "pinia";
import { ref } from "vue";
import { OverviewService } from "@/services/overviewService";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useOverviewStore = defineStore("overview", () => {
  const overviewService = new OverviewService(baseUrl);

  return {
    checkTables: overviewService.checkTables.bind(overviewService),
    checkStreams: overviewService.checkStreams.bind(overviewService),
    dropTable: overviewService.dropTable.bind(overviewService),
    dropStream: overviewService.dropStream.bind(overviewService),
  };
});
