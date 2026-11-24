import numpy as np, json, os
from PIL import Image, ImageDraw, ImageFilter
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/'
OUT=D+'tiles/'; os.makedirs(OUT,exist_ok=True)
CW,CH=130,66   # cell slot (128x64 diamond + 1px bleed)
EW,EH=64,44    # edge piece
def diamond_mask():
    m=Image.new('L',(CW*4,CH*4),0); d=ImageDraw.Draw(m)
    d.polygon([(65*4,-2),(130*4+2,33*4),(65*4,66*4+2),(-2,33*4)],fill=255)
    return m.resize((CW,CH),Image.LANCZOS)
def edge_mask(right):
    m=Image.new('L',(EW*4,EH*4),0); d=ImageDraw.Draw(m)
    if not right: pts=[(0,-4),(64*4,32*4-4),(64*4,EH*4),(0,11*4+4)]
    else: pts=[(0,32*4-4),(64*4,-4),(64*4,11*4+4),(0,EH*4)]
    d.polygon(pts,fill=255); return m.resize((EW,EH),Image.LANCZOS)
DM=diamond_mask(); LM=edge_mask(False); RM=edge_mask(True)
def masked(img,mask):
    img=img.convert('RGBA'); a=np.array(img); a[:,:,3]=(a[:,:,3].astype(np.float32)*np.array(mask)/255).astype(np.uint8); return Image.fromarray(a)
def SX(i,j): return 516+(i-j)*64
def SY(i,j): return 185+(i+j)*32
def cut_old(name):
    im=Image.open(D+f'floor_{name}.png').convert('RGBA')
    at=Image.new('RGBA',(8*CW,8*CH+2*EH),(0,0,0,0))
    for i in range(8):
        for j in range(8):
            x,y=SX(i,j)-65,SY(i,j)-1
            at.alpha_composite(masked(im.crop((x,y,x+CW,y+CH)),DM),(i*CW,j*CH))
    for i in range(8):   # left edges under cells (i,7)
        x,y=SX(i,8),SY(i,8)
        at.alpha_composite(masked(im.crop((x,y,x+EW,y+EH)),LM),(i*EW,8*CH))
    for j in range(8):   # right edges under cells (7,j)
        x,y=SX(8,j+1),SY(8,j)
        at.alpha_composite(masked(im.crop((x,y,x+EW,y+EH)),RM),(j*EW,8*CH+EH))
    at.save(OUT+f'{name}.png',optimize=True)
def affine(dst,src):
    # find coeffs mapping dst->src
    A=[];b=[]
    for (x,y),(u,v) in zip(dst,src):
        A.append([x,y,1,0,0,0]); b.append(u); A.append([0,0,0,x,y,1]); b.append(v)
    return np.linalg.lstsq(np.array(A,float),np.array(b,float),rcond=None)[0]
def cut_new(name,g,sheet,SC=4):
    L,R,T=np.array(g['L']),np.array(g['R']),np.array(g['T']); B=L+R-T
    h=float(g['bottom'])-B[1]-1.5   # side thickness in source
    at=Image.new('RGBA',(CW,CH+2*EH),(0,0,0,0))
    big=sheet.resize((sheet.width*SC,sheet.height*SC),Image.BICUBIC) if False else sheet
    # top face: dst diamond (65,1)T (129,33)R (1,33)L inside the 130x66 slot
    c=affine([(65,1),(129,33),(1,33)],[T,R,L])
    top=sheet.transform((CW,CH),Image.AFFINE,tuple(c),resample=Image.BICUBIC)
    at.alpha_composite(masked(top,DM),(0,0))
    c=affine([(0,0),(64,32),(0,11)],[L,B,L+[0,h]])
    le=sheet.transform((EW,EH),Image.AFFINE,tuple(c),resample=Image.BICUBIC)
    at.alpha_composite(masked(le,LM),(0,CH))
    c=affine([(0,32),(64,0),(0,43)],[B,R,B+[0,h]])
    re=sheet.transform((EW,EH),Image.AFFINE,tuple(c),resample=Image.BICUBIC)
    at.alpha_composite(masked(re,RM),(0,CH+EH))
    at.save(OUT+f'{name}.png',optimize=True)
if __name__=='__main__':
    for n in ['tatami','lightwood','darkwood','marble','beach','starry']: cut_old(n)
    geo=json.load(open('tile_geo.json')); sheet=Image.open('src/floors_sheet.png').convert('RGBA')
    for n,g in geo.items(): cut_new(n,g,sheet)
    print(sorted(os.listdir(OUT)))
