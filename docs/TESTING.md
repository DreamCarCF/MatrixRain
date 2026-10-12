# 测试结果与发布前验收

## 已执行并通过

测试环境是 Windows，不是 macOS 或手机真机。

| 项目 | 实际验证范围 | 结果 |
| --- | --- | --- |
| Windows 源码构建 | 使用发布包的 Build-Windows.cmd 和系统 .NET Framework 编译器生成 MatrixCode.scr | 通过 |
| Windows 原生自测 | 临时目录运行 /test；单字符 ASCII、动画推进、暂停、调速、绘制、Esc 关闭窗体 | 通过 |
| Chromium / Edge 桌面视口 | 1920×1080，动画、暂停、调速、字号、调整窗口 | 通过 |
| Chromium / Edge 移动视口 | 390×844，触屏面板显隐、横竖屏切换、控件布局 | 通过 |
| Playwright WebKit 桌面及移动视口 | 同上；使用 Windows 上的 WebKit 测试运行时，不是 Safari 真机 | 通过 |
| 两种网页引擎离线缓存 | localhost 首次缓存，停止实际 HTTP 服务后刷新，代码雨继续运行 | 通过 |
| 本地项目子路径 | 在 /matrix-rain/ 前缀下验证资源路径、service worker 作用域与离线缓存 | 两种网页引擎通过 |
| 本地单文件离线版 | file:// 打开 index.html，动画正常；不注册 PWA | 通过 |
| 辅助功能与小屏 | 减少动态效果设置下默认暂停；320×568 布局；继续及退出按钮 | 通过 |
| Mac 启动脚本 | bash -n 语法检查 | 通过；未在 Mac 执行 |

测试依赖为 Playwright 1.63.0。页面测试没有发现未处理 JavaScript 异常。实际预览图位于本目录：preview-desktop.png、preview-mobile.png、preview-windows.png。

WebKit 的离线模拟开关曾出现测试引擎内部错误，因此最终测试采用“关闭真实 HTTP 服务，再刷新网页”的方式验证缓存回退，两种引擎均通过。此结论不等于手机主屏幕应用已经实机验收。

## 复现命令

以下仅供开发者运行，粉丝正常使用无需安装 Python 或测试浏览器。

```sh
python -m pip install -r requirements-dev.txt
python -m playwright install chromium webkit
python tests/test_web.py
```

默认用 Chromium。测试 WebKit：

```sh
# macOS / Linux / Git Bash
MATRIX_BROWSER_ENGINE=webkit python tests/test_web.py
```

Windows PowerShell：

```powershell
$env:MATRIX_BROWSER_ENGINE='webkit'
python tests/test_web.py
Remove-Item Env:MATRIX_BROWSER_ENGINE
```

如使用本机已安装的 Edge，设置 MATRIX_BROWSER_CHANNEL=msedge，并保持默认 chromium 引擎；不能把 msedge channel 传给 WebKit。

Windows 原生自测：

```sh
python tests/test_windows.py
```

该脚本在临时目录运行程序，不会更改系统屏保设置或打开全屏窗口；它会更新 docs/preview-windows.png。

## 尚未覆盖：发布前建议补测

- macOS 真机 Safari/Chrome：双击 .command、打开 index.html、全屏、快捷键、显示缩放与持续运行。
- iPhone/iPad Safari：普通网页和添加到主屏幕、安全区、横竖屏、离线重开。
- Android 真机：触屏、全屏、添加到主屏幕、离线重开、性能与发热。
- Screen Wake Lock 真机有效性、系统省电模式及后台回来后的行为；按钮变色不能单独证明系统真的保持亮屏。
- Windows /s 系统屏保的空闲启动、鼠标唤醒、多显示器，以及 /p 设置窗口预览；自测不等于这些系统集成行为已全部覆盖。
- 安装脚本与“恢复时显示登录屏幕”：主动执行会改变本机屏保配置，请先记录原设置并按需恢复。没有为验证而自动安装。
- 最终 GitHub Pages 的 /<repo>/ 路径、HTTPS、资源请求和更新缓存；本地测试不能证明尚未部署的真实站点成功。
- 各设备较长时间运行的流畅度、耗电与发热。

手机网页不是系统屏保或动态锁屏；Mac 网页版不是原生 .saver 插件。未经上述真机验收，不应宣传为“所有设备测试通过”。
