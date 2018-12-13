<a name="1.0.7"></a>
## 1.0.7 (2018-12-13)


### Features

* Add VS 2019 support ([d1eacf0](https://github.com/vchirikov/GoToDnSpy/commit/d1eacf0))

<a name="1.0.6"></a>
## 1.0.6

* Added support for net framework referenced assemblies. [issue](https://github.com/verysimplenick/GoToDnSpy/issues/2)

<a name="1.0.5"></a>
## 1.0.5

* Some changes

<a name="1.0.4"></a>
## 1.0.4

* Added .Net Core projects support

<a name="1.0.3"></a>
## 1.0.3

* Added navigate support for Roslyn many others `*DeclarationSyntax`
    (less "is not a valid identifier" errors)
* Added support for referenced project output (i.e. your referenced class library project)
    (no more 'not found' errors on this type of reference)

<a name="1.0.2"></a>
## 1.0.2
* Added navigate support for Roslyn `PropertyDeclarationSyntax`
    (less "is not a valid identifier" errors)
* Added support for solution projects output, now we can easy see CIL of compiled project!
    (must build project first)
* Fixed support EnvDTE fake project "Miscellaneous Files"

<a name="1.0.1"></a>
## 1.0.1

* Fixed error if empty dnSpy path

<a name="1.0"></a>
## 1.0

* First public release
