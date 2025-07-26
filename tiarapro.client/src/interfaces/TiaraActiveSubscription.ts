export interface TiaraActiveSubscription {
    id: string;
    user_id: string;
    subscription_id: string;
    order_id: string;
    segmentations_used: number;
    segmentations_allowed: number;
    subscribed_at: string;
    expires_at: string;
    is_active: boolean;
}