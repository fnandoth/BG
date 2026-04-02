export enum PostType {
  Post = 0,
  Reply = 1,
  Repost = 2,
  Quote = 3,
};


export interface UserResponse {
  id: string;
  userName: string;
  email: string;
  displayName: string;
  bio: string;
  avatarUrl: string;
  isPrivate: boolean;
  createdAt: string;
}

export interface UserPlain {
  displayName: string;
  avatarUrl: string;
  bio: string;
  createdAt: string;
}

export interface SessionUser {
  id?: string;
  userName?: string;
  email?: string;
  displayName: string;
  bio?: string;
  avatarUrl: string;
  isPrivate?: boolean;
  createdAt?: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAt: string;
  user: UserResponse;
}

export interface AuthorSnapshot {
  userId: string;
  username: string;
  displayName: string;
  avatarUrl?: string | null;
  isVerified: boolean;
}

export interface PostSummary {
  id: string;
  content: string;
  author: AuthorSnapshot;
}

export interface TimelinePost {
  id: string;
  content: string;
  postType: PostType;
  author: AuthorSnapshot;
  likesCount: number;
  repliesCount: number;
  repostsCount: number;
  quotesCount: number;
  quotedPost?: PostSummary | null;
  repostedPost?: PostSummary | null;
  createdAt: string;
}

export interface PostDetail {
  id: string;
  content: string;
  postType: PostType;
  author: AuthorSnapshot;
  likesCount: number;
  repliesCount: number;
  repostsCount: number;
  quotesCount: number;
  quotedPost?: PostSummary | null;
  replies: PostDetail[];
  createdAt: string;
}

export interface CreatePostRequest {
  content: string;
  mediaUrls?: string[];
  postType: PostType;
  parentPostId?: string;
  quotedPostId?: string;
  repostedPostId?: string;
}

export interface NotificationActor {
  userId: string;
  username: string;
  displayName: string;
  avatarUrl?: string | null;
}

export interface NotificationPostPreview {
  postId: string;
  contentPreview: string;
}

export interface NotificationItem {
  id: string;
  type: string;
  isRead: boolean;
  createdAt: string;
  actor: NotificationActor;
  post?: NotificationPostPreview | null;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
}

export interface SessionResponse {
  authenticated: boolean;
  user: SessionUser | null;
  expiresAt?: string | null;
}

export interface ToggleLikeResponse {
  liked: boolean;
}
