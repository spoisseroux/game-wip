#!/bin/bash
# setup-unity-smartmerge.sh
# Sets up Unity Smart Merge (YAMLMerge) with fallback

# --- CONFIGURE THIS ---
UNITY_VERSION="6000.4.4f1"
UNITYYAMLMERGE="/Applications/Unity/Hub/Editor/$UNITY_VERSION/Unity.app/Contents/Tools/UnityYAMLMerge"
# ----------------------

git config merge.unityyaml.name "Unity Smart Merge (YAMLMerge)"
git config merge.unityyaml.driver "'$UNITYYAMLMERGE' merge -p %O %B %A %A || true"

echo "✅ Unity Smart Merge configured for Git (Unity $UNITY_VERSION) with fallback."

