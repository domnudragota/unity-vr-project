# How to Get the Unity Project from GitHub

This short guide explains how to download the Unity project from GitHub, open it in Unity, and pull the newest changes later. Please try to work on separate branches when incrementally implementing features.

## 1. What you need first

Before starting, make sure you have:

* Unity Hub installed
* the correct Unity version installed
* Git installed
* access to the GitHub repository

## 2. Copy the repository link

Open the GitHub repository page.

Click the green **Code** button and copy the **HTTPS** link.

It should look similar to this:

```bash
https://github.com/your-username/unity-vr-project.git
```

## 3. Download the project on your PC

Choose or create a folder where you want to keep the project.

Then open **Git Bash** in that location and run:

```bash
git clone https://github.com/domnudragota/unity-vr-project.git
```

This will create a local folder with the whole project.

## 4. Open the project in Unity Hub

After the clone is finished:

1. Open **Unity Hub**
2. Click **Add** or **Add project**
3. Select the downloaded project folder
4. Open the project

Unity may take a little time the first time because it needs to import packages and generate local files.

This is normal, I think.

## 5. Important note about Unity files

In the repository, we keep the important project files such as:

* `Assets`
* `Packages`
* `ProjectSettings`

Unity will automatically regenerate folders like `Library` on each PC.

Because of that, you do not need those generated files from GitHub.

## 6. How to get the newest changes later

If the project was already cloned before and someone pushed new updates, go into the project folder and run:

```bash
git pull
```

This downloads the newest changes from GitHub.

Example:

```bash
cd "/c/Users/YourName/Documents/unity-vr-project"
git pull
```

After that, open Unity Hub and open the same project again.

If Unity is already open, it will usually reimport the changed files automatically.

## 7. Basic daily workflow

A simple workflow is:

1. Before starting work, run `git pull`
2. Open the project in Unity
3. Make your changes
4. Save the scene and project
5. Commit and push your changes

Example:

```bash
git add .
git commit -m "Describe your changes"
git push
```

## 8. If there are errors after pull

Sometimes after pulling, Unity may need a bit of time to reimport assets or packages.

In most cases, you just need to wait until Unity finishes loading.

If something still looks broken:

* close Unity
* open the project again from Unity Hub
* let Unity reimport everything

