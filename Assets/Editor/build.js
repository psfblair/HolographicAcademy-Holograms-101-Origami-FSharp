import System.Diagnostics;

class Autobuild {

    static function Build() {
        // Get filename.
        var path = "E:/holodevelop/Origami/App";
        var levels : String[] = [ "Assets/Origami.unity" ];
        
        // Build player.
        BuildPipeline.BuildPlayer(levels, path, BuildTarget.WSAPlayer, BuildOptions.None);
    }
}