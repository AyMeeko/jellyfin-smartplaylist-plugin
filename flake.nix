{
  description = "Jellyfin Plugin Project";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = { self, nixpkgs }:
    let
      supportedSystems = [ "x86_64-linux" "x86_64-darwin" ];
      forAllSystems = f: nixpkgs.lib.genAttrs supportedSystems (system: f system);
      pkgs = forAllSystems (system: import nixpkgs { inherit system; });
    in
    {
      devShell = forAllSystems (system:
        pkgs.${system}.mkShell {
          buildInputs = [
            pkgs.${system}.dotnet-sdk_8
          ];
        });

      packages = forAllSystems (system:
        {
          default = pkgs.${system}.stdenv.mkDerivation {
            name = "jellyfin-plugin";
            src = ./.;

            buildInputs = [
              pkgs.${system}.dotnet-sdk_8
            ];

            buildPhase = ''
              dotnet build --configuration Release
            '';

            installPhase = ''
              mkdir -p $out/bin
              cp bin/Release/net8.0/*.dll $out/bin/
            '';
          };
        });
    };
}
