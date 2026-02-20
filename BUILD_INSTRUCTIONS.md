# How to Build Your Modem File Manager Application

## You Don't Need to Know C# - Just Follow These Steps!

I've already added all the new features to your application. Now you just need to create the executable file you can run.

---

## EASIEST METHOD: Using Visual Studio

### Step 1: Open the Project
1. Double-click this file to open in Visual Studio:
   ```
   ModemToolbarIE.sln
   ```
2. Wait for Visual Studio to load completely

### Step 2: Build the Application
1. At the top menu, click **Build**
2. Click **Build Solution** (or press `Ctrl+Shift+B`)
3. Wait for the build to complete (you'll see "Build succeeded" at the bottom)

### Step 3: Create the Installer
1. In the **Solution Explorer** (right side), find **ModemMergerWinFormsApp**
2. **Right-click** on **ModemMergerWinFormsApp**
3. Select **Publish...**
4. You'll see a publish profile window

### Step 4: Configure Publishing (First Time Only)
If you see an existing profile named "ClickOnceProfile":
- Click on it and click **Publish** button
- Skip to Step 5

If you DON'T see a profile:
1. Click **New** or **Add a publish profile**
2. Select **Folder** as the target
3. Choose a location like: `C:\Modem_App_Installer\`
4. Click **Finish**
5. Click **Publish**

### Step 5: Find Your Application
After publishing completes, go to the folder:
```
ModemMergerWinFormsApp\bin\Release\net6.0-windows\publish\
```

Or the folder you chose in Step 4.

You'll find: **ModemMergerWinFormsApp.exe**

### Step 6: Run Your Application
- Double-click **ModemMergerWinFormsApp.exe** to run it
- You'll see a "File Manager" button - that's your new feature!

---

## What's New in Your Application

### New "File Manager" Button
When you click the **File Manager** button:

1. **Enter a Modem Number** (7 digits)
2. Click **Load Modem**
3. You'll see TWO tabs:
   - **Attachments Tab**: All downloadable files from the modem
   - **Gant Tools Tab**: All tools and their descriptions

### Download Files
- **Check** the boxes next to files you want
- Click **Download Selected** to download checked files
- Or click **Download All** to download everything
- Choose where to save using the **Browse** button
- Watch the progress bar as files download

### File Information
Each file shows:
- File name
- File type
- Where it came from (link text)
- How it will be downloaded (method)
- The URL

---

## Troubleshooting

### "Build Failed" Error
1. Make sure you opened the **.sln** file (not individual project files)
2. Right-click the **Solution** in Solution Explorer
3. Click **Restore NuGet Packages**
4. Wait for it to finish
5. Try building again

### "Cannot Find .NET 6.0" Error
You need to install .NET 6.0 Desktop Runtime:
1. Go to: https://dotnet.microsoft.com/download/dotnet/6.0
2. Download: **.NET Desktop Runtime 6.0.x (x64)**
3. Install it
4. Try publishing again

### "ModemWebUtility Not Found" Error
This means not all projects were built:
1. Click **Build** → **Rebuild Solution**
2. Wait for all projects to build
3. Try publishing again

---

## Alternative: Create a Portable Folder

If ClickOnce doesn't work, create a simple folder with all files:

1. Right-click **ModemMergerWinFormsApp** project
2. Click **Publish**
3. Click **New Profile**
4. Choose **Folder**
5. Set location to: `C:\ModemApp_Portable\`
6. Configuration: **Release**
7. Target Framework: **net6.0-windows**
8. **Deployment mode**: Choose one:
   - **Self-contained**: Bigger file but works on any computer (no .NET needed)
   - **Framework-dependent**: Smaller but needs .NET 6.0 installed
9. Click **Publish**

Now you can copy the entire folder to any computer and run the .exe file!

---

## Quick Reference: What Files Do What

- **ModemMergerWinFormsApp.exe** - The main application (what you run)
- **ModemWebUtility.dll** - Contains the code that talks to the modem website
- **HtmlAgilityPack.dll** - Helps read HTML from web pages
- **ClearScript.*.dll** - Helps run JavaScript code from web pages

---

## Need Help?

Common reasons it might not work:
1. **.NET 6.0 not installed** → Download from Microsoft
2. **Antivirus blocking** → Add folder to exceptions
3. **Network access needed** → Application needs internet to connect to modem website
4. **Windows authentication** → Make sure you're logged in with your Halliburton credentials

---

## Summary

**You now have:**
✅ Original modem merge functionality (copy BHA between modems)
✅ NEW: File Manager to view all modem attachments
✅ NEW: Download files individually or in bulk
✅ NEW: View Gant tools and their descriptions
✅ Progress tracking for downloads
✅ Customizable download location

**You DO NOT need to:**
❌ Know C# or any programming
❌ Modify any code
❌ Install development tools (except Visual Studio to build)

Just follow the steps above to create your .exe file!
