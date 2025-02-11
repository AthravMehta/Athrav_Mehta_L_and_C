export interface BlogInfo {
  tumblelog: {
    title: string;
    description: string;
    name: string;
  };
  "posts-total": number;
}

export interface PostRange {
  startPost: number;
  endPost: number;
}
