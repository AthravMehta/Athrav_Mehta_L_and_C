import { ErrorConstants, InputConstants } from "./Constants";
import promptSync from "prompt-sync";

const prompt = promptSync();
export class InputService {
  getBlogName(): string {
    const blogName = prompt(InputConstants.BlogNameInput);
    if (!blogName) {
      throw new Error(ErrorConstants.InvalidBlogName);
    }
    return blogName.trim();
  }

  getPostsRange(): { startPost: number; endPost: number } {
    const range = prompt(InputConstants.RangeInput);

    if (!range) {
      throw new Error(ErrorConstants.InvalidPostRange);
    }

    const rangeParts = range.split("-").map((num) => parseInt(num.trim(), 10));

    if (
      rangeParts.length !== 2 ||
      isNaN(rangeParts[0]) ||
      isNaN(rangeParts[1])
    ) {
      throw new Error(ErrorConstants.InvalidRangeFormat);
    }

    return { startPost: rangeParts[0], endPost: rangeParts[1] };
  }
}
