from PIL import Image
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/'
CW,CH,EW,EH=130,66,64,44
cache={}
NN=[8]
def atlas(m):
    if m not in cache: cache[m]=Image.open(D+f'tiles/{m}.png').convert('RGBA')
    return cache[m]
MULTI={'tatami','lightwood','darkwood','marble','beach','starry'}
def srcidx(t,N):
    if N<=8 or t<4: return t%8
    E=N-8
    if t<4+E: return 2+((t-4)%2)
    return t-E
def cell_src(m,i,j,N=None):
    base=m.split(':')[0]
    if base in MULTI:
        si,sj=srcidx(i,NN[0]),srcidx(j,NN[0])
        if ':' in m:  # beach:blue / beach:white parity
            p=int(m.split(':')[1]); 
            if (si+sj)%2!=p: sj=(sj+1)%8
        return base,(si*CW,sj*CH,CW,CH)
    return base,(0,0,CW,CH)
def ledge(m,i,N):
    base=m.split(':')[0]
    if base in MULTI:
        si=srcidx(i,NN[0])
        if ':' in m:
            p=int(m.split(':')[1])
            if (si+7)%2!=p: si=(si+1)%8
        return base,(si*EW,8*CH,EW,EH)
    return base,(0,CH,EW,EH)
def redge(m,j,N):
    base=m.split(':')[0]
    if base in MULTI:
        sj=srcidx(j,NN[0])
        if ':' in m:
            p=int(m.split(':')[1])
            if (7+sj)%2!=p: sj=(sj+1)%8
        return base,(sj*EW,8*CH+EH,EW,EH)
    return base,(0,CH+EH,EW,EH)
def render(N,tiles,wall=None):
    NN[0]=N
    OX=64*N+4; OY=185
    W=128*N+8; H=OY+64*N+24
    out=Image.new('RGBA',(W,H),(70,70,70,255))
    if wall: out.alpha_composite(Image.open(wall).convert('RGBA'),(0,0))
    SX=lambda i,j: OX+(i-j)*64; SY=lambda i,j: OY+(i+j)*32
    for s in range(2*N-1):
        for i in range(N):
            j=s-i
            if not 0<=j<N: continue
            m=tiles(i,j); b,r=cell_src(m,i,j); a=atlas(b)
            out.alpha_composite(a.crop((r[0],r[1],r[0]+r[2],r[1]+r[3])),(SX(i,j)-65,SY(i,j)-1))
    for i in range(N):
        m=tiles(i,N-1); b,r=ledge(m,i,N)
        out.alpha_composite(atlas(b).crop((r[0],r[1],r[0]+r[2],r[1]+r[3])),(SX(i,N),SY(i,N)))
    for j in range(N):
        m=tiles(N-1,j); b,r=redge(m,j,N)
        out.alpha_composite(atlas(b).crop((r[0],r[1],r[0]+r[2],r[1]+r[3])),(SX(N,j+1),SY(N,j)))
    return out
if __name__=='__main__':
    import numpy as np
    # 8x8 beach vs original
    a=render(8,lambda i,j:'beach')
    o=Image.new('RGBA',a.size,(70,70,70,255)); o.alpha_composite(Image.open(D+'floor_beach.png').convert('RGBA'))
    diff=np.abs(np.array(a).astype(int)-np.array(o).astype(int))[:,:,:3]
    print('beach diff mean',diff.mean(),'max',diff.max(), (diff.sum(2)>60).sum())
    Image.fromarray(np.concatenate([np.array(a),np.array(o)],1)).convert('RGB').resize((1040,358)).save('cmp8.jpg')
    render(12,lambda i,j:'tatami','w_sakura_12.png').convert('RGB').save('p12_tatami.jpg',quality=88)
    pat=lambda i,j: ('redcarpet' if 3<=i<=6 and 3<=j<=6 else ('flowertile' if (i,j) in [(2,2),(7,7),(2,7),(7,2)] else ('checker' if (i+j)%5==0 else 'marble2'))) if i<10 and j<10 else 'grass'
    render(10,pat,'w_european_10.png').convert('RGB').save('p10_mix.jpg',quality=88)
    render(10,lambda i,j: 'beach:0' if (i//2+j//2)%2 else 'beach:1','w_beach_10.png').convert('RGB').save('p10_beach.jpg',quality=88)
