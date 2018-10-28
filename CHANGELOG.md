# 1.0.6

    + Added support for net framework referenced assemblies. [issue](https://github.com/verysimplenick/GoToDnSpy/issues/2)
# 1.0.5

    + Some changes
# 1.0.4

    + Added .Net Core projects support
# 1.0.3

    + Added navigate support for Roslyn many others `*DeclarationSyntax`
        (less "is not a valid identifier" errors)
    + Added support for referenced project output (i.e. your referenced class library project)
      (no more 'not found' errors on this type of reference)
# 1.0.2

    + Added navigate support for Roslyn `PropertyDeclarationSyntax`
        (less "is not a valid identifier" errors)
    + Added support for solution projects output, now we can easy see CIL of compiled project!
        (must build project first)
    * Fixed support EnvDTE fake project "Miscellaneous Files"
# 1.0.1

    * Fixed error if empty dnSpy path
# 1.0

    + First public release
