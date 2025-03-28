export const Constants = {
  API: {
    API_URL: ".tumblr.com/api/read/json?type=photo&num=",
    RESPONSE_HEADER: "var tumblr_api_read =",
  },
  INPUT: {
    BLOG_NAME_INPUT: "Enter Blog Name:",
    RANGE_INPUT: "Enter the Range ( start-end ): ",
  },
  BASIC_INFO: {
    TITLE: "Title",
    NAME: "Name",
    DESCRIPTION: "Description",
    NUMBER_OF_POST: "Number of Posts",
  },
  ERRORS: {
    ERROR_OCCURRED: "An error occurred",
    NO_PHOTOS_ERROR_MESSAGE: "No photos available for current post.",
    FAILED_TO_FETCH: "Failed to fetch! -- Response Code",
    RESPONSE_FORMAT_ERROR:
      "Response from Tumblr, doesn't match the required format",
    INVALID_BLOG_NAME: "Blog Name not found. Please provide a valid blog name.",
    INVALID_POST_RANGE: "Range not found. Please provide a valid range.",
    INVALID_RANGE_FORMAT:
      "Invalid Range Format. Please enter the range in 'start-end' format.",
    TITLE_NO_AVAILABLE: "Title not available",
    NAME_NO_AVAILABLE: "Name not available",
    DESCRIPTION_NO_AVAILABLE: "Description not available",
  },
};
