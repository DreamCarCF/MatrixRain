"""Run: python tests/test_web.py (requires Playwright and a Chromium browser)."""
import functools
import http.server
import json
import os
from pathlib import Path
import threading
from playwright.sync_api import sync_playwright
ROOT = Path(__file__).resolve().parents[1]
class QuietHandler(http.server.SimpleHTTPRequestHandler):
    def log_message(self, *args): pass
    def translate_path(self, path):
        # Serve the same folder beneath a project-site prefix, like GitHub Pages.
        if path.startswith('/matrix-rain/'):
            path = path[len('/matrix-rain'):]
        return super().translate_path(path)
server = http.server.ThreadingHTTPServer(('127.0.0.1', 0), functools.partial(QuietHandler, directory=str(ROOT)))
threading.Thread(target=server.serve_forever, daemon=True).start()
url = f'http://127.0.0.1:{server.server_port}/matrix-rain/'
try:
    with sync_playwright() as p:
        engine = getattr(p, os.environ.get('MATRIX_BROWSER_ENGINE', 'chromium'))
        browser = engine.launch(channel=os.environ.get('MATRIX_BROWSER_CHANNEL') or None, headless=True)
        for name, options in [('desktop', {'viewport': {'width': 1920, 'height': 1080}}), ('mobile', {'viewport': {'width': 390, 'height': 844}, 'is_mobile': True, 'has_touch': True, 'device_scale_factor': 3})]:
            context = browser.new_context(**options)
            page = context.new_page()
            errors = []
            page.on('pageerror', lambda e: errors.append(str(e)))
            response = page.goto(url)
            assert response.status == 200
            page.wait_for_function('window.matrixState && matrixState().frames > 10')
            state = page.evaluate('matrixState()')
            assert state['asciiOnly'] and state['singleCharacters']
            assert state['streams'] > 15
            assert page.evaluate('document.documentElement.scrollWidth <= innerWidth')
            assert page.locator('#panel').bounding_box()['width'] <= options['viewport']['width']
            page.locator('#pause').click()
            before = page.evaluate('matrixState().frames')
            page.wait_for_timeout(150)
            assert page.evaluate('matrixState().frames') == before
            page.locator('#speed').evaluate("el=>{el.value='1.8';el.dispatchEvent(new Event('input',{bubbles:true}));}")
            assert page.evaluate('matrixState().speed') == 1.8
            page.locator('#size').evaluate("el=>{el.value='24';el.dispatchEvent(new Event('input',{bubbles:true}));}")
            assert page.evaluate('matrixState().size') == 24
            page.locator('#pause').click()
            page.wait_for_function(f'matrixState().frames > {before}')
            page.set_viewport_size({'width': 844, 'height': 390})
            page.wait_for_function('matrixState().width === 844 && matrixState().height === 390')
            assert page.evaluate('document.documentElement.scrollWidth <= innerWidth')
            page.set_viewport_size(options['viewport'])
            page.wait_for_function(f'matrixState().width === {options["viewport"]["width"]}')
            if name == 'mobile':
                page.locator('#hide').tap()
                assert page.evaluate("document.body.classList.contains('quiet')")
                page.touchscreen.tap(100, 100)
                assert not page.evaluate("document.body.classList.contains('quiet')")
            (ROOT/'docs').mkdir(exist_ok=True)
            page.screenshot(path=str(ROOT/'docs'/f'preview-{name}.png'))
            page.evaluate("navigator.serviceWorker.ready.then(()=>{window.swReady=true})")
            page.wait_for_function('window.swReady === true')
            page.wait_for_function('navigator.serviceWorker.controller !== null')
            assert page.evaluate("navigator.serviceWorker.controller.scriptURL.endsWith('/sw.js')")
            page.wait_for_function("caches.keys().then(keys=>keys.length>0)")
            # Stop the actual local origin to verify service-worker fallback, rather
            # than relying on engine-specific offline emulation.
            address = server.server_address
            server.shutdown()
            server.server_close()
            page.reload()
            page.wait_for_function('matrixState().frames > 5')
            server = http.server.ThreadingHTTPServer(address, functools.partial(QuietHandler, directory=str(ROOT)))
            threading.Thread(target=server.serve_forever, daemon=True).start()
            assert not errors, errors
            print(json.dumps({'view':name,'result':'PASS','checks':['ASCII single-character animation','pause','speed','size','responsive layout','rotate','PWA cached offline reload'],'state':state}))
            context.close()
        # Local-file distribution needs no web server or external assets.
        page = browser.new_page()
        page.goto((ROOT/'index.html').as_uri())
        page.wait_for_function('matrixState().frames > 5')
        print('PASS: offline local HTML')
        context = browser.new_context(reduced_motion='reduce', viewport={'width': 320, 'height': 568})
        page = context.new_page()
        page.goto(url)
        page.wait_for_function('window.matrixState')
        assert page.evaluate('matrixState().paused')
        assert page.locator('#panel').bounding_box()['width'] <= 320
        page.locator('#pause').click()
        page.wait_for_function('matrixState().frames > 3')
        page.locator('#exit').click()
        assert page.evaluate('matrixState().paused')
        assert '已停止动画' in page.locator('#status').inner_text()
        print('PASS: reduced-motion startup, small phone width, exit action')
        context.close()
        browser.close()
finally:
    server.shutdown()
