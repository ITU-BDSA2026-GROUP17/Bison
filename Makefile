.PHONY: all build clean zip

BUILD_PROJECT := Bison.CLI.Client
DATABASE_PROJECT := Bison.Database
BASE_PATH := src/$(BUILD_PROJECT)/
DATABASE_PATH := src/$(DATABASE_PROJECT)/
OS := linux win osx
ARCH := x64 arm64
PLATFORMS := $(foreach os,$(OS),$(foreach arch,$(ARCH),$(os)-$(arch)))

get_ext = $(if $(filter win-%,$(1)),.exe)

ifdef BISON_VERSION
version := $(BISON_VERSION:v%=%)
else
version := 0.0.0
endif

all: build

${BASE_PATH}bin/Release/net8.0/%/publish/$(BUILD_PROJECT): ${BASE_PATH}Program.cs ${BASE_PATH}${BUILD_PROJECT}.csproj
	@echo "Publishing for $*..."
	dotnet publish ${BASE_PATH} -c Release -r $* -p:FileVersion=${version}

${BASE_PATH}bin/Release/net8.0/win-%/publish/$(BUILD_PROJECT).exe: ${BASE_PATH}Program.cs ${BASE_PATH}${BUILD_PROJECT}.csproj
	@echo "Publishing for win-$*..."
	dotnet publish ${BASE_PATH} -c Release -r win-$* -p:FileVersion=${version}

${DATABASE_PATH}bin/Release/net8.0/%/publish/$(DATABASE_PROJECT): ${DATABASE_PATH}Program.cs ${DATABASE_PATH}${DATABASE_PROJECT}.csproj
	@echo "Publishing for $*..."
	dotnet publish ${DATABASE_PATH} -c Release -r $* -p:FileVersion=${version}

${DATABASE_PATH}bin/Release/net8.0/win-%/publish/$(DATABASE_PROJECT).exe: ${DATABASE_PATH}Program.cs ${DATABASE_PATH}${DATABASE_PROJECT}.csproj
	@echo "Publishing for win-$*..."
	dotnet publish ${DATABASE_PATH} -c Release -r win-$* -p:FileVersion=${version}

build: $(foreach platform,$(PLATFORMS),${BASE_PATH}bin/Release/net8.0/$(platform)/publish/$(BUILD_PROJECT)$(call get_ext,$(platform)) ${DATABASE_PATH}bin/Release/net8.0/$(platform)/publish/$(DATABASE_PROJECT)$(call get_ext,$(platform)))

zip:
	rm -rf data/*.zip
	@mkdir -p data
	@$(foreach platform,$(PLATFORMS),\
		ext="$(call get_ext,$(platform))"; \
		zip -j data/bison-$(platform).zip ${BASE_PATH}bin/Release/net8.0/$(platform)/publish/$(BUILD_PROJECT) ${DATABASE_PATH}bin/Release/net8.0/$(platform)/publish/$(DATABASE_PROJECT)$$ext; \
	)

ci: build zip

clean:
	rm -rf ./src/**/bin/ ./src/**/obj/ ./test/**/bin/ ./test/**/obj/ ./data/*.zip
