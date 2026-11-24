import numpy as np, json
from PIL import Image, ImageDraw
from scipy import ndimage
src=Image.open('src/floors_sheet.png').convert('RGB'); a=np.array(src).astype(int)
names=['tatami','lightwood','marble','beach','darkwood','starry']
cols=[(30,242),(545,757),(1055,1268)]
rows=[(318,425),(748,855)]
geo={}
for k,nm in enumerate(names):
    x0,x1=cols[k%3]; y0,y1=rows[k//3]
    reg=a[y0:y1,x0:x1]
    m=(np.abs(reg-255).sum(2)>45)
    m=ndimage.binary_closing(m,iterations=2); m=ndimage.binary_fill_holes(m)
    lab,n=ndimage.label(m); sizes=ndimage.sum(m,lab,range(1,n+1)); big=np.argmax(sizes)+1; m=lab==big
    ys,xs=np.nonzero(m)
    xl=xs.min(); yl=ys[xs<=xl+1].min()
    xr=xs.max(); yr=ys[xs>=xr-1].min()
    yb=ys.max(); xb=int(np.median(xs[ys>=yb-1]))
    col=np.nonzero(m[:,xl+2])[0]; thick=col.max()-col.min()
    L=np.array([xl+x0,yl+y0],float); R=np.array([xr+x0,yr+y0],float); B=np.array([xb+x0,yb+y0-thick],float)
    T=L+R-B
    geo[nm]=dict(L=L.tolist(),R=R.tolist(),T=T.tolist(),B=B.tolist(),h=float(thick))
    print(nm,geo[nm])
json.dump(geo,open('sample_geo.json','w'))
d=ImageDraw.Draw(src)
for g in geo.values():
    d.polygon([tuple(g['T']),tuple(g['R']),tuple(g['B']),tuple(g['L'])],outline=(255,0,0))
s=Image.new('RGB',(1536,280),'white'); s.paste(src.crop((0,300,1536,430)),(0,0)); s.paste(src.crop((0,730,1536,860)),(0,140)); s.save('samples_dbg.png')
