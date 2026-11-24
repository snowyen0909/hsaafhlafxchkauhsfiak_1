import numpy as np, json, sys
from PIL import Image
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/'
OX,OY=516,185
def colat(a,side,d):
    x=OX-1-d if side==0 else OX+d
    return a[:,x,:]
def cost(a,side,d1,L,K=7):
    sh=32*L; c=0
    for k in range(-K,K+1):
        p=colat(a,side,d1+k).astype(np.float32); q=colat(a,side,d1+64*L+k).astype(np.float32)
        p=np.roll(p,sh,axis=0)
        al=np.minimum(p[:,3],q[:,3])/255.
        m=al>0.5
        if m.sum()<100: return 1e9
        c+=np.abs(p[m,:3]-q[m,:3]).mean()
    # prefer plain areas: horizontal gradient energy at the seam column
    g=0
    for k in range(-3,4):
        u=colat(a,side,d1+64*L+k).astype(np.float32); v=colat(a,side,d1+64*L+k+1).astype(np.float32)
        al=np.minimum(u[:,3],v[:,3])>128
        g+=np.abs(u[al,:3]-v[al,:3]).mean() if al.sum()>50 else 0
    return c/(2*K+1)+0.6*g/7
def candidates(a,side,L):
    res=[]
    for d1 in range(24,500-64*L):
        res.append((cost(a,side,d1,L),d1))
    res.sort()
    # non-max suppress
    out=[]
    for c,d in res:
        if all(abs(d-o)>20 for _,o in out): out.append((c,d))
        if len(out)>=5: break
    return out
if __name__=='__main__':
    R={}
    for st in ['sakura','modern','european','beach','forest','starry']:
        a=np.array(Image.open(D+f'wall_{st}.png').convert('RGBA'))
        for side in (0,1):
            for L in (1,2,4):
                c=candidates(a,side,L)
                print(st,side,L,[(round(x,1),d) for x,d in c])
                R[f'{st}_{side}_{L}']=[(float(x),int(d)) for x,d in c]
    json.dump(R,open('cands.json','w'))
