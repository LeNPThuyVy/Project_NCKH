import random
import shutil
import threading
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List, Optional, Tuple

import tkinter as tk
from tkinter import ttk, filedialog, messagebox

import yaml
import cv2
import numpy as np
from PIL import Image, ImageTk

from ultralytics import YOLO

IMG_EXTS = {".jpg", ".jpeg", ".png", ".bmp", ".webp"}


# =========================
# Core helpers
# =========================
@dataclass
class YamlConfig:
    root_path: Path
    train_rel: str
    val_rel: str
    test_rel: str
    names: Dict[int, str]        # id -> name
    name_to_id: Dict[str, int]   # name -> id


def load_data_yaml(data_yaml_path: Path) -> YamlConfig:
    data = yaml.safe_load(data_yaml_path.read_text(encoding="utf-8"))
    base_dir = data_yaml_path.parent

    root_path = Path(data["path"])
    if not root_path.is_absolute():
        root_path = (base_dir / root_path).resolve()

    train_rel = str(data["train"])
    val_rel = str(data["val"])
    test_rel = str(data.get("test", "images/test"))

    names_raw = data.get("names", {})
    if isinstance(names_raw, list):
        names = {i: str(n) for i, n in enumerate(names_raw)}
    else:
        names = {int(k): str(v) for k, v in names_raw.items()}

    name_to_id = {v: k for k, v in names.items()}
    return YamlConfig(root_path, train_rel, val_rel, test_rel, names, name_to_id)


def ensure_dirs(cfg: YamlConfig) -> Tuple[Path, Path, Path, Path, Path, Path]:
    train_img = cfg.root_path / cfg.train_rel
    val_img = cfg.root_path / cfg.val_rel
    test_img = cfg.root_path / cfg.test_rel

    def labels_dir(images_dir: Path) -> Path:
        parts = list(images_dir.parts)
        if "images" in parts:
            idx = parts.index("images")
            parts[idx] = "labels"
            return Path(*parts)
        return images_dir.parent.parent / "labels" / images_dir.name

    train_lab = labels_dir(train_img)
    val_lab = labels_dir(val_img)
    test_lab = labels_dir(test_img)

    for d in [train_img, val_img, test_img, train_lab, val_lab, test_lab]:
        d.mkdir(parents=True, exist_ok=True)

    return train_img, val_img, test_img, train_lab, val_lab, test_lab


def list_images(raw_dir: Path) -> List[Path]:
    files: List[Path] = []
    for p in raw_dir.rglob("*"):
        if p.is_file() and p.suffix.lower() in IMG_EXTS:
            files.append(p)
    return sorted(files)


def infer_class_id_from_raw(raw_path: Path, cfg: YamlConfig) -> Optional[int]:
    class_name = raw_path.parent.name
    return cfg.name_to_id.get(class_name, None)


def split_items(items: List[Path], train_ratio: float, val_ratio: float, seed: int):
    if train_ratio <= 0 or train_ratio >= 1:
        raise ValueError("train_ratio phải trong (0, 1)")
    if val_ratio < 0 or val_ratio >= 1:
        raise ValueError("val_ratio phải trong [0, 1)")
    if train_ratio + val_ratio >= 1:
        raise ValueError("train_ratio + val_ratio phải < 1")

    rnd = random.Random(seed)
    items = items[:]
    rnd.shuffle(items)
    n = len(items)
    n_train = int(n * train_ratio)
    n_val = int(n * val_ratio)
    train = items[:n_train]
    val = items[n_train:n_train + n_val]
    test = items[n_train + n_val:]
    return train, val, test


def safe_copy_flat(src: Path, out_dir: Path) -> Path:
    out_dir.mkdir(parents=True, exist_ok=True)
    dst = out_dir / src.name
    if dst.exists():
        stem, suf = src.stem, src.suffix
        k = 1
        while True:
            cand = out_dir / f"{stem}_{k}{suf}"
            if not cand.exists():
                dst = cand
                break
            k += 1
    shutil.copy2(src, dst)
    return dst


