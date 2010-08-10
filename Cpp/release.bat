@echo off

if "%1" == "" goto Help

md Release
md Release\%1

robocopy Source "Release\%1\Source" *.h *.cpp *.cc *.proto *.sln *.vcxproj *.vcxproj.filters /s
robocopy Lib "Release\%1\Lib" *.lib *.pdb /s

pushd Release
7z a "%1.zip" "%1\"
popd

goto End

:Help

echo. Takes one parameter, the name of the release, e.g.
echo.   release.bat protobuf-remote-1.0

:End