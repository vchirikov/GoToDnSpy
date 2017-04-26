# Release notes

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