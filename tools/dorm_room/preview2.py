from PIL import Image
D='/home/claude/hsaafhlafxchkauhsfiak_1/ContentMod/dorm/'
CW,CH,EW,EH=130,66,64,44
C={}
def A(n):
    if n not in C: C[n]=Image.open(D+'tiles/'+n+'.png').convert('RGBA')
    return C[n]
def render(N,tile,wall=None):
    OX=64*N+4; OY=185
    out=Image.new('RGBA',(128*N+8,OY+64*N+24),(70,70,70,255))
    if wall: out.alpha_composite(Image.open(wall).convert('RGBA'))
    SX=lambda i,j: OX+(i-j)*64; SY=lambda i,j: OY+(i+j)*32
    for i in range(N):
        for j in range(N):
            a=A(tile(i,j)); r=((i%2)*CW,(j%2)*CH)
            out.alpha_composite(a.crop((r[0],r[1],r[0]+CW,r[1]+CH)),(SX(i,j)-65,SY(i,j)-1))
    for k in range(N):
        a=A(tile(k,N-1)); out.alpha_composite(a.crop(((k%2)*EW,2*CH,(k%2)*EW+EW,2*CH+EH)),(SX(k,N),SY(k,N)))
        a=A(tile(N-1,k)); out.alpha_composite(a.crop(((k%2)*EW,2*CH+EH,(k%2)*EW+EW,2*CH+2*EH)),(SX(N,k+1),SY(N,k)))
    return out
if __name__=='__main__':
    ims=[]
    for m in ['tatami_t','lightwood_t','marble_t','beach_t','darkwood_t','starry_t','white','checker','marble2','darkwood2','lightwood2','slate','redcarpet','bluecarpet','flowertile','grass']:
        ims.append((m,render(4,lambda i,j:m)))
    W=ims[0][1].width; H=ims[0][1].height
    s=Image.new('RGB',(W*4,H*4),(70,70,70))
    for k,(m,im) in enumerate(ims): s.paste(im.convert('RGB'),((k%4)*W,(k//4)*H))
    s.save('quads.jpg',quality=88)
    pat=lambda i,j: 'redcarpet' if 2<=i<=5 and 2<=j<=5 else ('marble_t' if (i+j)%2 else 'white')
    render(8,pat,D+'wall_european.png').convert('RGB').save('quadmix.jpg',quality=88)
