{ pkgs, ... }:

let
  # Combine .NET 8 and .NET 10 environments 
  my-dotnet-bundle = with pkgs.dotnetCorePackages; combinePackages [
    sdk_8_0
    sdk_10_0
    aspnetcore_8_0
  ];
in {
    # https://devenv.sh/packages/
    packages = [
        my-dotnet-bundle
        pkgs.roslyn-ls
    ];
    env.DOTNET_ROOT = "${my-dotnet-bundle}";
    
    scripts.watcher = {
        exec = ''
            watchexec -c -e cs \
            "dotnet format && dotnet run --project src/Bison.Database"
        '';
        packages = [ pkgs.watchexec ];
    };

    # https://devenv.sh/basics/
    enterShell = '''';

    # https://devenv.sh/tests/
    # https://devenv.sh/services/
    # https://devenv.sh/tasks/
}
