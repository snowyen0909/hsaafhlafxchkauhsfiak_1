#!/bin/bash
# Builds a static libopenmpt (arm64 iOS device) for the MO3 interactive music.
# Runs on the macOS CI runner. Output: src/Lawn_iOS/native/libopenmpt.a
set -euo pipefail
VER=0.8.10
SHA=95073a10d33d0ed4352f41b68edccb9636002d2626f9fd2e00b191fcaabdd876
TAR="libopenmpt-${VER}+release.makefile.tar.gz"
WORK="${RUNNER_TEMP:-/tmp}/openmpt"
OUT="$(pwd)/src/Lawn_iOS/native"
mkdir -p "$WORK" "$OUT"
cd "$WORK"
curl -fsSL -o "$TAR" "https://lib.openmpt.org/files/libopenmpt/src/${TAR}"
echo "${SHA}  ${TAR}" | shasum -a 256 -c -
tar xzf "$TAR"
cd "libopenmpt-${VER}+release"
SDK=$(xcrun --sdk iphoneos --show-sdk-path)
CLANG=$(xcrun --sdk iphoneos -f clang)
CLANGXX=$(xcrun --sdk iphoneos -f clang++)
ARCHFLAGS="-arch arm64 -isysroot ${SDK} -miphoneos-version-min=15.0"
make -j"$(sysctl -n hw.ncpu)" CONFIG=clang \
  CC="${CLANG} ${ARCHFLAGS}" CXX="${CLANGXX} ${ARCHFLAGS}" LD="${CLANGXX} ${ARCHFLAGS}" AR="$(xcrun -f ar)" \
  SHARED_LIB=0 STATIC_LIB=1 EXAMPLES=0 OPENMPT123=0 TEST=0 \
  NO_ZLIB=1 NO_MPG123=1 NO_OGG=1 NO_VORBIS=1 NO_VORBISFILE=1 \
  NO_PORTAUDIO=1 NO_PORTAUDIOCPP=1 NO_PULSEAUDIO=1 NO_SDL2=1 NO_FLAC=1 NO_SNDFILE=1 \
  bin/libopenmpt.a
cp bin/libopenmpt.a "$OUT/libopenmpt.a"
lipo -info "$OUT/libopenmpt.a"
nm -g "$OUT/libopenmpt.a" 2>/dev/null | grep -c "openmpt_module_ext_create_from_memory" || true
