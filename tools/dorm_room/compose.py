import numpy as np, json, itertools
from PIL import Image
from walls2 import cost, OX, OY, D
STY=['sakura','modern','european','beach','forest','starry']
def all_cands(a,side,L,n=12):
    res=[]
    for d1 in range(24,500-64*L):
        res.append((cost(a,side,d1,L),d1))
    res.sort(); out=[]
    for c,d in res:
        if all(abs(d-o)>16 for _,o in out): out.append((c,d))
        if len(out)>=n: break
    return out
def best_combo(a,side,extra,C):
    opts=[]
    parts={2:[[2],[1,1]],4:[[4],[2,2],[2,1,1],[1,1,1,1]]}[extra]
    for part in parts:
        lists=[C[L] for L in part]
        for combo in itertools.product(*[range(min(6,len(l))) for l in lists]):
            ins=[(lists[k][combo[k]][1],part[k],lists[k][combo[k]][0]) for k in range(len(part))]
            ins.sort()
            ok=all(ins[k+1][0]>=ins[k][0]+64*ins[k][1]+16 for k in range(len(ins)-1))
            if not ok: continue
            mx=max(i[2] for i in ins); sm=sum(i[2] for i in ins)
            opts.append((mx+0.3*sm+4*(len(part)-1),[(d,L) for d,L,_ in ins],mx))
    opts.sort(key=lambda o:o[0])
    return opts[0]
def src_map(ins,total):
    # returns list of (source d, other d or None, weight of other)
    segs=[]; s=0; t=0; seams=[]
    for d1,L in ins:
        end=d1+64*L
        segs.append((t,s,end)); t+=end-s; seams.append((t,64*L)); s=d1
    segs.append((t,s,10**6))
    m=[]
    for dt in range(total):
        for (t0,s0,e0) in reversed(segs):
            if dt>=t0: m.append(s0+dt-t0); break
    return m,seams
def build_side(a,side,ins,N,W=3):
    total=64*N+4+ (4 if True else 0)
    m,seams=src_map(ins,64*N+4)
    H=a.shape[0]
    cols=[]
    for dt in range(64*N+4):
        s=m[dt]
        if s>=516+ (0): s=515
        x=OX-1-s if side==0 else OX+s
        if x<0 or x>=a.shape[1]: cols.append(np.zeros((H,4),np.float32)); continue
        c=a[:,x,:].astype(np.float32)
        c=np.roll(c,(dt-s)//2,axis=0) if (dt-s)%2==0 else np.roll(c,(dt-s)//2,axis=0)
        # crossfade
        for T,shift in seams:
            if T-W<=dt<T+W:
                # other branch: before seam primary continues s; alternative is s-shift (repeat) after; we blend
                alt = s+shift if dt>=T else s-shift
                if 0<=alt<516:
                    xa=OX-1-alt if side==0 else OX+alt
                    ca=np.roll(a[:,xa,:].astype(np.float32),(dt-alt)//2,axis=0)
                    w=(dt-(T-W))/(2*W)  # 0..1
                    wp = w if dt>=T else 1-w   # weight of primary
                    c=c*wp+ca*(1-wp)
        cols.append(c)
    return np.stack(cols,axis=1)  # H x len x 4
def compose(st,N,plan):
    im=Image.open(D+f'wall_{st}.png').convert('RGBA'); a=np.array(im)
    nox=64*N+4; H=OY+64*N+24+ (32*N)
    out=np.zeros((OY+64*N+24,128*N+8,4),np.float32)
    for side in (0,1):
        ins=plan[side] if N>8 else []
        sc=build_side(a,side,ins,N)
        # sc rows are in source frame shifted; output rows same frame (corner at OY)
        h=min(sc.shape[0],out.shape[0]); 
        n=sc.shape[1]
        for dt in range(n):
            x=nox-1-dt if side==0 else nox+dt
            if 0<=x<out.shape[1]:
                # roll shifted down; but rolling wraps; fine since top region transparent
                out[:h,x,:]=sc[:h,dt,:]
    return Image.fromarray(np.clip(out,0,255).astype(np.uint8))
if __name__=='__main__':
    plans={}
    for st in STY:
        a=np.array(Image.open(D+f'wall_{st}.png').convert('RGBA'))
        plans[st]={}
        for N,extra in ((10,2),(12,4)):
            p=[]
            for side in (0,1):
                C={L:all_cands(a,side,L) for L in (1,2,4)}
                b=best_combo(a,side,extra,C); p.append(b[1]); print(st,N,side,b[1],round(b[2],1))
            plans[st][N]=p
    json.dump(plans,open('plans.json','w'))
