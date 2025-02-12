import { AppConstants, ErrorConstants } from "./Constants";

export class ApiService {
  getUrl(blogName: string, startPost: number, endPost: number): string {
    return `https://${blogName}${AppConstants.ApiUrl}${
      endPost - startPost + 1
    }&start=${startPost - 1}`;
  }

  async fetchResponse(apiUrl: string): Promise<string> {
    const response = await fetch(apiUrl);
    if (!response.ok) {
      throw new Error(`${ErrorConstants.FailedToFetch}: ${response.status}`);
    }
    return await response.text();
  }

  getUsefulDatafromResponse(response: string): any {
    if (response.startsWith(AppConstants.ResponseHeader)) {
      response = response.replace(/^var tumblr_api_read = /, "").slice(0, -2); // necesarry for parsing response, Api returns this way
    } else {
      throw new Error(ErrorConstants.ResponseFormatError);
    }
    return JSON.parse(response);
  }
}