def copy_with_manifest(raw_items: List[Path], out_dir: Path) -> List[Tuple[Path, Path]]:
    manifest: List[Tuple[Path, Path]] = []
    for raw in raw_items:
        copied = safe_copy_flat(raw, out_dir)
        manifest.append((raw, copied))
    return manifest


def xyxy_to_xywhn(xyxy, w: int, h: int):
    x1, y1, x2, y2 = xyxy
    x1 = max(0, min(float(x1), w - 1))
    x2 = max(0, min(float(x2), w - 1))
    y1 = max(0, min(float(y1), h - 1))
    y2 = max(0, min(float(y2), h - 1))
    bw = max(1.0, x2 - x1)
    bh = max(1.0, y2 - y1)
    cx = x1 + bw / 2.0
    cy = y1 + bh / 2.0
    return cx / w, cy / h, bw / w, bh / h


def save_label_file(label_path: Path, lines: List[str]):
    label_path.parent.mkdir(parents=True, exist_ok=True)
    label_path.write_text("\n".join(lines) + ("\n" if lines else ""), encoding="utf-8")


def predict_boxes(model: YOLO, img_path: Path, conf: float, iou: float, strategy: str):
    im = cv2.imread(str(img_path))
    if im is None:
        return None, []
    h, w = im.shape[:2]

    results = model.predict(source=str(img_path), conf=conf, iou=iou, verbose=False)
    r0 = results[0]
    boxes = r0.boxes
    if boxes is None or len(boxes) == 0:
        return (w, h), []

    cand = []
    for b in boxes:
        x1, y1, x2, y2 = b.xyxy[0].tolist()
        area = max(0.0, (x2 - x1)) * max(0.0, (y2 - y1))
        cand.append((area, (x1, y1, x2, y2)))

    if strategy == "largest":
        cand.sort(key=lambda t: t[0], reverse=True)
        return (w, h), [cand[0][1]]
    elif strategy == "all":
        return (w, h), [c[1] for c in cand]
    else:
        raise ValueError("strategy phải là 'largest' hoặc 'all'")


def draw_boxes(img_bgr: np.ndarray, boxes_xyxy: List[Tuple[float, float, float, float]]) -> np.ndarray:
    out = img_bgr.copy()
    for (x1, y1, x2, y2) in boxes_xyxy:
        cv2.rectangle(out, (int(x1), int(y1)), (int(x2), int(y2)), (0, 255, 0), 2)
    return out


