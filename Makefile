.PHONY: all build clean

ifdef BISON_VERSION
version := $(BISON_VERSION:v%=%)
else
version := 0.0.0
endif

all: build

bin/Release/net8.0/linux-x64/publish/Bison: Program.cs Bison.csproj
	dotnet publish -r linux-x64 -p FileVersion=${version}

bin/Release/net8.0/linux-arm64/publish/Bison: Program.cs Bison.csproj
	dotnet publish -r linux-arm64 -p FileVersion=${version}

bin/Release/net8.0/win-x64/publish/Bison.exe: Program.cs Bison.csproj
	dotnet publish -r win-x64 -p FileVersion=${version}

bin/Release/net8.0/win-arm64/publish/Bison.exe: Program.cs Bison.csproj
	dotnet publish -r win-arm64 -p FileVersion=${version}

bin/Release/net8.0/osx-x64/publish/Bison: Program.cs Bison.csproj
	dotnet publish -r osx-x64 -p FileVersion=${version}

bin/Release/net8.0/osx-arm64/publish/Bison: Program.cs Bison.csproj
	dotnet publish -r osx-arm64 -p FileVersion=${version}

build: bin/Release/net8.0/linux-x64/publish/Bison bin/Release/net8.0/linux-arm64/publish/Bison bin/Release/net8.0/win-x64/publish/Bison.exe bin/Release/net8.0/win-arm64/publish/Bison.exe bin/Release/net8.0/osx-x64/publish/Bison bin/Release/net8.0/osx-arm64/publish/Bison

clean:
	rm -r ./bin/ ./obj/
