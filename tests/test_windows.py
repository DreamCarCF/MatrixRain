"""Windows-only native self-test; runs a temporary copy without opening full-screen windows."""
from pathlib import Path
import shutil
import subprocess
import tempfile
root=Path(__file__).resolve().parents[1]
with tempfile.TemporaryDirectory() as folder:
    exe=Path(folder)/'MatrixCode.scr'
    shutil.copy2(root/'windows'/'MatrixCode.scr',exe)
    result=subprocess.run([str(exe),'/test'],capture_output=True,timeout=20)
    report=(Path(folder)/'native-test.txt').read_text()
    print(report)
    assert result.returncode==0 and report.startswith('PASS'),report
    assert (Path(folder)/'native-preview.png').exists()
    shutil.copy2(Path(folder)/'native-preview.png',root/'docs'/'preview-windows.png')