# =========================
# Manual Annotator (when empty)
# =========================
class ManualAnnotator(tk.Toplevel):
    """
    - Vẽ bbox bằng chuột (click-drag-release)
    - Chọn class từ dropdown (names trong data.yaml)
    - Save: ghi 1 bbox (hoặc nhiều bbox nếu bạn vẽ nhiều) theo YOLO format
    """
    def __init__(self, parent, img_path: Path, class_names: Dict[int, str]):
        super().__init__(parent)
        self.title(f"Manual Annotator - {img_path.name}")
        self.geometry("1100x720")
        self.resizable(True, True)

        self.img_path = img_path
        self.class_names = class_names
        self.id_list = sorted(class_names.keys())
        self.name_list = [f"{i}: {class_names[i]}" for i in self.id_list]

        self.result_lines: Optional[List[str]] = None  # set when saved
        self._start = None
        self._rect_id = None
        self._rects: List[Tuple[int, int, int, int, int]] = []  # (cid, x1,y1,x2,y2) in original image coords

        # load image
        bgr = cv2.imread(str(img_path))
        if bgr is None:
            raise ValueError(f"Không đọc được ảnh: {img_path}")
        self.img_bgr = bgr
        self.img_h, self.img_w = bgr.shape[:2]

        # UI
        self._build_ui()

        # render
        self._render_image()

        self.protocol("WM_DELETE_WINDOW", self._on_close)

    def _build_ui(self):
        root = ttk.Frame(self, padding=10)
        root.pack(fill=tk.BOTH, expand=True)

        left = ttk.Frame(root)
        left.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

        right = ttk.Frame(root, width=320)
        right.pack(side=tk.RIGHT, fill=tk.Y)

        # Canvas for drawing
        self.canvas = tk.Canvas(left, bg="black")
        self.canvas.pack(fill=tk.BOTH, expand=True)

        self.canvas.bind("<ButtonPress-1>", self._on_mouse_down)
        self.canvas.bind("<B1-Motion>", self._on_mouse_move)
        self.canvas.bind("<ButtonRelease-1>", self._on_mouse_up)

        # Controls
        ttk.Label(right, text="Gán nhãn thủ công", font=("Segoe UI", 12, "bold")).pack(anchor="w", pady=(0, 8))

        ttk.Label(right, text="Class").pack(anchor="w")
        self.var_class = tk.StringVar(value=self.name_list[0] if self.name_list else "")
        self.cbo = ttk.Combobox(right, textvariable=self.var_class, values=self.name_list, state="readonly")
        self.cbo.pack(fill=tk.X, pady=(0, 10))

        ttk.Button(right, text="Undo last box", command=self._undo).pack(fill=tk.X, pady=4)
        ttk.Button(right, text="Clear all", command=self._clear).pack(fill=tk.X, pady=4)

        ttk.Separator(right).pack(fill=tk.X, pady=12)

        ttk.Button(right, text="Save", command=self._save).pack(fill=tk.X, pady=4)
        ttk.Button(right, text="Skip", command=self._skip).pack(fill=tk.X, pady=4)

        ttk.Label(
            right,
            text="Hướng dẫn:\n- Kéo chuột để vẽ bbox\n- Có thể vẽ nhiều bbox\n- Save để ghi label YOLO",
            wraplength=300
        ).pack(anchor="w", pady=(12, 0))

        # list boxes info
        ttk.Label(right, text="Boxes đã vẽ").pack(anchor="w", pady=(12, 4))
        self.lst = tk.Listbox(right, height=12)
        self.lst.pack(fill=tk.X)

    def _render_image(self):
        # fit image to canvas area but keep scale; store scale & offsets
        self.update_idletasks()
        cw = max(1, self.canvas.winfo_width())
        ch = max(1, self.canvas.winfo_height())

        rgb = cv2.cvtColor(self.img_bgr, cv2.COLOR_BGR2RGB)
        pil = Image.fromarray(rgb)

        scale = min(cw / self.img_w, ch / self.img_h, 1.0)
        self.scale = scale
        disp_w, disp_h = int(self.img_w * scale), int(self.img_h * scale)
        self.disp_w, self.disp_h = disp_w, disp_h

        pil = pil.resize((disp_w, disp_h))
        self.tk_img = ImageTk.PhotoImage(pil)

        self.canvas.delete("all")
        # center image
        self.off_x = (cw - disp_w) // 2
        self.off_y = (ch - disp_h) // 2

        self.canvas.create_image(self.off_x, self.off_y, anchor="nw", image=self.tk_img)

        # redraw stored rects
        for cid, x1, y1, x2, y2 in self._rects:
            self._draw_rect_on_canvas(x1, y1, x2, y2)

    def _canvas_to_img_xy(self, cx, cy) -> Optional[Tuple[int, int]]:
        # convert canvas coordinate to original image coordinate
        x = cx - self.off_x
        y = cy - self.off_y
        if x < 0 or y < 0 or x >= self.disp_w or y >= self.disp_h:
            return None
        ix = int(x / self.scale)
        iy = int(y / self.scale)
        ix = max(0, min(ix, self.img_w - 1))
        iy = max(0, min(iy, self.img_h - 1))
        return ix, iy

    def _draw_rect_on_canvas(self, x1, y1, x2, y2):
        # convert original image coords to canvas coords
        cx1 = self.off_x + int(x1 * self.scale)
        cy1 = self.off_y + int(y1 * self.scale)
        cx2 = self.off_x + int(x2 * self.scale)
        cy2 = self.off_y + int(y2 * self.scale)
        self.canvas.create_rectangle(cx1, cy1, cx2, cy2, outline="lime", width=2)

    def _current_class_id(self) -> int:
        # parse "id: name"
        s = self.var_class.get()
        cid = int(s.split(":")[0].strip())
        return cid

    def _on_mouse_down(self, event):
        pt = self._canvas_to_img_xy(event.x, event.y)
        if pt is None:
            return
        self._start = pt
        # draw temp rect
        if self._rect_id is not None:
            self.canvas.delete(self._rect_id)
            self._rect_id = None

    def _on_mouse_move(self, event):
        if self._start is None:
            return
        pt = self._canvas_to_img_xy(event.x, event.y)
        if pt is None:
            return
        x1, y1 = self._start
        x2, y2 = pt
        # convert to canvas coords for temp rect
        cx1 = self.off_x + int(x1 * self.scale)
        cy1 = self.off_y + int(y1 * self.scale)
        cx2 = self.off_x + int(x2 * self.scale)
        cy2 = self.off_y + int(y2 * self.scale)

        if self._rect_id is not None:
            self.canvas.coords(self._rect_id, cx1, cy1, cx2, cy2)
        else:
            self._rect_id = self.canvas.create_rectangle(cx1, cy1, cx2, cy2, outline="yellow", width=2)

    def _on_mouse_up(self, event):
        if self._start is None:
            return
        pt = self._canvas_to_img_xy(event.x, event.y)
        if pt is None:
            # cancel temp
            self._start = None
            if self._rect_id is not None:
                self.canvas.delete(self._rect_id)
                self._rect_id = None
            return

        x1, y1 = self._start
        x2, y2 = pt
        self._start = None
        if self._rect_id is not None:
            self.canvas.delete(self._rect_id)
            self._rect_id = None

        # normalize to x1<x2, y1<y2
        xa, xb = sorted([x1, x2])
        ya, yb = sorted([y1, y2])

        # minimal box size
        if (xb - xa) < 5 or (yb - ya) < 5:
            return

        cid = self._current_class_id()
        self._rects.append((cid, xa, ya, xb, yb))
        self._draw_rect_on_canvas(xa, ya, xb, yb)

        self.lst.insert("end", f"cid={cid}  ({xa},{ya})-({xb},{yb})")

    def _undo(self):
        if not self._rects:
            return
        self._rects.pop()
        self.lst.delete("end")
        self._render_image()

    def _clear(self):
        self._rects.clear()
        self.lst.delete(0, "end")
        self._render_image()

    def _save(self):
        # convert rects to YOLO lines
        if not self._rects:
            if messagebox.askyesno("No boxes", "Chưa vẽ bbox nào. Bạn muốn lưu label rỗng không?"):
                self.result_lines = []
                self.destroy()
            return

        lines = []
        for cid, x1, y1, x2, y2 in self._rects:
            xc, yc, bw, bh = xyxy_to_xywhn((x1, y1, x2, y2), self.img_w, self.img_h)
            lines.append(f"{cid} {xc:.6f} {yc:.6f} {bw:.6f} {bh:.6f}")

        self.result_lines = lines
        self.destroy()

    def _skip(self):
        self.result_lines = None
        self.destroy()

    def _on_close(self):
        # treat close as skip (no overwrite)
        self.result_lines = None
        self.destroy()


