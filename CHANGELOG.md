<a name="2.0.1"></a>
## 2.0.2 (2025-03-03)

### Build

* Update packages, fixes [#18](https://github.com/vchirikov/GoToDnSpy/issues/14)  


<a name="2.0.1"></a>
## 2.0.1 (2020-11-10)

### Bug Fixes

* Fix for the "member name wasn't found" problem. ([dbde45e](https://github.com/vchirikov/GoToDnSpy/commit/dbde45e)), fixes [#14](https://github.com/vchirikov/GoToDnSpy/issues/14)  


<a name="2.0.0"></a>
## 2.0.0 (2020-11-10)

### Features

* VS 2022 support ([5604590](https://github.com/vchirikov/GoToDnSpy/commit/56045905b9cc94c7eb71cfa6813b3ea90cfdd6d2)), fixes [#13](https://github.com/vchirikov/GoToDnSpy/issues/13)  
* Support of VS2019 is dropped, because [PIA layer was changed a lot](https://developercommunity.visualstudio.com/t/dte-events-not-working-in-vs2022/1455126)  

### Bug Fixes

* Better search through the latest aspnetcore/runtime sources

<a name="1.1.5"></a>
## 1.1.5 (2020-11-10)

### Features

* Open selection in new tab and make "--dont-load-files" optional ([ea467795](https://github.com/vchirikov/GoToDnSpy/pull/12/commits/ea46779554f36c5020f4bb9e0371979d9e350ee3))

<a name="1.1.4"></a>
## 1.1.4 (2020-09-16)

### Bug Fixes

* Remove vs 2017 support ([b66bc8ea](https://github.com/vchirikov/GoToDnSpy/commit/b66bc8ea8fb5b60cad5a89548bb3acd831728a51))

<a name="1.1.3"></a>
## 1.1.3 (2020-##-##)

### Features

* Add System.Buffers source support ([2a1f09ef](https://github.com/vchirikov/GoToDnSpy/commit/2a1f09ef))

<a name="1.1.2"></a>
## 1.1.2 (2020-03-07)

### Features

* Add netstandard.library and microsoft.netcore.app.ref redirect to [source.dot.net](https://source.dot.net) ([855a684f](https://github.com/vchirikov/GoToDnSpy/commit/855a684f))

<a name="1.1.0"></a>
## 1.1.0 (2020-03-07)

### Features

* Now the plugin check if symbol is from netstandard.dll, and if so sources will be searched on [source.dot.net](https://source.dot.net) ([5a4c1592](https://github.com/vchirikov/GoToDnSpy/commit/5a4c1592))
* Required nupkg packages have been updated.
* Small bugfixes and improvements.

### Bug Fixes

* I think that [#9](https://github.com/vchirikov/GoToDnSpy/issues/9) was fixed due to packages update.

<a name="1.0.10"></a>
## 1.0.10 (2019-09-11)

### Features

* Migrate to AsyncPackage, update dependencies ([f58a5c3](https://github.com/vchirikov/GoToDnSpy/commit/f58a5c3))



<a name="1.0.9"></a>
## 1.0.9 (2019-09-11)

### Features

* Add context menu command ([0ed689d](https://github.com/vchirikov/GoToDnSpy/commit/0ed689d)), closes [#8](https://github.com/vchirikov/GoToDnSpy/issues/8)

<a name="1.0.8"></a>
## 1.0.8 (2019-02-07)


### Bug Fixes

* Fix issue with getting IWpfTextView ([455f493](https://github.com/vchirikov/GoToDnSpy/commit/455f493)), closes [#5](https://github.com/vchirikov/GoToDnSpy/issues/5) [#7](https://github.com/vchirikov/GoToDnSpy/issues/7)


### Features

* Add VS 2019 support ([d1eacf0](https://github.com/vchirikov/GoToDnSpy/commit/d1eacf0))



<a name="1.0.7"></a>
## 1.0.7 (2018-12-13)


### Features

* Add VS 2019 support ([d1eacf0](https://github.com/vchirikov/GoToDnSpy/commit/d1eacf0))

<a name="1.0.6"></a>
## 1.0.6

* Added support for net framework referenced assemblies. [issue](https://github.com/vchirikov/GoToDnSpy/issues/2)

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
