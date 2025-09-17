export interface User {
    id: string;
    email: string;
    profilePicture: string;
    photoUrl: string;
    userName: string;
    fullName: string;
    isOnline: boolean;
    connectionId: string;
    unreadCount: number;
    isTyping: boolean;
}