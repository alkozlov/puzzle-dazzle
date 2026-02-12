# Google Play Billing Setup Guide

This guide explains how to configure Google Play Console for the Mazele Dazzle app's in-app subscriptions.

## Prerequisites

1. Google Play Console account with app created
2. App published to at least Internal Testing track
3. Signed APK/AAB uploaded to Play Console

## Subscription Product Configuration

### Product IDs (defined in code)

The app uses these product IDs (see `GooglePlaySubscriptionService.cs`):

- **Monthly**: `premium_monthly`
- **Yearly**: `premium_yearly`

### Steps to Configure in Google Play Console

1. **Navigate to Monetization Setup**
   - Open your app in Google Play Console
   - Go to **Monetize** > **Subscriptions**

2. **Create Monthly Subscription**
   - Click **Create subscription**
   - Product ID: `premium_monthly`
   - Name: `Mazele Dazzle Premium - Monthly`
   - Description: `Unlimited maze generation with premium subscription`
   - Billing period: `1 month`
   - Base price: `$0.99 USD`
   - Free trial: `None` (not supported in initial release)
   - Grace period: `3 days` (recommended)
   - Save and activate

3. **Create Yearly Subscription**
   - Click **Create subscription**
   - Product ID: `premium_yearly`
   - Name: `Mazele Dazzle Premium - Yearly`
   - Description: `Unlimited maze generation with annual premium subscription`
   - Billing period: `1 year`
   - Base price: `$9.99 USD`
   - Free trial: `None` (not supported in initial release)
   - Grace period: `3 days` (recommended)
   - Save and activate

## Testing Subscriptions

### Test with License Testers

1. **Add Test Accounts**
   - Go to **Settings** > **License testing**
   - Add tester email addresses
   - Select **License test response**: `RESPOND_NORMALLY`

2. **Testing Workflow**
   - Build and sign the app with release key
   - Upload to Internal Testing track
   - Add testers to Internal Testing
   - Testers download app from Play Store
   - Test purchases will not be charged
   - Subscriptions auto-cancel after a few minutes for testing

### Debug vs Release Builds

- **DEBUG builds**: Use `MockSubscriptionService` (unlimited access for development)
- **RELEASE builds**: Use `GooglePlaySubscriptionService` (real billing)

## Important Notes

### Product ID Requirements

- **Must match exactly** between code and Play Console
- Product IDs cannot be changed after creation
- If you need to change, must create new products

### Subscription Features

- ✅ Monthly and yearly subscriptions
- ✅ Automatic renewal
- ✅ Grace period (3 days)
- ✅ Purchase verification
- ❌ Free trial (not in initial release)
- ❌ Introductory pricing (not in initial release)

### App Requirements

- App must be published (at least to Internal Testing)
- App must be signed with release key
- Cannot test on debug builds in production
- License testers see special testing UI

## Troubleshooting

### "Item not available" error
- Check product IDs match exactly
- Ensure products are activated in Play Console
- Verify app is published to testing track
- Confirm using release-signed APK

### Purchases not working
- Check billing client connection status
- Verify Google Play Services is up to date on device
- Ensure device has valid payment method (for real purchases)
- Check Play Console billing reports

### Testing issues
- Wait 2-4 hours after creating products for them to be available
- Clear Google Play Store cache
- Ensure tester account is added to License testing
- Use release-signed build, not debug build

## Implementation Details

### Service Architecture

```
ISubscriptionService (interface)
├── MockSubscriptionService (DEBUG builds)
└── GooglePlaySubscriptionService (RELEASE builds)
```

### Purchase Flow

1. User clicks "Upgrade to Premium"
2. `PremiumUpgradePage` calls `PurchaseSubscriptionAsync(productId)`
3. `GooglePlaySubscriptionService` launches Google Play billing flow
4. User completes purchase in Google Play UI
5. Service receives purchase callback
6. Purchase is acknowledged automatically
7. User status updated to premium
8. Success message shown, navigate back

### Subscription Verification

- On app launch: `RefreshSubscriptionStatusAsync()` called
- Queries active purchases from Google Play
- Updates `_isPremium` flag
- Auto-acknowledges any unacknowledged purchases

## Next Steps

After configuring products in Play Console:

1. Upload release APK to Internal Testing
2. Add test accounts to license testing
3. Test purchase flow with test accounts
4. Verify subscription status persists across app restarts
5. Test subscription management (cancel, resubscribe)
6. Review billing reports in Play Console

## References

- [Google Play Billing Documentation](https://developer.android.com/google/play/billing)
- [Subscription Setup Guide](https://support.google.com/googleplay/android-developer/answer/140504)
- [Testing Guide](https://developer.android.com/google/play/billing/test)
