#!/bin/bash
# Offline browser edition. Does not install a native macOS screen saver.
set -e
HERE="$(cd -- "$(dirname -- "$0")" && pwd)"
/usr/bin/open "$HERE/../index.html"
