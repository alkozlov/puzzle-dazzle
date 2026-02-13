# Google Play Billing Setup Guide

Complete step-by-step guide to get in-app subscriptions working on a real Android device.

> **Why are prices showing "N/A"?** The billing client connects but product queries return empty results if the subscription products don't exist in Google Play Console, or the app was sideloaded instead of installed through Google Play.

## Prerequisites

- Google Play Developer Account ($25 one-time fee at https://play.google.com/console)
- A signed release AAB/APK of the app
- A real Android device with a Google account

## Overview

The billing flow requires this chain to work:

```
Google Play Console (products configured)
        ↓
App uploaded to a testing track
        ↓
App installed via Google Play (not sideloaded)
        ↓
BillingClient connects → queries products → shows prices
```

If any link is missing, prices will show "N/A".

---

## Step 1: Create the App in Google Play Console

If you haven't already created the app in Google Play Console, follow `docs/google-play-setup.md` first. You need:

- App created in Play Console
- Basic store listing filled in (name, description, screenshots)
- Privacy policy URL set

---

## Step 2: Upload a Signed Release Build

The app must be uploaded to at least the **Internal Testing** track before you can create subscription products.

### Build the release AAB

```powershell
# From the project root on Windows
dotnet publish src/PuzzleDazzle/PuzzleDazzle.csproj -f net9.0-android -c Release
```

The signed AAB will be in:
```
src/PuzzleDazzle/bin/Release/net9.0-android/publish/
```

### Upload to Internal Testing

1. In Google Play Console, go to **Testing > Internal testing**
2. Click **Create new release**
3. Upload your signed AAB file
4. Add release notes (e.g., "Initial billing test")
5. Click **Save** then **Review release** then **Start rollout**

> **Important:** The app's package name in the uploaded AAB must match your `AndroidManifest.xml`. The signing key must be consistent across uploads.

---

## Step 3: Create Subscription Products

This is the critical step. Without products in Play Console, `QueryProductDetailsAsync` returns empty results.

### Navigate to Subscriptions

1. Open your app in Google Play Console
2. Go to **Monetize > Subscriptions**

> **Note:** The "Subscriptions" option may not appear until you've uploaded at least one AAB/APK to any track.

### Create Monthly Subscription

1. Click **Create subscription**
2. Fill in:
   - **Product ID**: `premium_monthly` (must match exactly - cannot be changed later)
   - **Name**: Mazele Dazzle Premium - Monthly
   - **Description**: Unlimited maze generation with premium subscription
3. Click **Save**
4. Under **Base plans**, click **Add base plan**:
   - **Base plan ID**: e.g., `monthly-base`
   - **Auto-renewing**: Yes
   - **Billing period**: 1 month
   - **Grace period**: 3 days (recommended)
5. Click **Set prices**:
   - Set price for your default country (e.g., $0.99 USD)
   - Click **Update prices** to auto-fill other countries
6. **Activate** the base plan
7. **Activate** the subscription (top of page)

### Create Yearly Subscription

1. Click **Create subscription**
2. Fill in:
   - **Product ID**: `premium_yearly` (must match exactly)
   - **Name**: Mazele Dazzle Premium - Yearly
   - **Description**: Unlimited maze generation with annual premium subscription
3. Click **Save**
4. Under **Base plans**, click **Add base plan**:
   - **Base plan ID**: e.g., `yearly-base`
   - **Auto-renewing**: Yes
   - **Billing period**: 1 year
   - **Grace period**: 7 days (recommended)
5. Click **Set prices**:
   - Set price (e.g., $9.99 USD)
   - Click **Update prices** to auto-fill other countries
6. **Activate** the base plan
7. **Activate** the subscription

### Verify Products Are Active

After creating both products, verify:

- Both subscriptions show **Active** status (green)
- Each has at least one **Active** base plan
- Product IDs are exactly `premium_monthly` and `premium_yearly`

> **Wait time:** New products can take **up to 24 hours** to propagate. In practice, it's usually 15-60 minutes, but don't panic if they don't appear immediately.

---

## Step 4: Set Up License Testing

License testers can make purchases without being charged real money.

1. Go to **Settings > License testing** (in the left sidebar, under "All apps")
2. Under **Gmail accounts with license testing access**, add your Google account email
3. Set **License test response** to: **RESPOND_NORMALLY**
4. Click **Save**

> **Important:** The email must match the Google account signed in on your test device.

---

## Step 5: Add Testers to Internal Testing Track

1. Go to **Testing > Internal testing**
2. Click the **Testers** tab
3. Create a new email list or use an existing one
4. Add your tester email addresses (including your own)
5. Click **Save changes**
6. Copy the **Opt-in URL** (you'll need this)

---

## Step 6: Install the App from Google Play

This is where most people get stuck. **Sideloaded apps cannot query billing products.**

### Join the test

1. On your Android device, open the **Opt-in URL** from Step 5 in a browser
2. Sign in with the Google account that was added as a tester
3. Click **Accept** to join the test

### Install from Google Play

1. After joining, click **Download it on Google Play** (or search for the app in Play Store)
2. Install the app from the Play Store

> **Critical:** You must install through the Play Store. Running `adb install` or transferring the APK directly will NOT work for billing.

### If the app doesn't appear in Play Store

- Wait 10-30 minutes after rollout for the internal testing track to propagate
- Make sure you joined the test with the same Google account that's primary on the device
- Clear Google Play Store cache: **Settings > Apps > Google Play Store > Clear Cache**
- Try the direct opt-in link again

---

## Step 7: Test the Billing Flow

1. Open the app (installed from Play Store)
2. Navigate to **Settings > Upgrade to Premium**
3. You should see real prices loaded from Google Play (e.g., "$0.99/month")
4. Click **Subscribe Monthly** or **Subscribe Annually**
5. Google Play purchase dialog should appear
6. As a license tester, the purchase dialog will show **"Test card"** as the payment method
7. Complete the purchase -- you will NOT be charged

### After successful purchase

- The app should show "Success!" and navigate back
- Premium status should be active (no daily limit)
- Restart the app to verify premium status persists

---

## Troubleshooting

### Prices show "N/A"

| Possible cause | Fix |
|---|---|
| Products not created in Play Console | Create them (Step 3) |
| Products not activated | Activate both the subscription and the base plan |
| Product IDs don't match code | Must be exactly `premium_monthly` and `premium_yearly` |
| App was sideloaded, not from Play Store | Uninstall and reinstall from Play Store (Step 6) |
| Products haven't propagated yet | Wait up to 24 hours |
| No AAB uploaded to any track | Upload to Internal Testing first |

### Prices show "---"

This means the billing client threw an exception during the query. Check device logs:

```powershell
adb logcat -s PuzzleDazzle
```

### "Item not available" error when purchasing

- Verify the subscription base plan is **Active** (not just the subscription itself)
- Verify the tester account is in License Testing
- Verify the app is installed from Play Store

### Purchase succeeds but premium not granted

- Check that the purchase was acknowledged (in device logs)
- Restart the app to trigger `RefreshSubscriptionStatusAsync`

### Testing subscription renewal

License test subscriptions have accelerated renewal:
- **Daily** subscriptions renew every 5 minutes
- **Weekly** subscriptions renew every 5 minutes
- **Monthly** subscriptions renew every 5 minutes
- **Yearly** subscriptions renew every 5 minutes
- Test subscriptions auto-cancel after 6 renewals

---

## Product IDs Reference

Defined in `ISubscriptionService.cs`:

```csharp
const string ProductIdMonthly = "premium_monthly";
const string ProductIdYearly = "premium_yearly";
```

These must match the Product IDs in Google Play Console exactly. They are case-sensitive and cannot be changed after creation.

---

## Architecture

### Service Registration

```
DEBUG  → MockSubscriptionService  (hardcoded prices, no real billing)
RELEASE → GooglePlaySubscriptionService (real Google Play Billing)
```

### Purchase Flow

```
User taps "Subscribe"
    → PremiumUpgradePage calls PurchaseSubscriptionAsync(productId)
    → Service waits for billing client connection (up to 10s)
    → Queries ProductDetails from Google Play
    → Launches Google Play billing flow (system UI)
    → User completes/cancels purchase
    → OnPurchasesUpdated callback fires
    → Purchase acknowledged with retry (3 attempts)
    → Premium status persisted to Preferences
    → Success/failure returned to UI
```

### Subscription Status on App Start

```
App starts
    → Constructor loads cached premium status from Preferences (instant)
    → MainActivity.OnCreate calls Initialize(activity)
    → BillingClient.StartConnection (async)
    → OnBillingSetupFinished → RefreshSubscriptionStatusAsync
    → Queries active purchases from Google Play
    → Updates and persists premium status
```

---

## Checklist

- [ ] Google Play Developer Account created
- [ ] App created in Google Play Console
- [ ] Signed release AAB uploaded to Internal Testing track
- [ ] Subscription `premium_monthly` created and **activated** with active base plan
- [ ] Subscription `premium_yearly` created and **activated** with active base plan
- [ ] Your Google account added to **License Testing** (Settings > License testing)
- [ ] Your email added to Internal Testing testers list
- [ ] Joined the test via opt-in URL on your device
- [ ] App installed from Google Play Store (not sideloaded)
- [ ] Waited 15-60 minutes for product propagation (if newly created)
- [ ] Prices showing correctly on Premium page
- [ ] Test purchase completes successfully

---

## References

- [Google Play Billing Documentation](https://developer.android.com/google/play/billing)
- [Create Subscriptions](https://support.google.com/googleplay/android-developer/answer/140504)
- [Testing In-App Billing](https://developer.android.com/google/play/billing/test)
- [License Testing](https://developer.android.com/google/play/billing/test#license-testers)
