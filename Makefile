.PHONY: all build clean zip ci dafny run coverage-report

BUILD_PROJECT := Bison.Razor
BASE_PATH := src/$(BUILD_PROJECT)/
OS := linux win osx
ARCH := x64 arm64
PLATFORMS := $(foreach os,$(OS),$(foreach arch,$(ARCH),$(os)-$(arch)))

get_ext = $(if $(filter win-%,$(1)),.exe)

ifdef BISON_VERSION
version := $(BISON_VERSION:v%=%)
else
version := 0.0.0
endif

run:
	dotnet run --project ${BASE_PATH}${BUILD_PROJECT}.csproj --launch-profile https

all: build

${BASE_PATH}bin/Release/*/%/publish/$(BUILD_PROJECT): ${BASE_PATH}Program.cs ${BASE_PATH}${BUILD_PROJECT}.csproj
	@echo "Publishing for $*..."
	dotnet publish ${BASE_PATH} -c Release -r $* -p:FileVersion=${version}

${BASE_PATH}bin/Release/*/win-%/publish/$(BUILD_PROJECT).exe: ${BASE_PATH}Program.cs ${BASE_PATH}${BUILD_PROJECT}.csproj
	@echo "Publishing for win-$*..."
	dotnet publish ${BASE_PATH} -c Release -r win-$* -p:FileVersion=${version}

dafny:
	dotnet dafny translate cs --include-runtime src/Bison.Verified/*.dfy --output src/Bison.Verified/Generated.cs

coverage-report: $(wildcard test/**/coverage.cobertura.xml)
	@dotnet tool restore
	dotnet reportgenerator -reports:"test/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

build: $(foreach platform,$(PLATFORMS),${BASE_PATH}bin/Release/*/$(platform)/publish/$(BUILD_PROJECT)$(call get_ext,$(platform)))

zip:
	rm -rf dist/*.zip
	@mkdir -p dist
	$(foreach platform,$(PLATFORMS),\
		(ext="$(call get_ext,$(platform))"; CURRENT="$$(pwd)"; \
		cd ${BASE_PATH}bin/Release/*/$(platform)/publish && zip -r -9 -J $$CURRENT/dist/bison-$(platform).zip ./*); \
	)

ci: build zip

clean:
	rm -rf ./src/**/bin/ ./src/**/obj/ ./test/**/bin/ ./test/**/obj/ ./dist/*.zip
