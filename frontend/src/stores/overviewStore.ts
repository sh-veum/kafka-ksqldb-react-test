import axios from "axios";
import { defineStore } from "pinia";
import { ref } from "vue";

const baseUrl = `${import.meta.env.VITE_API_URL}`;
export const useOverviewStore = defineStore("overview", () => {
  const loading = ref(false);
  const error = ref<string | null>(null);

  const checkTables = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(`${baseUrl}/api/overview/check_tables`);
      return response.data;
    } catch (err) {
      error.value = "Failed to check tables";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const checkStreams = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(`${baseUrl}/api/overview/check_streams`);
      return response.data;
    } catch (err) {
      error.value = "Failed to check streams";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const dropTable = async (tableName: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.delete(
        `${baseUrl}/api/overview/drop_table`,
        {
          params: { tableName },
        }
      );
      return response.data;
    } catch (err) {
      error.value = "Failed to drop table";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const dropStream = async (streamName: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.delete(
        `${baseUrl}/api/overview/drop_stream`,
        {
          params: { streamName },
        }
      );
      return response.data;
    } catch (err) {
      error.value = "Failed to drop stream";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  return {
    loading,
    error,
    checkTables,
    checkStreams,
    dropTable,
    dropStream,
  };
});
