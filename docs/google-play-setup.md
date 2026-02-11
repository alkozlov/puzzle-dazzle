# Google Play Console Setup Guide

Step-by-step guide for setting up your app in Google Play Console.

## Prerequisites

- ✅ Google Play Developer Account (verified)
- ✅ Signed AAB file: `com.companyname.mazeledazzle-Signed.aab`
- ✅ App descriptions and store listing text
- ✅ Privacy policy document
- ⏳ Screenshots (2-8 images)
- ⏳ Feature graphic (1024x500px)
- ⏳ Support email address

---

## Step 1: Access Google Play Console

1. Go to https://play.google.com/console
2. Sign in with your verified Google account
3. Accept the Developer Distribution Agreement if prompted

---

## Step 2: Create New App

1. Click **"Create app"** button
2. Fill in the details:

### App Details
- **App name**: Mazele Dazzle
- **Default language**: English (United States)
- **App or game**: App
- **Free or paid**: Free
- **Declarations**:
  - ✅ Check: Developer Program Policies
  - ✅ Check: US export laws

3. Click **"Create app"**

---

## Step 3: Complete Dashboard Tasks

Google Play Console uses a task-based system. Complete each required task:

### 3.1 Set Up Your App

#### Store Settings → Main Store Listing

**App details:**
- **App name**: Mazele Dazzle - Maze Generator
- **Short description** (80 chars max):
  ```
  Create and share custom mazes. Perfect for puzzles, education, and fun!
  ```
- **Full description** (4000 chars max):
  ```
  [Copy from docs/store-listing.md - Full Description section]
  ```

**Graphics:**
- **App icon**: 512 x 512 px (upload from app or recreate)
  - Use: `src/PuzzleDazzle/Resources/AppIcon/appicon.svg` (export as PNG at 512x512)
  
- **Feature graphic**: 1024 x 500 px **(REQUIRED)**
  - Create using Canva, Figma, or design tool
  - Include: App name, tagline, maze visual
  
- **Phone screenshots**: 2-8 images **(REQUIRED)**
  - Take screenshots on your phone
  - Show: Generation screen, Settings, maze examples
  - Dimensions: Minimum 320px, maximum 3840px
  
- **Tablet screenshots**: Optional (can add later)

**Categorization:**
- **App category**: Puzzle
- **Tags**: maze, puzzle, brain teaser, educational, creative

**Contact details:**
- **Email**: mazele.dazzle.app@gmail.com (or your email)
- **Website**: Optional (can add later)
- **Phone**: Optional

**Privacy Policy** **(REQUIRED)**:
- You need to host the privacy policy online
- Options:
  1. Create a GitHub Pages site (free)
  2. Use Google Sites (free)
  3. Upload to your own domain
- URL format: `https://yourdomain.com/privacy-policy`

Click **"Save"** when done.

#### Store Settings → Store Presence

**Feature Graphic and Screenshots:**
Complete if not done in Main Store Listing above.

#### App Content

**Privacy Policy:**
- **URL**: [Your hosted privacy policy URL]
- Use content from `docs/privacy-policy.md`

**Ads:**
- **Does your app contain ads?**: No

**App access:**
- **Is your app restricted?**: No
- All features available to all users

**Content ratings:**
- Click **"Start questionnaire"**
- **Email**: Your email
- **Category**: Utility, Productivity, Communication, or Other
- **Questions**: Answer honestly:
  - Violence: None
  - Sexual content: None
  - Language: None
  - Controlled substances: None
  - Gambling: None
  - Social features: None
  - Location sharing: None
- Expected rating: **Everyone (E)** or **Everyone 10+ (E10+)**
- Submit and get your content rating certificate

**Target audience and content:**
- **Target age groups**: All ages (or select specific ranges)
- **Is your app designed for children?**: No (simpler compliance)
  - If Yes, additional policies apply
- **Is this a news app?**: No

**News apps:**
- **Is this a news app?**: No

**COVID-19 contact tracing and status apps:**
- Skip (not applicable)

**Data safety:**
- Click **"Start"**
- **Does your app collect or share data?**: No
  - We don't collect any user data
- **Does your app follow Play Families Policy?**: No (unless targeting kids)
- **Does your app use HTTPS?**: Yes (for Google Play Billing)
- Review and submit

**Government apps:**
- Skip (not applicable)

---

## Step 4: Create Release (Closed Testing Track)

### Production → Releases → Testing

1. Click **"Testing"** tab
2. Click **"Create closed test"**
3. **Testers**:
   - Click **"Create email list"**
   - Name: "Beta Testers"
   - Add email addresses of friends/family
   - Save

