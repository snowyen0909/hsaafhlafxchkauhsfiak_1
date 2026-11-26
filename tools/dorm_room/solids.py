# Solid-colour floors: 2x2 tile-layout atlases (260x220), identical seamless cells, darker front edges.
import numpy as np, sys
from PIL import Image
sys.path.insert(0,'/home/claude/tiles2')
from cut_floors import DM, LM, RM, masked, CW, CH, EW, EH
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/'
COLORS={'cream':(244,232,212),'white':(248,247,243),'grey':(204,204,208),'dark':(100,102,112),'wood':(214,172,124),
        'pink':(246,208,216),'blue':(192,222,242),'mint':(198,232,208),'yellow':(250,234,182)}
rng=np.random.default_rng(7)
def flat(w,h,col,k=1.0,noise=1.5):
    a=np.ones((h,w,3))*np.array(col)*k + rng.normal(0,noise,(h,w,1))
    return Image.fromarray(np.clip(a,0,255).astype(np.uint8)).convert('RGBA')
for name,col in COLORS.items():
    at=Image.new('RGBA',(2*CW,2*CH+2*EH),(0,0,0,0))
    top=masked(flat(CW,CH,col),DM)
    le=flat(EW,EH,col,0.78,1.0); re=flat(EW,EH,col,0.64,1.0)
    for p in (le,re):   # darker rim along the bottom of the thickness
        a=np.array(p).astype(float); 
        for y in range(EH): a[y,:,:3]*=1.0
        p.paste(Image.fromarray(a.astype(np.uint8)))
    le=masked(le,LM); re=masked(re,RM)
    for q in range(2):
        for p in range(2): at.alpha_composite(top,(q*CW,p*CH))
        at.alpha_composite(le,(q*EW,2*CH)); at.alpha_composite(re,(q*EW,2*CH+EH))
    at.save(D+f'tiles/solid_{name}.png',optimize=True)
sys.path.insert(0,'/home/claude/room2')
from preview2 import render
for name in COLORS:
    im=render(2,lambda i,j:f'solid_{name}'); arr=np.array(im); bg=(arr[:,:,0]==70)&(arr[:,:,1]==70)&(arr[:,:,2]==70); arr[bg,3]=0
    im=Image.fromarray(arr); im=im.crop(im.getbbox()); im=im.resize((260,int(im.height*260/im.width)),Image.LANCZOS)
    c=Image.new('RGBA',(260,132),(0,0,0,0)); c.alpha_composite(im,(0,(132-im.height)//2)); c.save(D+f'floor_solid_{name}_0.png')
print('ok')
