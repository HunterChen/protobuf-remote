@echo off

if "%1" == "" goto Help

rmdir /s /q temp_release
del "%1.zip"

md temp_release
md temp_release\%1

robocopy Source "temp_release\%1\Source" *.cs *.proto *.sln *.csproj /s
robocopy Samples "temp_release\%1\Samples" *.cs *.proto *.sln *.csproj /s
robocopy Bin "temp_release\%1\Bin" ProtoBufRemote.dll ProtoBufRemote.pdb /s

pushd temp_release
7z a "%1.zip" "%1\"
popd

copy "temp_release\%1.zip" .
rmdir /s /q temp_release

goto End

:Help

echo. Takes one parameter, the name of the release, e.g.
echo.   release.bat protobuf-remote-net-1.0

:End