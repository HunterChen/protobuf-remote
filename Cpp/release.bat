@echo off

if "%1" == "" goto Help

rmdir /s /q temp_release
del "%1.zip"

md temp_release
md temp_release\%1

robocopy Source "temp_release\%1\Source" *.h *.cpp *.cc *.proto *.sln *.vcxproj *.vcxproj.filters /s
robocopy Lib "temp_release\%1\Lib" *.lib *.pdb /s

pushd temp_release
7z a "%1.zip" "%1\"
popd

copy "temp_release\%1.zip" .
rmdir /s /q temp_release

goto End

:Help

echo. Takes one parameter, the name of the release, e.g.
echo.   release.bat protobuf-remote-cpp-1.0

:End