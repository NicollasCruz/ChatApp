export interface ApiResponse<T> {
    isSuccess: boolean;
    data: T;
    message: string;
    error: string;
}