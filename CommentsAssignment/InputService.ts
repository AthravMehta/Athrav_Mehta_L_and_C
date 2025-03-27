import { Constants } from "./Constants";
import promptSync from "prompt-sync";

const prompt = promptSync();
export class InputService {
  getBlogName(): string {
    const blogName = prompt(Constants.INPUT.BLOG_NAME_INPUT);
    if (!blogName) {
      throw new Error(Constants.ERRORS.INVALID_BLOG_NAME);
    }
    return blogName.trim();
  }

  getPostsRange(): { startPost: number; endPost: number } {
    const range = prompt(Constants.INPUT.RANGE_INPUT);

    if (!range) {
      throw new Error(Constants.ERRORS.INVALID_POST_RANGE);
    }

    const rangeParts = range.split("-").map((num) => parseInt(num.trim(), 10));

    if (
      rangeParts.length !== 2 ||
      isNaN(rangeParts[0]) ||
      isNaN(rangeParts[1])
    ) {
      throw new Error(Constants.ERRORS.INVALID_RANGE_FORMAT);
    }

    return { startPost: rangeParts[0], endPost: rangeParts[1] };
  }
}
