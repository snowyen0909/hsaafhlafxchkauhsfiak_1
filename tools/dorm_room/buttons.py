import numpy as np, cv2
from PIL import Image
from scipy import ndimage
src=np.array(Image.open('src/buttons_sheet.png').convert('RGB'))
B={'wall_show':(10,150,592,420,'right'),'wall_hide':(588,150,1168,420,'right'),'expand':(1170,150,1768,420,'right'),
   'tile_mode':(10,465,447,790,'bottom'),'fill_all':(447,460,842,790,'bottom'),'eyedrop':(842,470,1145,790,'bottom'),
   'zoom_reset':(1145,480,1517,790,'bottom'),'lock':(1517,500,1770,790,None)}
OUT='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/ui2/'
for name,(x0,y0,x1,y1,txt) in B.items():
    c=src[y0:y1,x0:x1].copy()
    a=c.astype(int); neutral=(a.max(2)-a.min(2))<14
    bgl=neutral&(a.min(2)>150)
    lab,n=ndimage.label(bgl)
    border=set(np.unique(np.concatenate([lab[0],lab[-1],lab[:,0],lab[:,-1]])))-{0}
    bg=np.isin(lab,list(border))
    alpha=(~bg).astype(np.uint8)*255
    # keep largest component of foreground
    fl,fn=ndimage.label(alpha>0); sizes=ndimage.sum(alpha>0,fl,range(1,fn+1))
    keep=np.isin(fl,[i+1 for i,s in enumerate(sizes) if s>0.02*sizes.max()])
    alpha=np.where(keep,255,0).astype(np.uint8)
    alpha=cv2.GaussianBlur(alpha,(3,3),0)
    if txt:
        h,w=c.shape[:2]
        lum=c.mean(2)
        if txt=='right': box=(int(w*0.44),int(h*0.18),int(w*0.97),int(h*0.80))
        else: box=(int(w*0.08),int(h*0.655),int(w*0.92),int(h*0.93))
        bx0,by0,bx1,by1=box
        tm=np.zeros((h,w),np.uint8)
        cc=c[by0:by1,bx0:bx1].astype(int); sub=(lum[by0:by1,bx0:bx1]<160)&(cc[:,:,0]>=cc[:,:,1])&(cc[:,:,0]-cc[:,:,2]>8)
        tm[by0:by1,bx0:bx1]=sub*255
        tm=cv2.dilate(tm,np.ones((3,3),np.uint8),iterations=3)
        c=cv2.inpaint(c,tm,7,cv2.INPAINT_TELEA)
        print(name,'textbox',box,'size',w,h)
    rgba=np.dstack([c,alpha])
    ys,xs=np.nonzero(alpha>10); rgba=rgba[ys.min():ys.max()+1,xs.min():xs.max()+1]
    Image.fromarray(rgba).save(OUT+f'bt_{name}.png')
    print(name,rgba.shape)
