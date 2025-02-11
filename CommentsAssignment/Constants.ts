export const AppConstants = {
  PhotoUrl: "photo-url-1280",
  ApiUrl: ".tumblr.com/api/read/json?type=photo&num=",
  ResponseHeader: "var tumblr_api_read =",
};

export const InputConstants = {
  BlogNameInput: "Enter Blog Name:",
  RangeInput: "Enter the Range ( start-end ): ",
};

export const BasicInfoConstant = {
  Title: "Title",
  Name: "Name",
  Description: "Description",
  NumberOfPost: "Number of Posts",
};

export const ErrorConstants = {
  ErrorOccured: "An error occurred",
  NoPhotosErrorMessage: "No photos available for current post.",
  FailedToFetch: "Failed to fetch! -- Response Code",
  ResponseFormatError:
    "Response from Tumblr, doesn't match the required format",
  InvalidBlogName: "Blog Name not found. Please provide a valid blog name.",
  InvalidPostRange: "Range not found. Please provide a valid range.",
  InvalidRangeFormat:
    "Invalid Range Format. Please enter the range in 'start-end' format.",
  TitleNoAvailable: "Title not available",
  NameNoAvailable: "Name not available",
  DescriptionNoAvailable: "Description not available",
};
