import { Constants } from "./Constants";
import { TumblrApiResponse } from "./Interface";

export class ApiService {
  getUrl(blogName: string, startPost: number, endPost: number): string {
    return `https://${blogName}${Constants.API.API_URL}${
      endPost - startPost + 1
    }&start=${startPost - 1}`;
  }

  async fetchResponse(apiUrl: string): Promise<string> {
    const response = await fetch(apiUrl);
    if (!response.ok) {
      throw new Error(
        `${Constants.ERRORS.FAILED_TO_FETCH}: ${response.status}`
      );
    }
    return await response.text();
  }

  getUsefulDatafromResponse(response: string): TumblrApiResponse {
    if (response.startsWith(Constants.API.RESPONSE_HEADER)) {
      response = this.extractTumblrApiResponse(response);
    } else {
      throw new Error(Constants.ERRORS.RESPONSE_FORMAT_ERROR);
    }
    return JSON.parse(response);
  }

  extractTumblrApiResponse(apiResponse: string): string {
    // Eliminating Tumblr API's wrapper to parse raw JSON.
    return apiResponse.replace(/^var tumblr_api_read = /, "").slice(0, -2);
  }
}
