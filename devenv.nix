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

    git-hooks.hooks.formatting = {
        enable = true;
        
        # The name of the hook (appears on the report table):
        name = "Formatting of .NET files";

        # The command to execute (mandatory):
        entry = "make format";
        
        # The pattern of files to run on (default: "" (all))
        # see also https://pre-commit.com/#hooks-files
        files = "\\.(cs)$";
    };

    # https://devenv.sh/tests/
    # https://devenv.sh/services/
    # https://devenv.sh/tasks/
}
