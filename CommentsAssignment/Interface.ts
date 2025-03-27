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

export interface TumblrApiResponse {
  tumblelog: Tumblelog;
  "posts-start": number;
  "posts-total": number;
  "posts-type": string;
  posts: Post[];
}

export interface Tumblelog {
  title: string;
  description: string;
  name: string;
  timezone: string;
  cname: boolean;
  feeds: any[];
  uuid: string;
}

export interface Post {
  "photo-url-1280": string;
}
