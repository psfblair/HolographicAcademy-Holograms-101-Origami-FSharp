import System.Diagnostics;

class Autobuild {

    static function Build() {
        // Get filename.
        var path = "C:/Users/psfblair/Documents/holodevelop/MicrosoftAcademy/101-Origami/App";
        var levels : String[] = [ "Assets/Origami.unity" ];
        
        // Build player.
        BuildPipeline.BuildPlayer(levels, path, BuildTarget.WSAPlayer, BuildOptions.None);
    }
}