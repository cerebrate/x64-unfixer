x64-unfixer
===========

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cerebrate/x64-unfixer?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

NOTE: The best way to install x64-unfixer is via Chocolatey: https://chocolatey.org/packages/x64-unfixer

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

License
-------

x64-unfixer is in the public domain.

FAQ
---

*I need more information on how to make this work. Can you help me?*

Well, I could, but look, if you can't figure it out from the intentionally minimal set of instructions supplied above, you probably oughtn't to be trying to run unsupported mods on an unstable build in the first place, savvy?

I'm trying to provide an workaround for people who already know what they're doing, here, not make a rod for my own back and, more importantly, for those of the original mod authors who don't want to hear about problems they can't realistically fix any more than I do. And, no offense, but people who can't work it out themselves are demonstrably more likely to ignore the bit up top about how "THIS IS A USE-AT-OWN-RISK TOOL" and bug said people about these things.

Sorry.

*This is a crack! You've published a crack!*

Yes, yes it is. Yes, yes I have.

*You're a bad, bad man.*

And when you've never ripped a CD, de-DRMed an e-book, music, video, or other media file, run a pirated version of a game that you bought so that SecuROM or other copy-protection wouldn't break, slow, etc., your computer, removed the spy/adware that came packaged with some free software, or run a web-based ad-blocker, you can get back to me on the morality of patching your way around things you find inconvenient and the obligation to always honor the supplier's intentions.

*That's completely different!*

At this point, you are almost certainly going to advance one of two arguments.

If it's the "but those are all big, bad evil corporations so it's all right when we do it to *them*" argument, go ask your mother to give you a fresh primer in the thing we call "hypocrisy".

If it's the "but using them is okay, it's just writing them and letting other people use them that's wrong" argument, let me just say that your morality is fascinating to me, but I ain't buying it.

*But...*

Go away.

Also Available
--------------

If you're looking for a UI to help you use x64-unfixer on multiple mods at once, jrodrigv has written one which you can find  here:

https://github.com/jrodrigv/KSPx64TotalUnfixer/
