# Each tile sample (WHOLE: one tile per cell, copied into all four slots) (an iso block showing 2x2 cells) → atlas 260x220: 4 cell slots (130x66) at (qi*130, qj*66),
# 2 left-edge pieces (64x44) at (qi*64, 132), 2 right-edge pieces at (qj*64, 176).
import numpy as np, json
from PIL import Image, ImageDraw
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/tiles/'
from cut_floors import DM, LM, RM, masked, affine, CW, CH, EW, EH
SS=4
def warp(sheet, dst_pts, src_pts, size):
    # supersampled affine: render at SSx then downsample
    W,H=size
    dst=[(x*SS,y*SS) for x,y in dst_pts]
    c=affine(dst,src_pts)
    big=sheet.transform((W*SS,H*SS),Image.AFFINE,tuple(c),resample=Image.BICUBIC)
    return big.resize((W,H),Image.LANCZOS)
WHOLE={'redcarpet','bluecarpet','flowertile'}
def cut(name, g, sheet, out):
    L,R,T=np.array(g['L']),np.array(g['R']),np.array(g['T']); B=L+R-T
    h=g['h']-1.0
    whole=name in WHOLE
    u=(R-T)/(1 if whole else 2); v=(L-T)/(1 if whole else 2)
    at=Image.new('RGBA',(2*CW,2*CH+2*EH),(0,0,0,0))
    for qi in range(2):
        for qj in range(2):
            top=T+(0 if whole else qi)*u+(0 if whole else qj)*v
            # slot: diamond top (65,1), right (129,33), left (1,33) — 1 px bleed around 128x64
            tile=warp(sheet,[(65,1),(129,33),(1,33)],[tuple(top),tuple(top+u),tuple(top+v)],(CW,CH))
            at.alpha_composite(masked(tile,DM),(qi*CW,qj*CH))
    for qi in range(2):
        s=L+(0 if whole else qi)*u; e=s+u
        le=warp(sheet,[(0,0),(64,32),(0,11)],[tuple(s),tuple(e),tuple(s+[0,h])],(EW,EH))
        at.alpha_composite(masked(le,LM),(qi*EW,2*CH))
    for qj in range(2):
        r=R+(0 if whole else qj)*v; b=r+v
        re=warp(sheet,[(0,32),(64,0),(0,43)],[tuple(b),tuple(r),tuple(b+[0,h])],(EW,EH))
        at.alpha_composite(masked(re,RM),(qj*EW,2*CH+EH))
    at.save(D+out,optimize=True)
if __name__=='__main__':
    sheet=Image.open('src/floors_sheet.png').convert('RGBA')
    g6=json.load(open('sample_geo.json'))
    for n,g in g6.items(): cut(n,g,sheet,f'{n}_t.png')
    g10=json.load(open('tile_geo.json'))
    for n,g in g10.items():
        gg=dict(L=g['L'],R=g['R'],T=g['T'],h=float(g['bottom'])-(g['L'][1]+g['R'][1]-g['T'][1]))
        cut(n,gg,sheet,f'{n}.png')
    print('done')
