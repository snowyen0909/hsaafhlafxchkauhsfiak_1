# Builds tavel_super_gatling.zip (the sample plant mod) from 塔维尔's PC mod folder.
# usage: python3 -I make_tavel_pack.py <PGvZ_PC_塔维尔的植物娘_...> <out.zip>
import json, os, sys, zipfile

src, out = sys.argv[1], sys.argv[2]
payload = os.path.join(src, 'Payload')
files = {
    'cards/super_gatling.png': os.path.join(payload, 'mods', 'nineplants', 'assets', 'super_gatling', 'card.png'),
    'spine/super_gatlingpea.skel': os.path.join(payload, 'spanim', 'super_gatlingpea.skel'),
    'spine/super_gatlingpea.atlas': os.path.join(payload, 'spanim', 'super_gatlingpea.atlas'),
    'spine/super_gatlingpea.png': os.path.join(payload, 'spanim', 'super_gatlingpea.png'),
}
manifest = {
    'id': 'tavel_super_gatling', 'name': '超级机枪射手', 'version': '1.0', 'author': '塔维尔',
    'description': '塔维尔的植物娘：超级机枪射手。更大的枪，更密集的火力。',
    'plants': [{
        'id': 'super_gatling', 'name': '超级机枪射手',
        'description': '连续发射豌豆的超级机枪射手。\n伤害：机枪射手的 1.5 倍\n攻击间隔：0.9 秒',
        'base': 'Gatlingpea', 'cost': 400, 'recharge': 7.5, 'health': 300, 'fireInterval': 0.9, 'damage': 1.5,
        'card': 'cards/super_gatling.png', 'spine': 'spine/super_gatlingpea', 'spineScale': 14.3,
        'scale': 1.375, 'offset': [-28, -83], 'animations': {'idle': 'stand', 'attack': 'attack'},
    }],
}
note = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'tavel_install_note.txt')
with zipfile.ZipFile(out, 'w', zipfile.ZIP_DEFLATED) as z:
    z.writestr('tavel_super_gatling/mod.json', json.dumps(manifest, ensure_ascii=False, indent=2))
    for rel, path in files.items():
        z.write(path, 'tavel_super_gatling/' + rel)
    if os.path.exists(note):
        z.write(note, 'tavel_super_gatling/安装说明.txt')
