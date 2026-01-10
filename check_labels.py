from __future__ import annotations

from pathlib import Path
import math


def _infer_nc_from_data_yaml(data_yaml: Path) -> int | None:
    try:
        lines = data_yaml.read_text(encoding="utf-8", errors="ignore").splitlines()
    except Exception:
        return None

    names: dict[int, str] = {}
    for line in lines:
        if ":" not in line:
            continue
        k, v = line.split(":", 1)
        k = k.strip()
        v = v.strip()
        if k.isdigit():
            names[int(k)] = v

    return (max(names.keys()) + 1) if names else None


def main() -> int:
    repo_root = Path(__file__).resolve().parent
    labels_root = repo_root / "dataset_yolo" / "labels"
    data_yaml = repo_root / "dataset_yolo" / "data.yaml"

    nc = _infer_nc_from_data_yaml(data_yaml)

    issues: list[tuple[Path, str]] = []

    if not labels_root.exists():
        print(f"labels folder not found: {labels_root}")
        return 2

    for p in sorted(labels_root.rglob("*.txt")):
        try:
            txt = p.read_text(encoding="utf-8", errors="ignore").strip()
        except Exception as e:
            issues.append((p, f"read_error: {e}"))
            continue

        if not txt:
            continue

        for line_no, line in enumerate(txt.splitlines(), start=1):
            parts = line.strip().split()
            if len(parts) != 5:
                issues.append((p, f"line {line_no}: expected 5 items, got {len(parts)} -> {line!r}"))
                continue

            try:
                cls = int(float(parts[0]))
                x, y, w, h = map(float, parts[1:])
            except Exception as e:
                issues.append((p, f"line {line_no}: parse_error: {e} -> {line!r}"))
                continue

            if nc is not None and not (0 <= cls < nc):
                issues.append((p, f"line {line_no}: class {cls} out of range [0,{nc - 1}]"))

            vals = (x, y, w, h)
            if any((not math.isfinite(v)) for v in vals):
                issues.append((p, f"line {line_no}: non-finite coords {vals}"))

            if any((v < 0 or v > 1) for v in vals):
                issues.append((p, f"line {line_no}: coords out of [0,1] {vals}"))

            if w <= 0 or h <= 0:
                issues.append((p, f"line {line_no}: w/h must be >0 {vals}"))

    if issues:
        print(f"Found {len(issues)} label issues. Showing first 30:\n")
        for p, msg in issues[:30]:
            print(f"- {p}: {msg}")
        return 1

    print("No obvious label-format issues found (class/coords).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
