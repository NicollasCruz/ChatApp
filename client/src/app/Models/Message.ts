export interface Message{
    id: number;
    senderId: string | null;
    receiverId: string | null;
    content: string | null;
    timeStamp: string;
    isRead: boolean;
}