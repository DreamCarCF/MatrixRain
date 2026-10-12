# Matrix Rain

绿色单字符代码雨：使用英文字母、数字与符号逐列下落，不使用日文，也不显示整行程序代码。提供 Windows 原生屏保与跨平台离线网页，方便自行发布到 GitHub 并分享。

> 平台边界：Windows 提供原生 `.scr`；Mac 提供浏览器全屏体验，不是原生 `.saver`；手机提供网页/PWA 体验，不能设置成系统屏保或动态锁屏。本项目不自动安装、启用屏保或锁屏。

## 快速开始

| 平台 | 打开方式 | 说明 |
| --- | --- | --- |
| Windows 原生 | 双击 `windows/Start-Matrix.bat` | 以 `/manual` 手动模式运行；不是锁屏 |
| Windows 系统屏保 | 运行 `windows/Install-Screensaver.bat` | 打开系统屏保安装/设置入口，用户自行选择与配置 |
| Windows / Mac / Linux 网页 | 用浏览器打开根目录 `index.html` | 动画单文件离线可用；直接 `file://` 打开不注册 PWA |
| Mac | 运行 `macos/Start-Matrix.command`，或直接打开 `index.html` | 启动脚本打开上一级网页，不安装系统组件 |
| 手机 | 访问发布者的 GitHub Pages 地址 | 可在浏览器中使用；支持时添加到主屏幕 |

Mac 如果提示脚本不能执行，可在终端给 `Start-Matrix.command` 添加执行权限（`chmod +x`），也可以跳过脚本直接打开根目录网页。不要为运行网页而关闭系统安全保护。

## 网页操作

- 面板：暂停/继续、速度滑块、字号滑块、全屏、保持亮屏、返回/退出。
- 桌面快捷键：`F` 切换全屏、空格暂停/继续、上下方向键调速、`Esc` 退出网页全屏。
- 触屏：轻触空白处显示/隐藏底部面板，约 4 秒后自动隐藏；暂停或操作控件时保留面板，滑块操作不作为空白点击处理。
- **保持亮屏**：点击按钮后才请求 Screen Wake Lock；仅在浏览器支持且允许时生效。页面转入后台等情况可能使亮屏锁失效，不保证永久亮屏。
- **返回/退出**：退出网页全屏；主屏幕独立应用模式会提示返回主屏幕。网页不能强制关闭浏览器。
- iOS 的全屏 API 可能不可用，可尝试“添加到主屏幕”，但不等于系统屏保。

## Windows 原生操作

- 手动模式：`Esc` 关闭、空格暂停/继续、上下方向键调速、鼠标单击关闭。
- 系统屏保 `/s` 模式：按键或移动鼠标退出。
- 本机已有桌面快捷键不会随发布包自动传给其他用户。需要热键时，请自行为 `Start-Matrix.bat` 创建快捷方式并在属性中设置，例如 `Ctrl+Alt+Shift+M`（以系统接受的组合为准）。
- 手动运行不锁屏、不提供登录保护。需要系统屏保恢复时的登录保护，请在 Windows 屏保设置中自行勾选“在恢复时显示登录屏幕”。

## 离线与 PWA

`index.html` 内嵌动画所需的 JavaScript/CSS，可直接离线打开。PWA 需要通过 **HTTPS（例如 GitHub Pages）或 localhost** 使用，浏览器支持时注册 `sw.js`；首次成功加载并缓存后，可离线访问缓存的应用。首次访问仍需联网，清除网站数据后需重新缓存。直接以 `file://` 打开时不注册 PWA。

项目不播放后台音频、不请求麦克风或摄像头、不进行分析遥测、不请求通知。保持亮屏只在用户点击且浏览器支持时请求。

## 发布包结构

```text
MatrixRain-Release/
├─ index.html
├─ manifest.webmanifest
├─ sw.js
├─ README.md
├─ readinfo.txt
├─ LICENSE-NOTE.txt
├─ assets/
│  ├─ icon-192.png
│  └─ icon-512.png
├─ windows/
│  ├─ MatrixCode.scr
│  ├─ Start-Matrix.bat
│  ├─ Install-Screensaver.bat
│  ├─ Build-Windows.cmd
│  ├─ readinfo.txt
│  └─ src/MatrixCode.cs
├─ macos/
│  ├─ Start-Matrix.command
│  └─ readinfo.txt
├─ mobile/readinfo.txt
├─ docs/
│  ├─ GITHUB-PUBLISH.md
│  ├─ TESTING.md
│  ├─ preview-desktop.png
│  └─ preview-mobile.png
├─ tests/test_web.py
└─ requirements-dev.txt
```

以上文件均已生成。另含 `tests/test_windows.py` 和 `docs/preview-windows.png`。Windows 源码与基于 .NET Framework `csc` 的构建脚本位于 `windows/`。测试依赖仅用于开发验证，终端用户无需安装。

## GitHub 发布与验证

- 发布方法：[docs/GITHUB-PUBLISH.md](docs/GITHUB-PUBLISH.md)。上传此文件夹的**内容**到仓库根目录，再配置 GitHub Pages；不要求使用 Actions 工作流。
- 验收与测试范围：[docs/TESTING.md](docs/TESTING.md)。最终包已通过 Windows 原生自测，以及 Chromium、WebKit 的桌面/移动视口测试；模拟不代表 Mac/iPhone 真机兼容性。
- 预览图：`docs/preview-desktop.png` 与 `docs/preview-mobile.png` 已由实际测试流程生成。

## 许可证

许可证暂未决定，请查看 `LICENSE-NOTE.txt`。发布者应在正式分发前选择合适的许可证；本说明不暗示项目采用 MIT 或其他开源许可证。公开仓库不等于自动授予开源使用许可。
