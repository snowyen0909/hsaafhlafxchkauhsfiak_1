import numpy as np, json
from PIL import Image, ImageDraw
from scipy import ndimage
src=Image.open('src/floors_sheet.png').convert('RGB')
im=np.array(src).astype(np.float32)
y0=885
lab=np.load('lab.npy')
names=['white','checker','marble2','darkwood2','lightwood2','slate','redcarpet','bluecarpet','flowertile','grass']
labs=[None,9,10,2,3,4,5,6,7,8]
info={}
def corners(mask,ox,oy):
    ys,xs=np.nonzero(mask)
    xl=xs.min(); yl=ys[xs<=xl+1].min()
    xr=xs.max(); yr=ys[xs>=xr-1].min()
    yt=ys.min(); xt=int(np.median(xs[ys<=yt+1]))
    return (xl+ox,yl+oy),(xr+ox,yr+oy),(xt+ox,yt+oy),(xs.min()+ox,ys.max()+oy)
for k,(nm,l) in enumerate(zip(names,labs)):
    if l is None: continue
    mask=(lab==l)
    L,R,T,B=corners(mask,0,y0)
    info[nm]=dict(L=L,R=R,T=T,bottom=int(B[1]))
# white tile: copy geometry of checker shifted
c=info['checker']; dx=-148.5
info['white']={kk:((v[0]+dx,v[1]) if isinstance(v,tuple) else v) for kk,v in c.items()}
for k,v in info.items(): print(k,v)
json.dump({k:{kk:(list(map(float,vv)) if isinstance(vv,tuple) else vv) for kk,vv in v.items()} for k,v in info.items()},open('tile_geo.json','w'))
dbg=src.copy(); d=ImageDraw.Draw(dbg)
for k,v in info.items():
    L,R,T=v['L'],v['R'],v['T']; Bt=(L[0]+R[0]-T[0],L[1]+R[1]-T[1])
    d.polygon([T,R,Bt,L],outline=(255,0,0))
dbg.crop((0,880,1536,990)).save('tiles_dbg.png')
