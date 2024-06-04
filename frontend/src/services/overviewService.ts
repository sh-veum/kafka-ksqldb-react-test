import { HttpService } from "@/lib/httpService";

export class OverviewService {
  private baseUrl: string;
  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  async checkTables() {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/overview/check_tables`
    );
  }

  async checkStreams() {
    return HttpService.performGetRequest(
      `${this.baseUrl}/api/overview/check_streams`
    );
  }

  async dropTable(tableName: string) {
    return HttpService.performDeleteRequest(
      `${this.baseUrl}/api/overview/drop_table?tableName=${tableName}`
    );
  }

  async dropStream(streamName: string) {
    return HttpService.performDeleteRequest(
      `${this.baseUrl}/api/overview/drop_stream?streamName=${streamName}`
    );
  }
}
