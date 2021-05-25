# WSL Disk Shrinker

Facing smaller and smaller C disk space ?  Growing WSL disk usage?

This is the right tool you are searching for!

This small tool can help you reduce the disk usage of WSL distros.

## Usage

Open the application, you can see all your wsl distros.

> If you can't see some of your distro, you can edit the `distros.json` to add them.
>
> We would appreciate it if you could submit them via github issues to let us see what we've left

![App](Assets/App.png)

For each distro , there are two buttons. 

- The first button will let you shrink it's disk.
- The second button will reveal the `.vhdx` file in the explorer.

You can shrink all distros in the list using the `Shrink All` button.

And you can select custom `.vhdx` file and shrink them using the `Shrink Custom...` button.

Before the actual work begins, you need to confirm the four points.

![Dialog](Assets/Dialog.png)

If you click the `Proceed` button, we will shrink the disk for you.

![Processing](Assets/Processing.png)

That's all. It's quite simple!

## `distros.json` Format

An array of objects like this:

```json
{
    "identifier": "identifier",
    "icon": 3521,
    "keywords": [ "identifierwsl" ],
    "paths": [ "PATH" ]
}
```

- `identifier`: The distro name you can see when you run `wsl -l -v`
- `icon`: The icon id for the distro. (We use the `PackIcon` from `MaterialDesignInXAML`.)
- `keywords`: The keywords that identify this distro from other apps. (In terms of `PackageName`) 
  - You can find it by browsing `%localappdata%\Packages`
  - It should be in lowercase.
  - Most of the time, one keyword is enough.
  - If the distro isn't installed as a package, this property should be null.
- `paths`: The paths where the `.vhdx`  file located.
  - Some variables can be used within path
    - `{User}`: The user name of current user.
  - If the distro is installed as a package, using the keyword way and making `paths` null is recommended.

