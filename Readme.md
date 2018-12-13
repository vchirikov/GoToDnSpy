# GoToDnSpy

## Introduction

Sometimes I need something like Resharper command "Navigate To Decompiled Sources", but I don't want to buy and install R# only for this command. So I created this plugin for quick access to dnSpy.
BTW, after VS 2017.6 Preview can do it with included ILSpy.



[dnSpy](https://github.com/0xd4d/dnSpy/) is the best tool for .net reseacher.
You can [download latest dnSpy build](https://ci.appveyor.com/project/0xd4d/dnspy/branch/master/artifacts) from CI.

## Installing

Plugin can be found in [Visual Studio marketplace](https://marketplace.visualstudio.com/items?itemName=VladimirChirikov.GoToDnSpy).

**Please send positive feedback if you liked the extension :)**

## Preview

![Using GoToDnSpy](images/preview.gif)


## Options

For work plugin need put path to installed dnSpy in options page.
You can [download latest dnSpy build](https://ci.appveyor.com/project/0xd4d/dnspy/branch/master/artifacts) from CI.

![Options GoToDnSpy](images/options.png)

## Using

Place cursor at referenced code (method, event, etc) and run "GoTo dnSpy..." command.

GoToDnSpy command can be found in Visual Studio tools menu

![Tools menu with GoTo dnSpy](images/tools_menu.png)

You can add and use shortcut for fast run command

![Shortcut example](images/shortcut.png)


## ISSUES

"is not a valid identifier" errors means that for now plugin can't parse syntax tree under caret, please create issue on github with sample of code, it's help improve extension.
