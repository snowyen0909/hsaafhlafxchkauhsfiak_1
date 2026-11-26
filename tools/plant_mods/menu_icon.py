# Draws ContentMod/dorm/ui2/menu_mods.png: the main-menu 「模组」 icon (two seed cards and a puzzle piece),
# in the style of menu_dorm / menu_wardrobe (cartoon, dark outline). Drawn at 4x and scaled down.
import sys
from PIL import Image, ImageDraw, ImageFilter

K = 4
W, H = 209 * K, 174 * K
OUT = (74, 44, 26, 255)
LW = 7 * K


def card(fill, frame, window, sprout):
    w, h = 92 * K, 120 * K
    im = Image.new('RGBA', (w + 2 * LW, h + 2 * LW), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    o = LW
    d.rounded_rectangle((o, o, o + w, o + h), 14 * K, fill=frame, outline=OUT, width=LW)
    m = 11 * K
    d.rounded_rectangle((o + m, o + m, o + w - m, o + h - 38 * K), 9 * K, fill=window, outline=OUT, width=4 * K)
    # bottom strip with a sun coin
    d.ellipse((o + w - 40 * K, o + h - 34 * K, o + w - 12 * K, o + h - 6 * K), fill=(255, 214, 64, 255), outline=OUT, width=4 * K)
    d.rounded_rectangle((o + 14 * K, o + h - 26 * K, o + w - 48 * K, o + h - 16 * K), 4 * K, fill=fill)
    if sprout:
        cx, by = o + w // 2, o + h - 44 * K
        d.line((cx, by, cx, by - 38 * K), fill=(60, 130, 40, 255), width=6 * K)
        d.ellipse((cx - 34 * K, by - 54 * K, cx + 2 * K, by - 30 * K), fill=(120, 200, 70, 255), outline=OUT, width=4 * K)
        d.ellipse((cx - 2 * K, by - 62 * K, cx + 34 * K, by - 36 * K), fill=(140, 214, 84, 255), outline=OUT, width=4 * K)
        d.ellipse((cx - 22 * K, by - 8 * K, cx + 22 * K, by + 6 * K), fill=(150, 98, 58, 255))
    return im


def puzzle(size):
    s = size
    im = Image.new('RGBA', (s + 2 * LW, s + 2 * LW), (0, 0, 0, 0))
    mask = Image.new('L', im.size, 0)
    d = ImageDraw.Draw(mask)
    o = LW
    b = int(s * 0.18)          # knob radius
    body = (o, o + b, o + s - b, o + s)
    d.rounded_rectangle(body, 10 * K, fill=255)
    cx = (body[0] + body[2]) // 2
    d.ellipse((cx - b, o, cx + b, o + 2 * b), fill=255)             # top knob
    cy = (body[1] + body[3]) // 2
    d.ellipse((body[2] - b, cy - b, body[2] + b, cy + b), fill=255)  # right knob
    edge = mask.filter(ImageFilter.MaxFilter(2 * (LW // 2) + 1))
    im.paste(OUT, (0, 0), edge)
    gold = Image.new('RGBA', im.size, (255, 200, 60, 255))
    im.paste(gold, (0, 0), mask)
    hl = Image.new('L', im.size, 0)
    ImageDraw.Draw(hl).ellipse((o + 8 * K, o + b + 6 * K, o + s // 2, o + b + s // 3), fill=110)
    hl = Image.composite(hl, Image.new('L', im.size, 0), mask)
    im.paste((255, 245, 190, 255), (0, 0), hl)
    return im


def star(d, cx, cy, r):
    pts = []
    import math
    for i in range(8):
        a = math.pi / 4 * i - math.pi / 2
        rr = r if i % 2 == 0 else r * 0.32
        pts.append((cx + rr * math.cos(a), cy + rr * math.sin(a)))
    d.polygon(pts, fill=(255, 218, 60, 255), outline=OUT)


def main(out):
    im = Image.new('RGBA', (W, H), (0, 0, 0, 0))
    back = card((110, 170, 70, 255), (168, 220, 110, 255), (205, 240, 180, 255), False).rotate(14, expand=True, resample=Image.BICUBIC)
    front = card((230, 200, 140, 255), (255, 244, 214, 255), (190, 232, 255, 255), True).rotate(-8, expand=True, resample=Image.BICUBIC)
    im.alpha_composite(back, (18 * K, 8 * K))
    im.alpha_composite(front, (66 * K, 14 * K))
    pz = puzzle(70 * K)
    im.alpha_composite(pz, (124 * K, 84 * K))
    d = ImageDraw.Draw(im)
    star(d, 22 * K, 128 * K, 14 * K)
    star(d, 190 * K, 34 * K, 12 * K)
    im = im.resize((W // K, H // K), Image.LANCZOS)
    im.save(out)


if __name__ == '__main__':
    main(sys.argv[1])
