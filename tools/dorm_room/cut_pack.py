# Tile pack (flat 1x1 squares) -> 2x2 tile atlases (260x220) like cut_quads.py, one whole tile per cell.
import numpy as np, sys
from PIL import Image, ImageDraw
sys.path.insert(0,'/home/claude/tiles2')
from cut_floors import DM, LM, RM, masked, affine, CW, CH, EW, EH
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/'
pack=Image.open('/home/claude/tiles2/pack.png').convert('RGB')
rows=[172,289,406,524,643,762]; cols=[295,378,463,548]
def square(r,c,s=72):
    y=rows[r]+(1 if (r,c)==(3,0) else 0); x=cols[c]
    return pack.crop((x+3,y+3,x+3+s,y+3+s)).resize((288,288),Image.LANCZOS)
SS=4
def warp(img, dst_pts, src_pts, size):
    W,H=size
    c=affine([(x*SS,y*SS) for x,y in dst_pts],src_pts)
    return img.transform((W*SS,H*SS),Image.AFFINE,tuple(c),resample=Image.BICUBIC).resize((W,H),Image.LANCZOS)
def edge_strip(sq, side):
    a=np.array(sq).astype(float); s=a.shape[0]
    line=a[-3:,:,:].mean(0) if side=='L' else a[::-1,-3:,:].mean(1)   # along L->B / B->R
    k=0.66 if side=='L' else 0.5
    strip=np.repeat(line[None,:,:]*k,12,axis=0)
    strip[-2:]*=0.7
    strip[:1]=np.minimum(255,line[None,:,:]*0.9)
    return Image.fromarray(np.clip(strip,0,255).astype(np.uint8)).convert('RGBA')
def build(sq, out):
    sq=sq.convert('RGBA'); s=sq.width
    at=Image.new('RGBA',(2*CW,2*CH+2*EH),(0,0,0,0))
    top=warp(sq,[(65,1),(129,33),(1,33)],[(0,0),(s,0),(0,s)],(CW,CH))
    top=masked(top,DM)
    le=edge_strip(sq,'L'); re=edge_strip(sq,'R'); n=le.width
    lp=masked(warp(le,[(0,0),(64,32),(0,11)],[(0,0),(n,0),(0,11)],(EW,EH)),LM)
    rp=masked(warp(re,[(0,32),(64,0),(0,43)],[(0,0),(n,0),(0,11)],(EW,EH)),RM)
    for q in range(2):
        for p in range(2): at.alpha_composite(top,(q*CW,p*CH))
        at.alpha_composite(lp,(q*EW,2*CH)); at.alpha_composite(rp,(q*EW,2*CH+EH))
    at.save(D+'tiles/'+out+'.png',optimize=True)
    return at
MAP={(0,0):'tatami_t',(0,2):'sakuratile',(0,3):'pinksakura',(1,0):'lightwood_t',(1,1):'darkwood_t',(1,2):'marble2',
     (1,3):'greytile',(2,0):'marble_t',(2,1):'checker',(2,2):'redcarpet',(2,3):'bluecarpet',(3,0):'beach_t',
     (4,0):'darkwood2',(4,1):'lightwood2',(4,2):'slate',(4,3):'grass',(5,0):'starry_t'}
if __name__=='__main__':
    for (r,c),n in MAP.items(): build(square(r,c),n)
    print(len(MAP),'atlases')
