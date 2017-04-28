# GoToDnSpy

## Introduction

Sometimes I need something command like Resharper has "Navigate To Decompiled Sources", but I don't want
buy and install it only for this command. So I created this plugin for quick access to dnSpy.

[dnSpy](https://github.com/0xd4d/dnSpy/) is the best tool for .net reseacher.   
You can [download latest dnSpy build](https://ci.appveyor.com/project/0xd4d/dnspy/branch/master/artifacts) from CI.

## Installing

Plugin can be found in [Visual Studio marketplace](https://marketplace.visualstudio.com/vsgallery/02d8452f-a0ec-4cbc-adc7-d050c0f43d54).  

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

## Release notes

- 1.0.4
    + Added .Net Core projects support
- 1.0.3
    + Added navigate support for Roslyn many others `*DeclarationSyntax`  
        (less "is not a valid identifier" errors)
    + Add support for referenced project output (i.e. your referenced class library project)
      (no more 'not found' errors on this type of reference)
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
