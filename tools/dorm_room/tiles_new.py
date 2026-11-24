import numpy as np
from PIL import Image
from scipy import ndimage
im=np.array(Image.open('src/floors_sheet.png').convert('RGB')).astype(np.float32)
y0,y1=885,985
reg=im[y0:y1]
bg=np.median(np.concatenate([reg[:5].reshape(-1,3),reg[-3:].reshape(-1,3)]),axis=0)
print('bg',bg)
d=np.abs(reg-bg).sum(axis=2)
m=d>40
m=ndimage.binary_closing(m,iterations=2); m=ndimage.binary_fill_holes(m)
lab,n=ndimage.label(m)
objs=ndimage.find_objects(lab)
res=[]
for k,sl in enumerate(objs):
    area=(lab[sl]==k+1).sum()
    if area<3000: continue
    ys,xs=sl
    res.append((xs.start,xs.stop,ys.start+y0,ys.stop+y0,area,k+1))
res.sort()
for r in res: print(r)
np.save('lab.npy',lab)
