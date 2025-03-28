import { ApiService } from "./ApiService";
import { Constants } from "./Constants";
import { InputService } from "./InputService";
import { BlogInfo, Post, PostRange, TumblrApiResponse } from "./Interface";

class TumblrBlogInfo {
  static async getInfo() {
    const inputService = new InputService();
    const apiService = new ApiService();

    const blogName = inputService.getBlogName();
    const { startPost, endPost }: PostRange = inputService.getPostsRange();

    const apiUrl = apiService.getUrl(blogName, startPost, endPost);
    try {
      const response = await apiService.fetchResponse(apiUrl);
      const usefulData: TumblrApiResponse =
        apiService.getUsefulDatafromResponse(response);
      this.displayInformation(usefulData);
    } catch (error) {
      console.error(
        `${Constants.ERRORS.ERROR_OCCURRED}: ${
          error instanceof Error ? error.message : error
        }`
      );
    }
  }

  static displayInformation(data: TumblrApiResponse) {
    TumblrBlogInfo.displayBasicInfo(data);
    TumblrBlogInfo.displayPosts(data.posts);
  }

  static displayBasicInfo = ({
    tumblelog: { name, title, description },
    "posts-total": totalPosts,
  }: BlogInfo): void => {
    console.log(
      `\n${Constants.BASIC_INFO.TITLE}: ${
        title ? title : Constants.ERRORS.TITLE_NO_AVAILABLE
      }`
    );
    console.log(
      `${Constants.BASIC_INFO.NAME}: ${
        name ? name : Constants.ERRORS.NAME_NO_AVAILABLE
      }`
    );
    console.log(
      `${Constants.BASIC_INFO.DESCRIPTION}: ${
        description ? description : Constants.ERRORS.DESCRIPTION_NO_AVAILABLE
      }`
    );
    console.log(`${Constants.BASIC_INFO.NUMBER_OF_POST}: ${totalPosts}\n`);
  };

  static displayPosts = (posts: Post[]): void => {
    posts.forEach((post, index) => {
      console.log(
        `${index + 1}. ${
          post["photo-url-1280"] || Constants.ERRORS.NO_PHOTOS_ERROR_MESSAGE
        }`
      );
    });
  };
}

TumblrBlogInfo.getInfo();
