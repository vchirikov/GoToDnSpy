# GoToDnSpy

## Introduction

Sometimes I need something command like Resharper has "Navigate To Decompiled Sources", but I don't want
buy and install it only for this command. So I created this plugin for quick access to dnSpy.

[dnSpy](https://github.com/0xd4d/dnSpy/) is the best tool for .net reseacher.   
You can [download latest dnSpy build](https://ci.appveyor.com/project/0xd4d/dnspy/branch/master/artifacts) from CI.

## Installing

Plugin can be found in [new Visual Studio marketplace](https://marketplace.visualstudio.com/vsgallery/02d8452f-a0ec-4cbc-adc7-d050c0f43d54).  
[Old Visual Studio gallery link](https://visualstudiogallery.msdn.microsoft.com/02d8452f-a0ec-4cbc-adc7-d050c0f43d54?redir=0)

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

## Release notes

- 1.0.2
    + Added navigate support for Roslyn `PropertyDeclarationSyntax`  
        (less "is not a valid identifier" errors)
    + Added support for solution projects output, now we can easy see CIL of compiled project! 
        (must build project first)
    * Fix support EnvDTE fake project "Miscellaneous Files"
- 1.0.1
    * Fix error if empty dnSpy path
- 1.0
    + First public release
