#!/usr/bin/env python3
"""Prepare the Android Content folder for iOS.

iOS MonoGame plays Song assets through AVAudioPlayer, which cannot decode Ogg Vorbis.
This converts every Song's .ogg to .m4a (AAC) with ffmpeg and rewrites the Song .xnb
to point at the .m4a. Everything else is copied unchanged.

usage: prepare_ios_content.py <android assets/Content> <output Content>
"""
import os, shutil, struct, subprocess, sys

try:
    import lz4.block
except ImportError:
    lz4 = None

def read_xnb(path):
    d = open(path, 'rb').read()
    flags = d[5]
    if flags & 0x40:
        size = struct.unpack('<I', d[10:14])[0]
        body = lz4.block.decompress(d[14:], uncompressed_size=size)
    elif flags & 0x80:
        return d, None
    else:
        body = d[10:]
    return d, body

def r7(b, i):
    v = s = 0
    while True:
        c = b[i]; i += 1
        v |= (c & 0x7f) << s; s += 7
        if not c & 0x80:
            return v, i

def w7(v):
    out = bytearray()
    while True:
        c = v & 0x7f; v >>= 7
        out.append(c | (0x80 if v else 0))
        if not v:
            return bytes(out)

def main(src, dst):
    if os.path.exists(dst):
        shutil.rmtree(dst)
    shutil.copytree(src, dst)
    n = 0
    for root, _, files in os.walk(dst):
        for f in files:
            if not f.endswith('.xnb'):
                continue
            p = os.path.join(root, f)
            raw, body = read_xnb(p)
            if body is None or b'SongReader' not in body[:80]:
                continue
            i = 0
            nreaders, i = r7(body, i)
            for _ in range(nreaders):
                l, i = r7(body, i); i += l + 4
            shared, i = r7(body, i)
            idx, i = r7(body, i)
            l, j = r7(body, i)
            name = body[j:j + l].decode('utf-8')
            if not name.lower().endswith('.ogg'):
                continue
            newname = name[:-4] + '.m4a'
            ogg = os.path.join(root, name)
            m4a = os.path.join(root, newname)
            subprocess.run(['ffmpeg', '-v', 'error', '-y', '-i', ogg, '-c:a', 'aac', '-b:a', '160k', m4a], check=True)
            nb = newname.encode('utf-8')
            body = body[:i] + w7(len(nb)) + nb + body[j + l:]
            header = bytearray(raw[:10])
            header[5] = 0  # uncompressed
            struct.pack_into('<I', header, 6, 10 + len(body))
            open(p, 'wb').write(bytes(header) + body)
            n += 1
            print('song:', os.path.relpath(p, dst), '->', newname)
    print(f'{n} songs converted')

if __name__ == '__main__':
    main(sys.argv[1], sys.argv[2])
