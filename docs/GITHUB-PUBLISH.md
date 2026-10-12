# Matrix Rain — GitHub 发布指南

本指南供发布者自行操作，不会替你创建仓库、上传文件、启用屏保或改变系统锁屏设置。

## 1. 上传前检查

- 确认发布包中的 `index.html`、`manifest.webmanifest`、`sw.js`、两个图标及各平台启动文件存在；Windows 原生文件与源码、文档保持一致。
- 完成 [TESTING.md](TESTING.md) 的验收；将尚未测试的平台标注清楚，不能将 Chromium 移动模拟写成 iPhone 真机通过。
- 检查 `LICENSE-NOTE.txt`，正式发布前自行选择许可证。不要默认选择 MIT，也不要把“仓库公开”当作已经获得开源许可。
- 不上传本机 C 盘绝对路径配置、桌面 `.lnk`、私钥、访问令牌、`.env`、私人文件、测试日志、浏览器个人数据或测试截图中的隐私。
- 可保留 `tests/test_web.py` 和 `requirements-dev.txt` 供开发者复现；不要上传 `.venv/`、已安装的测试依赖、浏览器测试运行时、缓存等。预览图只保留确认无隐私的 `docs/preview-desktop.png` 和 `docs/preview-mobile.png`。

## 2. 创建仓库并上传

1. 在 GitHub 创建自己的仓库；希望粉丝直接访问和下载时，通常选择 Public。仓库名由你决定。
2. 使用网页“Add file → Upload files”、GitHub Desktop 或 Git 上传。
3. 上传 **MatrixRain-Release 文件夹里面的内容** 到仓库根目录，不要再套一层 `MatrixRain-Release/`。
4. 保留 `assets/`、`windows/`、`macos/`、`mobile/`、`docs/` 等目录结构。确认仓库根目录能直接看到 `index.html`。
5. 提交到 `main` 分支；如果采用其他分支，后续 Pages 选择实际分支。

GitHub 网页上传有大小限制；若二进制文件上传失败，可改用 Git 或附到 Release，不要用失效的本机路径代替文件。

## 3. 启用 GitHub Pages

1. 打开仓库 **Settings → Pages**。
2. 在 Build and deployment 中将 Source 选择 **Deploy from a branch**。
3. Branch 选择 **main**，目录选择 **/(root)**，保存。
4. 等待部署完成，以 Pages 页面显示的真实网址为准。

不用自行编写 GitHub Actions 工作流，也不用选择 Actions 作为部署来源。GitHub 可能仍显示内部部署任务记录，这不代表必须维护工作流。

普通项目仓库的地址模板：

```text
https://<username>.github.io/<repo>/
```

这是占位模板，不是真实网址；请替换为自己的账号和仓库名。自定义域名或账号主页仓库的地址可能不同，应以 Settings → Pages 中的网址为准。

## 4. 验证分享地址

- 用新的浏览器窗口访问实际 HTTPS 地址，确认动画、图标、面板与资源无 404。
- 项目站点带有 `/<repo>/` 前缀，检查 manifest、service worker 和资源能在此前缀下加载，而非错误地请求域名根目录。
- 查看浏览器控制台是否有 JavaScript 或 service worker 错误。
- 等首次加载与缓存完成后，在相同浏览器中离线刷新验证。不要只断网后看仍在运行的旧页面就判定离线缓存通过。
- 真机测试 Android 与 iPhone；iOS 全屏 API 可能不可用，需检查添加到主屏幕的实际效果。
- 更新版本后确认旧缓存能正确更新；遇到旧版本，先关闭旧页面重开，必要时清除该站点数据并重新缓存。

## 5. 给粉丝的两种入口

- **手机与网页**：分享上一步确认的 Pages HTTPS 地址。
- **Windows 原生与离线包**：仓库 “Code → Download ZIP”，或在 GitHub Releases 附上完整发布包 ZIP，并写清版本与使用说明。

压缩包应保留相对目录结构。不要分发发布者自己的桌面快捷方式；粉丝需要自行创建热键，例如 Ctrl+Alt+Shift+M。Mac 启动脚本可能需要 `chmod +x`，也可直接打开 `index.html`。

## 发布描述建议

> Matrix Rain：绿色英文字母、数字与符号单字符代码雨。Windows 提供原生屏保；Mac 与手机提供浏览器/PWA 体验。手机不能设置为系统屏保或动态锁屏。PWA 首次联网缓存后可离线使用，保持亮屏需浏览器支持并由用户点击请求。兼容性请参考测试记录，许可证请参考仓库许可证说明。

请按真实验收结果补充测试记录，不宣称未测试的 Mac、iPhone 或 WebKit 已通过。