# =========================
# Tkinter App
# =========================
class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("YOLO Dataset Builder (Tkinter) - Auto + Manual on Empty")
        self.geometry("1280x780")

        self.cfg: Optional[YamlConfig] = None
        self.raw_images: List[Path] = []
        self.model: Optional[YOLO] = None

        self.preview_imgtk = None
        self._build_ui()

    def _build_ui(self):
        left = ttk.Frame(self, padding=10)
        left.pack(side=tk.LEFT, fill=tk.Y)

        right = ttk.Frame(self, padding=10)
        right.pack(side=tk.RIGHT, fill=tk.BOTH, expand=True)

        self.var_yaml = tk.StringVar(value="data.yaml")
        self.var_raw = tk.StringVar(value="raw_images")
        self.var_model = tk.StringVar(value="yolov8n.pt")

        self.var_train = tk.DoubleVar(value=0.8)
        self.var_val = tk.DoubleVar(value=0.1)
        self.var_seed = tk.IntVar(value=42)

        self.var_conf = tk.DoubleVar(value=0.25)
        self.var_iou = tk.DoubleVar(value=0.45)
        self.var_strategy = tk.StringVar(value="largest")

        self.var_manual_on_empty = tk.BooleanVar(value=True)

        self.status = tk.StringVar(value="Ready")
        self.progress = tk.IntVar(value=0)

        ttk.Label(left, text="Cấu hình", font=("Segoe UI", 12, "bold")).pack(anchor="w", pady=(0, 8))
        self._path_row(left, "data.yaml", self.var_yaml, self.browse_yaml)
        self._path_row(left, "raw dir", self.var_raw, self.browse_raw)

        row = ttk.Frame(left)
        row.pack(fill=tk.X, pady=4)
        ttk.Label(row, text="YOLO model", width=12).pack(side=tk.LEFT)
        ttk.Entry(row, textvariable=self.var_model).pack(side=tk.LEFT, fill=tk.X, expand=True)
        ttk.Button(row, text="Load", command=self.load_model).pack(side=tk.LEFT, padx=4)

        ttk.Separator(left).pack(fill=tk.X, pady=10)

        ttk.Label(left, text="Split", font=("Segoe UI", 11, "bold")).pack(anchor="w")
        self._spin(left, "train", self.var_train, 0.50, 0.95, 0.01)
        self._spin(left, "val", self.var_val, 0.00, 0.40, 0.01)
        self._spin_int(left, "seed", self.var_seed, 0, 100000, 1)

        ttk.Separator(left).pack(fill=tk.X, pady=10)

        ttk.Label(left, text="Detect", font=("Segoe UI", 11, "bold")).pack(anchor="w")
        self._spin(left, "conf", self.var_conf, 0.05, 0.95, 0.01)
        self._spin(left, "iou", self.var_iou, 0.05, 0.95, 0.01)

        row = ttk.Frame(left)
        row.pack(fill=tk.X, pady=4)
        ttk.Label(row, text="strategy", width=12).pack(side=tk.LEFT)
        ttk.Combobox(row, textvariable=self.var_strategy, values=["largest", "all"], state="readonly").pack(
            side=tk.LEFT, fill=tk.X, expand=True
        )

        ttk.Checkbutton(
            left, text="Nếu empty label -> mở tool vẽ bbox thủ công",
            variable=self.var_manual_on_empty
        ).pack(anchor="w", pady=(8, 0))

        ttk.Separator(left).pack(fill=tk.X, pady=10)
        ttk.Button(left, text="Load & Validate", command=self.load_and_validate).pack(fill=tk.X, pady=4)
        ttk.Button(left, text="Preview Random (raw)", command=self.preview_random).pack(fill=tk.X, pady=4)
        ttk.Button(left, text="Build Dataset", command=self.build_async).pack(fill=tk.X, pady=10)

        ttk.Label(left, textvariable=self.status, foreground="blue", wraplength=360).pack(anchor="w", pady=(4, 6))
        ttk.Progressbar(left, variable=self.progress, maximum=100).pack(fill=tk.X, pady=(0, 10))

        ttk.Label(right, text="Preview", font=("Segoe UI", 12, "bold")).pack(anchor="w")
        self.preview = tk.Label(right, bd=1, relief=tk.SOLID)
        self.preview.pack(fill=tk.BOTH, expand=True, pady=(6, 10))

        ttk.Label(right, text="Logs", font=("Segoe UI", 12, "bold")).pack(anchor="w")
        self.log = tk.Text(right, height=14)
        self.log.pack(fill=tk.X)
        self.log.configure(state="disabled")

    def _path_row(self, parent, label, var, browse_cmd):
        row = ttk.Frame(parent)
        row.pack(fill=tk.X, pady=4)
        ttk.Label(row, text=label, width=12).pack(side=tk.LEFT)
        ttk.Entry(row, textvariable=var).pack(side=tk.LEFT, fill=tk.X, expand=True)
        ttk.Button(row, text="Browse", command=browse_cmd).pack(side=tk.LEFT, padx=4)

    def _spin(self, parent, name, var, from_, to, inc):
        row = ttk.Frame(parent)
        row.pack(fill=tk.X, pady=4)
        ttk.Label(row, text=name, width=12).pack(side=tk.LEFT)
        ttk.Spinbox(row, textvariable=var, from_=from_, to=to, increment=inc).pack(
            side=tk.LEFT, fill=tk.X, expand=True
        )

    def _spin_int(self, parent, name, var, from_, to, inc):
        row = ttk.Frame(parent)
        row.pack(fill=tk.X, pady=4)
        ttk.Label(row, text=name, width=12).pack(side=tk.LEFT)
        ttk.Spinbox(row, textvariable=var, from_=from_, to=to, increment=inc).pack(
            side=tk.LEFT, fill=tk.X, expand=True
        )

    def log_line(self, s: str):
        self.log.configure(state="normal")
        self.log.insert("end", s + "\n")
        self.log.see("end")
        self.log.configure(state="disabled")

    def browse_yaml(self):
        p = filedialog.askopenfilename(filetypes=[("YAML", "*.yaml *.yml"), ("All", "*.*")])
        if p:
            self.var_yaml.set(p)

    def browse_raw(self):
        p = filedialog.askdirectory()
        if p:
            self.var_raw.set(p)

    def load_model(self):
        try:
            self.status.set("Loading model...")
            self.update_idletasks()
            self.model = YOLO(self.var_model.get().strip())
            self.status.set("Model loaded.")
            self.log_line(f"[OK] Loaded model: {self.var_model.get().strip()}")
        except Exception as e:
            self.model = None
            self.status.set("Model load failed.")
            messagebox.showerror("Load model failed", str(e))

    def load_and_validate(self):
        try:
            yaml_path = Path(self.var_yaml.get().strip()).resolve()
            raw_dir = Path(self.var_raw.get().strip()).resolve()

            if not yaml_path.exists():
                raise FileNotFoundError(f"Không thấy data.yaml: {yaml_path}")
            if not raw_dir.exists():
                raise FileNotFoundError(f"Không thấy raw dir: {raw_dir}")

            self.cfg = load_data_yaml(yaml_path)
            self.raw_images = list_images(raw_dir)

            if not self.raw_images:
                raise ValueError("Không tìm thấy ảnh trong raw dir.")

            parents = sorted({p.parent.name for p in self.raw_images})
            missing = [n for n in parents if n not in self.cfg.name_to_id]
            if missing:
                self.log_line("[WARN] Folder raw không khớp names trong data.yaml: " + ", ".join(missing))
                self.log_line("       Các ảnh trong folder này sẽ bị SKIP khi build.")

            self.log_line(f"[OK] Dataset root: {self.cfg.root_path}")
            self.log_line(f"[OK] Names: {self.cfg.names}")
            self.log_line(f"[OK] Raw images: {len(self.raw_images)}")
            self.status.set("Validated.")
            messagebox.showinfo("OK", "Load & Validate thành công.")
        except Exception as e:
            self.status.set("Validate failed.")
            messagebox.showerror("Validate failed", str(e))

    def preview_random(self):
        if not self.raw_images:
            self.load_and_validate()
            if not self.raw_images:
                return

        if self.model is None:
            self.load_model()
            if self.model is None:
                return

        raw = random.choice(self.raw_images)
        conf = float(self.var_conf.get())
        iou = float(self.var_iou.get())
        strategy = self.var_strategy.get().strip()

        try:
            _, boxes = predict_boxes(self.model, raw, conf, iou, strategy)
            im = cv2.imread(str(raw))
            vis = draw_boxes(im, boxes)
            vis_rgb = cv2.cvtColor(vis, cv2.COLOR_BGR2RGB)

            pil = Image.fromarray(vis_rgb)
            w, h = pil.size
            max_w, max_h = 920, 560
            scale = min(max_w / w, max_h / h, 1.0)
            pil = pil.resize((int(w * scale), int(h * scale)))

            self.preview_imgtk = ImageTk.PhotoImage(pil)
            self.preview.configure(image=self.preview_imgtk)

            cid = infer_class_id_from_raw(raw, self.cfg) if self.cfg else None
            self.status.set(f"Preview: {raw.name} | folder={raw.parent.name} | class_id={cid} | boxes={len(boxes)}")
            self.log_line(f"[OK] Preview raw={raw.name} class_id={cid} boxes={len(boxes)}")
        except Exception as e:
            messagebox.showerror("Preview failed", str(e))

    def build_async(self):
        t = threading.Thread(target=self.build_dataset, daemon=True)
        t.start()

    def build_dataset(self):
        try:
            self.progress.set(0)
            self.status.set("Preparing...")
            self.update_idletasks()

            if self.cfg is None or not self.raw_images:
                self.load_and_validate()
                if self.cfg is None or not self.raw_images:
                    return

            if self.model is None:
                self.load_model()
                if self.model is None:
                    return

            train_ratio = float(self.var_train.get())
            val_ratio = float(self.var_val.get())
            seed = int(self.var_seed.get())

            conf = float(self.var_conf.get())
            iou = float(self.var_iou.get())
            strategy = self.var_strategy.get().strip()
            manual_on_empty = bool(self.var_manual_on_empty.get())

            train_img, val_img, test_img, train_lab, val_lab, test_lab = ensure_dirs(self.cfg)

            train_raw, val_raw, test_raw = split_items(self.raw_images, train_ratio, val_ratio, seed)
            self.log_line(f"[OK] Split raw: train={len(train_raw)} val={len(val_raw)} test={len(test_raw)}")

            self.status.set("Copying images...")
            self.progress.set(10)
            train_manifest = copy_with_manifest(train_raw, train_img)
            val_manifest = copy_with_manifest(val_raw, val_img)
            test_manifest = copy_with_manifest(test_raw, test_img)
            self.log_line("[OK] Copied images -> dataset/images (flatten).")

            self.status.set("Labeling train...")
            self.progress.set(25)
            ok1, sk1, empty1, manual1 = self._label_from_manifest(train_manifest, train_lab, conf, iou, strategy, manual_on_empty)

            self.status.set("Labeling val...")
            self.progress.set(55)
            ok2, sk2, empty2, manual2 = self._label_from_manifest(val_manifest, val_lab, conf, iou, strategy, manual_on_empty)

            self.status.set("Labeling test...")
            self.progress.set(75)
            ok3, sk3, empty3, manual3 = self._label_from_manifest(test_manifest, test_lab, conf, iou, strategy, manual_on_empty)

            self.progress.set(100)
            self.status.set("DONE")

            self.log_line(
                f"[DONE] ok={ok1+ok2+ok3} skipped={sk1+sk2+sk3} "
                f"empty={empty1+empty2+empty3} manual_saved={manual1+manual2+manual3}"
            )
            self.log_line(f"[OUT] dataset root: {self.cfg.root_path}")

            messagebox.showinfo(
                "DONE",
                "Xuất dataset xong.\n"
                f"Root: {self.cfg.root_path}\n"
                f"Empty (trước manual): {empty1+empty2+empty3}\n"
                f"Manual saved: {manual1+manual2+manual3}\n"
            )
        except Exception as e:
            self.status.set("Build failed.")
            messagebox.showerror("Build failed", str(e))

    def _label_from_manifest(
        self,
        manifest: List[Tuple[Path, Path]],
        labels_dir: Path,
        conf: float,
        iou: float,
        strategy: str,
        manual_on_empty: bool
    ) -> Tuple[int, int, int, int]:
        """
        Returns: ok, skipped, empty_detected, manual_saved
        - empty_detected: số ảnh YOLO detect 0 bbox
        - manual_saved: số ảnh được user vẽ và save bbox (ghi label không rỗng hoặc rỗng theo user chọn)
        """
        ok = 0
        skipped = 0
        empty = 0
        manual_saved = 0

        for raw_path, img_path in manifest:
            class_id = infer_class_id_from_raw(raw_path, self.cfg)
            if class_id is None:
                skipped += 1
                continue

            im = cv2.imread(str(img_path))
            if im is None:
                skipped += 1
                continue
            h, w = im.shape[:2]

            # detect
            _, boxes_xyxy = predict_boxes(self.model, img_path, conf, iou, strategy)

            lines = []
            for (x1, y1, x2, y2) in boxes_xyxy:
                xc, yc, bw, bh = xyxy_to_xywhn((x1, y1, x2, y2), w, h)
                lines.append(f"{class_id} {xc:.6f} {yc:.6f} {bw:.6f} {bh:.6f}")

            if len(lines) == 0:
                empty += 1
                if manual_on_empty:
                    # Manual dialog (runs on main thread)
                    # Important: we must call it via self.after to ensure UI thread safety when build is in worker thread
                    result_holder = {"lines": None}

                    def open_annotator():
                        # allow choose ANY class (not only raw class)
                        dlg = ManualAnnotator(self, img_path, self.cfg.names)
                        self.wait_window(dlg)
                        result_holder["lines"] = dlg.result_lines

                    self.after(0, open_annotator)

                    # Wait until dialog done
                    while result_holder["lines"] is None and self.winfo_exists():
                        # result_lines None means skipped/closed
                        # We need a way to differentiate "still running" vs "skip"
                        # We'll handle by polling a flag - simplest: add a sentinel
                        # We'll set sentinel after dialog closes by checking existence of any toplevel? too complex.
                        # Practical approach: block with update
                        self.update()
                        # if dialog saved empty list, it's not None; if skipped, it's None -> we will treat as skip.
                        # To avoid infinite loop on skip, we must detect that dialog closed.
                        # We'll detect by checking if no Toplevel of that title exists.
                        # Simplify: set a second key when dialog finished.
                        break

                    # Improved: run a synchronous helper instead (since Tk must run in main thread):
                    # We'll do a safer synchronous route: call annotator directly if build isn't threaded.
                    # However build is threaded; so we use a second modal approach below:
                    # Instead of polling, we re-open synchronously by scheduling and waiting using an event-like variable.

                    # Because the above polling is fragile, we'll implement a clean modal call below:
                    # -> re-run properly:
                    result_lines = self._run_manual_dialog(img_path)
                    if result_lines is None:
                        # user skipped: write empty label file as-is (still ok for YOLO)
                        pass
                    else:
                        lines = result_lines
                        manual_saved += 1

            label_path = labels_dir / f"{img_path.stem}.txt"
            save_label_file(label_path, lines)
            ok += 1

        self.log_line(f"[OK] Labeled: {labels_dir} | ok={ok} skipped={skipped} empty={empty} manual_saved={manual_saved}")
        return ok, skipped, empty, manual_saved

    def _run_manual_dialog(self, img_path: Path) -> Optional[List[str]]:
        """
        Run annotator safely on UI thread and block until close.
        Returns:
          - list[str]: lines to save (can be empty list if user chose to save empty)
          - None: user skipped/closed => keep empty
        """
        if self.cfg is None:
            return None

        # If build runs in worker thread, call dialog via self.after and wait using tk variable
        done = tk.BooleanVar(value=False)
        out = {"lines": None}

        def _open():
            dlg = ManualAnnotator(self, img_path, self.cfg.names)
            self.wait_window(dlg)
            out["lines"] = dlg.result_lines
            done.set(True)

        self.after(0, _open)

        # Wait until done
        while not done.get():
            self.update()

        return out["lines"]


if __name__ == "__main__":
    app = App()
    try:
        style = ttk.Style()
        if "clam" in style.theme_names():
            style.theme_use("clam")
    except Exception:
        pass
    app.mainloop()