4. **Release**:
   - Click **"Create new release"**
   - **Upload AAB**:
     - Upload: `com.companyname.mazeledazzle-Signed.aab`
     - Wait for processing (may take a few minutes)
   
   - **Release name**: 1.0 (Version 1)
   
   - **Release notes**: (What's new in this version)
     ```
     Initial beta release of Mazele Dazzle!
     
     Features:
     - Generate custom mazes with different sizes and difficulties
     - Classic and Soft visualization styles
     - Save mazes as PNG images
     - Share mazes via social media and messaging apps
     - Freemium model: 5 mazes per day free, unlimited with premium subscription
     
     Please test all features and provide feedback!
     ```
   
   - Click **"Save"** and then **"Review release"**
   - Review all details
   - Click **"Start rollout to Closed testing"**

5. **Share with testers**:
   - After rollout, get the opt-in URL
   - Share URL with your testers via email
   - Testers must opt-in, then can download from Play Store

---

## Step 5: Wait for Review (Closed Testing)

- **Review time**: Usually a few hours to 1 day for closed testing
- **Status**: Check "Release dashboard" for status
- **If approved**: Testers can download
- **If rejected**: Review feedback and fix issues

---

## Step 6: Gather Feedback (1-2 Weeks)

- Ask testers to try all features
- Collect feedback via:
  - Email
  - Google Forms survey
  - Direct messages
- Monitor crash reports in Play Console
- Fix critical issues and upload new versions

---

## Step 7: Production Release (When Ready)

After successful closed testing:

1. Go to **Production** → **Releases** → **Production**
2. Click **"Create new release"**
3. **Promote from closed testing** or **Upload new AAB**
4. Add production release notes
5. Review all compliance requirements:
   - Content rating ✅
   - Privacy policy ✅
   - Data safety ✅
   - Target audience ✅
6. Click **"Review release"**
7. Click **"Start rollout to Production"**

**Production review time**: 1-3 days (sometimes longer)

---

## Important Notes

### App Signing by Google Play (Recommended)

When uploading your first AAB, you'll be asked about app signing:

**Option 1: Google Play App Signing (Recommended)**
- Google manages your signing key
- More secure
- Enables advanced features
- **Choose this** unless you have specific reasons not to

**Option 2: Use your own keystore**
- You manage the keystore yourself
- More responsibility
- Risk of losing the key

We recommend **Option 1** for easier management.

### Version Management

When releasing updates:
1. Increment `ApplicationVersion` in `PuzzleDazzle.csproj` (must increase)
2. Update `ApplicationDisplayVersion` (e.g., 1.0 → 1.1)
3. Rebuild release AAB
4. Upload to desired track

### Country Availability

By default, your app is available in all countries. You can restrict this:
- **Production** → **Countries/regions**
- Select specific countries
- Useful for testing or regional restrictions

### Pricing & Distribution

- **Free app**: No setup needed
- **Premium features** (In-app purchases):
  - Set up after initial release
  - Requires Google Play Billing integration
  - We have mock implementation, need real billing (Task: puzzle-e36)

---

## Checklist Before Submission

- [ ] App name and descriptions written
- [ ] Screenshots taken (2-8 images)
- [ ] Feature graphic created (1024x500px)
- [ ] Privacy policy hosted online with URL
- [ ] Support email set up
- [ ] Content rating questionnaire completed
- [ ] Data safety form filled
- [ ] AAB file uploaded
- [ ] Testers invited (for closed testing)
- [ ] Release notes written
- [ ] All compliance requirements met

---

## Common Issues and Solutions

### Issue: "Privacy policy URL required"
**Solution**: Host `docs/privacy-policy.md` online (GitHub Pages, Google Sites, etc.) and add URL

### Issue: "Feature graphic missing"
**Solution**: Create 1024x500px banner image showing app name and visuals

### Issue: "Screenshots required"
**Solution**: Take 2-8 screenshots on your phone showing app features

### Issue: "Content rating incomplete"
**Solution**: Complete the content rating questionnaire honestly

### Issue: "AAB upload failed"
**Solution**: Ensure AAB is signed correctly and version code is higher than previous uploads

---

## Next Steps After This Guide

1. **Test on your device** - Verify everything works
2. **Take screenshots** - Show app features
3. **Create feature graphic** - Design 1024x500px banner
4. **Host privacy policy** - Put it online
5. **Set up support email** - Create dedicated email
6. **Upload to Play Console** - Follow steps above
7. **Invite testers** - Get feedback before production release

Good luck with your app launch! 🚀
