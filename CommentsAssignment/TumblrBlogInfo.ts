import { ApiService } from "./ApiService";
import { AppConstants, BasicInfoConstant, ErrorConstants } from "./Constants";
import { InputService } from "./InputService";
import { BlogInfo, PostRange } from "./Interface";

class TumblrBlogInfo {
  static async getInfo() {
    const inputService = new InputService();
    const apiService = new ApiService();

    const blogName = inputService.getBlogName();
    const { startPost, endPost }: PostRange = inputService.getPostsRange();

    const apiUrl = apiService.getUrl(blogName, startPost, endPost);
    try {
      const response = await apiService.fetchResponse(apiUrl);
      const usefulData = apiService.getUsefulDatafromResponse(response);
      this.displayInformation(usefulData);
    } catch (error) {
      console.error(
        `${ErrorConstants.ErrorOccured}: ${
          error instanceof Error ? error.message : error
        }`
      );
    }
  }

  static displayInformation(data: any) {
    TumblrBlogInfo.displayBasicInfo(data);
    TumblrBlogInfo.displayPosts(data.posts);
  }

  static displayBasicInfo = ({
    tumblelog: { name, title, description },
    "posts-total": totalPosts,
  }: BlogInfo): void => {
    console.log(
      `\n${BasicInfoConstant.Title}: ${
        title ? title : ErrorConstants.TitleNoAvailable
      }`
    );
    console.log(
      `${BasicInfoConstant.Name}: ${
        name ? name : ErrorConstants.NameNoAvailable
      }`
    );
    console.log(
      `${BasicInfoConstant.Description}: ${
        description ? description : ErrorConstants.DescriptionNoAvailable
      }`
    );
    console.log(`${BasicInfoConstant.NumberOfPost}: ${totalPosts}\n`);
  };

  static displayPosts = (posts: any[]): void => {
    let postNumbering = 1;
    for (const post of posts) {
      console.log(
        `${postNumbering}. ${
          post[AppConstants.PhotoUrl] || ErrorConstants.NoPhotosErrorMessage
        }`
      );
      postNumbering++;
    }
  };
}

TumblrBlogInfo.getInfo();
