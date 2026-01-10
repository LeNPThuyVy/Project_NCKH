from __future__ import annotations

import argparse
from pathlib import Path


def _iter_images(root: Path):
    exts = {".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff", ".webp"}
    for p in sorted(root.rglob("*")):
        if p.suffix.lower() in exts:
            yield p


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--images", default="dataset_yolo/images", help="images root folder")
    ap.add_argument("--limit", type=int, default=0, help="0 = no limit")
    args = ap.parse_args()

    repo_root = Path(__file__).resolve().parent
    images_root = (repo_root / args.images).resolve()

    if not images_root.exists():
        print(f"images folder not found: {images_root}")
        return 2

    # Prefer PIL, fallback to OpenCV
    reader = None
    reader_name = None
    try:
        from PIL import Image  # type: ignore

        def _read(p: Path):
            with Image.open(p) as im:
                im.verify()  # verifies without decoding full image

        reader = _read
        reader_name = "PIL.Image.verify"
    except Exception:
        try:
            import cv2  # type: ignore

            def _read(p: Path):
                img = cv2.imread(str(p))
                if img is None:
                    raise ValueError("cv2.imread returned None")

            reader = _read
            reader_name = "cv2.imread"
        except Exception:
            print("Neither Pillow nor OpenCV is available to validate images.")
            print("Install one of them: `pip install pillow` or ensure `opencv-python` is installed.")
            return 3

    bad: list[tuple[Path, str]] = []
    total = 0
    for p in _iter_images(images_root):
        total += 1
        try:
            assert reader is not None
            reader(p)
        except Exception as e:
            bad.append((p, str(e)))
            if len(bad) >= 30:
                break
        if args.limit and total >= args.limit:
            break

    print(f"Reader: {reader_name}")
    if bad:
        print(f"Bad images found: {len(bad)} (showing {min(len(bad), 30)})")
        for p, msg in bad[:30]:
            print(f"- {p}: {msg}")
        return 1

    print(f"No corrupt/unreadable images detected in {total} files scanned.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
