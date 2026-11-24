import sys, numpy as np, itertools, json
from PIL import Image
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/'
OX,OY=516,185
def bands(im):
    a=np.array(im.convert('RGBA')).astype(np.float32)
    L=[];R=[]
    for j in range(8):
        x0=OX-64*(j+1); x1=OX-64*j
        if j==7: x0=0
        L.append((x0,x1))
        x0=OX+64*j; x1=OX+64*(j+1)
        if j==7: x1=im.width
        R.append((x0,x1))
    return a,L,R
def col(a,x,shift):
    # column x, shifted down by 'shift' px so features align
    c=a[:,x,:]
    return np.roll(c,shift,axis=0)
def seam_cost(a,side,s,e,W=3):
    # run [s..e] inserted after e: seam e|s. natural: e|e+1 and s-1|s
    # compare W columns right of seam: start of s vs start of e+1 ; left of seam: end of e vs end of s-1
    sign=-1 if side==0 else 1
    def start(k): return OX+sign*64*k   # boundary x between band k-1 and k
    cost=0
    for w in range(W):
        # columns inside band (just after boundary going outward)
        xs=start(s)+sign*w + (0 if sign>0 else -1)
        xn=start(e+1)+sign*w + (0 if sign>0 else -1)
        cs=np.roll(a[:,xs,:],32*(e+1-s),axis=0); cn=a[:,xn,:]
        cost+=np.abs(cs-cn).mean()
        xe=start(e+1)-sign*w - (0 if sign>0 else -1) - (sign>0)*1 + (sign<0)*1
        xp=start(s)-sign*w - (0 if sign>0 else -1) - (sign>0)*1 + (sign<0)*1
        ce=a[:,xe,:]; cp=np.roll(a[:,xp,:],32*(e+1-s),axis=0)
        cost+=np.abs(ce-cp).mean()
    return cost/W
def plan(a,side,extra):
    best=[]
    for L in [4,3,2,1]:
        if extra%L: continue
        for s in range(1,8-L):
            e=s+L-1
            if e>6: continue
            c=seam_cost(a,side,s,e)
            best.append((c - 1.5*L,c,s,e,extra//L))
    best.sort()
    return best[:6]
def compose(im,seqL,seqR,N):
    a,L,R=bands(im)
    W=128*N+8; H=OY+64*N+24
    out=Image.new('RGBA',(W,H),(0,0,0,0))
    nox=64*N+4
    for t,s in enumerate(seqL):
        x0,x1=L[s]; band=im.crop((x0,0,x1,im.height))
        tx=nox-64*(t+1) - ((OX-64*(s+1))-x0)
        if t==N-1 and s==7: tx=nox-64*N-4
        out.alpha_composite(band,(tx,32*(t-s)) if 32*(t-s)>=0 else (tx,0)) if 32*(t-s)>=0 else None
    for t,s in enumerate(seqR):
        x0,x1=R[s]; band=im.crop((x0,0,x1,im.height))
        tx=nox+64*t
        out.alpha_composite(band,(tx,32*(t-s)))
    return out
if __name__=='__main__':
    res={}
    for st in ['sakura','modern','european','beach','forest','starry']:
        im=Image.open(D+f'wall_{st}.png').convert('RGBA'); a,_,_=bands(im)
        for side in (0,1):
            for extra in (2,4):
                p=plan(a,side,extra)
                print(st,side,extra,[(round(c,1),s,e,r) for _,c,s,e,r in p[:4]])
