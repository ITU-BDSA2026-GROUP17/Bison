{ pkgs, ... }:

{
    languages.dotnet = {
        enable = true;
    };
    # https://devenv.sh/packages/
    packages = with pkgs; [
        dotnet-aspnetcore
    ];
    scripts.watcher = {
        exec = ''
            watchexec -c -e cs \
            "cargo clippy && cargo test && cargo run"
        '';
        packages = [ pkgs.watchexec ];
    };
    
    # https://devenv.sh/basics/
    enterShell = '''';
    
    # https://devenv.sh/tests/
    # https://devenv.sh/services/
    # https://devenv.sh/tasks/
}
