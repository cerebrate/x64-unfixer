x64-unfixer
===========

Unfixes KSP mods that use CompatibilityChecker to prevent themselves from running on Windows x64.

Run:

x64-unfixer c:\path\to\the\mods\plugin.dll

...and x64-unfixer will rename the old .dll to .old to give you a backup, then patch the .dll in place to prevent
CommunityChecker from detecting that it's running on x64.

If you have problems and think this tool might be responsible, simply delete the .dll and rename the .old back in its
place.

I am not responsible for any use you may make of x64-unfixer, and I can't support anything you patch with it. Also,
if you patch their mods, the original authors of those mods can't support them either, and if they supported them
on x64 in the first place, this tool would be unnecessary.

Anyway, DON'T SEND THEM SUPPORT REQUESTS FOR VERSIONS YOU'VE PATCHED, OKAY? THIS IS A USE-AT-OWN-RISK TOOL.

License:

x64-unfixer is in the public domain.
