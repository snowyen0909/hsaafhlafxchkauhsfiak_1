#!/bin/bash
# Wraps the freshly built .app into an unsigned IPA: tools/package_ipa.sh <output.ipa>
set -euo pipefail
OUT="$1"
APP=$(find src/Lawn_iOS/bin/Release -name "*.app" -type d -maxdepth 4 | head -1)
echo "App bundle: $APP"
test -n "$APP"
du -sh "$APP"
rm -rf out && mkdir -p out/Payload
cp -R "$APP" out/Payload/
(cd out && zip -qry "../$OUT" Payload)
rm -rf out
ls -lh "$OUT"
