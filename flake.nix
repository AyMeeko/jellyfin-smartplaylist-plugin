{
  description = "Jellyfin Plugin Project";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = { self, nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs { inherit system; };
    in
    {
      devShell.${system} = pkgs.mkShell {
        buildInputs = [
          pkgs.dotnet-sdk_8
        ];
      };

      packages.${system}.default = pkgs.stdenv.mkDerivation {
        name = "jellyfin-plugin";
        src = ./.;

        buildInputs = [
          pkgs.dotnet-sdk_8
        ];

        buildPhase = ''
          dotnet build --configuration Release
        '';

        installPhase = ''
          mkdir -p $out/bin
          cp bin/Release/net8.0/*.dll $out/bin/
        '';
      };
    };
}
