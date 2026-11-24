import json,sys
from PIL import Image, ImageDraw
from compose import compose, STY
plans=json.load(open('plans.json'))
N=int(sys.argv[1])
ims=[]
for st in STY:
    p={int(k):v for k,v in plans[st].items()}
    im=compose(st,N,p[N] if N>8 else [[],[]])
    im.save(f'w_{st}_{N}.png')
    ims.append(im)
W=ims[0].width//2; H=ims[0].height//2
sheet=Image.new('RGBA',(W*2,H*3),(60,60,60,255))
for k,im in enumerate(ims):
    sheet.alpha_composite(im.resize((W,H)),((k%2)*W,(k//2)*H))
sheet.convert('RGB').save(f'review_{N}.jpg',quality=85)
print(sheet.size)
