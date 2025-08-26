# setup-unity-smartmerge.ps1
# Sets up Unity Smart Merge (YAMLMerge) with fallback

# --- CONFIGURE THIS ---
$unityVersion = "6000.2.1f1"
$unityYamlMerge = "C:/Program Files/Unity/Hub/Editor/$unityVersion/Editor/Data/Tools/UnityYAMLMerge.exe"
# ----------------------

git config merge.unityyaml.name "Unity Smart Merge (YAMLMerge)"
git config merge.unityyaml.driver "`"$unityYamlMerge`" merge -p %O %B %A %A || exit 0"

Write-Output "✅ Unity Smart Merge configured for Git (Unity $unityVersion) with fallback."

