import axios from "axios";

export class HttpService {
  static async performPostRequest(url: string, data?: any) {
    try {
      const response = await axios.post(url, data);
      console.log("Post request response", response.data);
      return { data: response.data, loading: false, error: null };
    } catch (err) {
      const errorMessage = this.handleError(err);
      console.error(err);
      return { data: null, loading: false, error: errorMessage };
    }
  }

  static async performGetRequest(url: string) {
    try {
      const response = await axios.get(url);
      console.log("Get request response", response.data);
      return { data: response.data, loading: false, error: null };
    } catch (err) {
      const errorMessage = this.handleError(err);
      console.error(err);
      return { data: null, loading: false, error: errorMessage };
    }
  }

  static async performDeleteRequest(url: string) {
    try {
      const response = await axios.delete(url);
      console.log("Delete request response", response.data);
      return { data: response.data, loading: false, error: null };
    } catch (err) {
      const errorMessage = this.handleError(err);
      console.error(err);
      return { data: null, loading: false, error: errorMessage };
    }
  }

  private static handleError(err: any) {
    if (axios.isAxiosError(err) && err.response) {
      return err.response.data;
    } else {
      return "An error occurred";
    }
  }
}
