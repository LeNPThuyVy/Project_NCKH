import os
import time
import threading
import queue
import csv
from dataclasses import dataclass, asdict
from typing import Optional, List

import cv2
import numpy as np
from PIL import Image, ImageTk
import tkinter as tk
from tkinter import ttk, filedialog, messagebox

# Ultralytics YOLO
from ultralytics import YOLO


# =========================
# Data structures
# =========================
@dataclass
class DetectionLog:
    ts: str
    source_type: str   # image/video/camera
    source_path: str
    frame_index: int
    class_id: int
    class_name: str
    conf: float
    x1: float
    y1: float
    x2: float
    y2: float


# =========================
# Utilities
# =========================
def bgr_to_tk(bgr: np.ndarray, max_w: int, max_h: int) -> ImageTk.PhotoImage:
    """Convert BGR numpy image to Tk PhotoImage with resize-to-fit."""
    if bgr is None:
        return None
    h, w = bgr.shape[:2]
    if w == 0 or h == 0:
        return None

    scale = min(max_w / w, max_h / h, 1.0)
    new_w, new_h = int(w * scale), int(h * scale)
    resized = cv2.resize(bgr, (new_w, new_h), interpolation=cv2.INTER_AREA)

    rgb = cv2.cvtColor(resized, cv2.COLOR_BGR2RGB)
    pil = Image.fromarray(rgb)
    return ImageTk.PhotoImage(pil)


def ensure_dir(path: str):
    if path and not os.path.exists(path):
        os.makedirs(path, exist_ok=True)


def now_str():
    return time.strftime("%Y-%m-%d %H:%M:%S")


# =========================
# Main App
# =========================
class YoloRunnerUI(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("YOLO Runner UI (Image / Video / Camera)")
        self.geometry("1250x780")
        self.minsize(1100, 700)

        # State
        self.model: Optional[YOLO] = None
        self.model_path: Optional[str] = None
        self.class_names = None

        self.logs: List[DetectionLog] = []

        # Worker threads
        self.worker_thread: Optional[threading.Thread] = None
        self.stop_event = threading.Event()
        self.frame_queue = queue.Queue(maxsize=2)  # to update UI
        self.video_writer = None

        self.current_source_type = ""
        self.current_source_path = ""
        self.current_frame_index = 0
        self.last_fps_time = time.time()
        self.fps = 0.0
        self.fps_counter = 0

        self._build_ui()
        self._poll_queue()

        self.protocol("WM_DELETE_WINDOW", self.on_close)

    # -------------------------
    # UI Layout
    # -------------------------
    def _build_ui(self):
        self.columnconfigure(0, weight=0)
        self.columnconfigure(1, weight=1)
        self.rowconfigure(0, weight=1)

        # Left control panel
        left = ttk.Frame(self, padding=10)
        left.grid(row=0, column=0, sticky="nsw")
        left.columnconfigure(0, weight=1)

        # Right display
        right = ttk.Frame(self, padding=10)
        right.grid(row=0, column=1, sticky="nsew")
        right.columnconfigure(0, weight=1)
        right.rowconfigure(0, weight=1)
        right.rowconfigure(1, weight=0)

        # ---- Model settings
        model_box = ttk.LabelFrame(left, text="Model & Inference Settings", padding=10)
        model_box.grid(row=0, column=0, sticky="ew", pady=(0, 10))
        model_box.columnconfigure(0, weight=1)

        self.model_path_var = tk.StringVar(value="")
        ttk.Label(model_box, text="Model (.pt):").grid(row=0, column=0, sticky="w")
        path_row = ttk.Frame(model_box)
        path_row.grid(row=1, column=0, sticky="ew", pady=(4, 8))
        path_row.columnconfigure(0, weight=1)
        ttk.Entry(path_row, textvariable=self.model_path_var).grid(row=0, column=0, sticky="ew")
        ttk.Button(path_row, text="Browse", command=self.browse_model).grid(row=0, column=1, padx=(6, 0))
        ttk.Button(model_box, text="Load Model", command=self.load_model).grid(row=2, column=0, sticky="ew")

        # Device
        self.device_var = tk.StringVar(value="cpu")
        dev_row = ttk.Frame(model_box)
        dev_row.grid(row=3, column=0, sticky="ew", pady=(8, 0))
        ttk.Label(dev_row, text="Device:").grid(row=0, column=0, sticky="w")
        ttk.Combobox(dev_row, textvariable=self.device_var, values=["cpu", "cuda", "cuda:0", "cuda:1"], width=10, state="readonly").grid(row=0, column=1, padx=(6, 0), sticky="w")

        # Conf / IoU
        self.conf_var = tk.DoubleVar(value=0.25)
        self.iou_var = tk.DoubleVar(value=0.45)

        ttk.Label(model_box, text="Confidence (conf):").grid(row=4, column=0, sticky="w", pady=(10, 0))
        self.conf_scale = ttk.Scale(model_box, from_=0.01, to=0.99, variable=self.conf_var, orient="horizontal")
        self.conf_scale.grid(row=5, column=0, sticky="ew")
        self.conf_label = ttk.Label(model_box, text="0.25")
        self.conf_label.grid(row=6, column=0, sticky="w")

        ttk.Label(model_box, text="IoU threshold:").grid(row=7, column=0, sticky="w", pady=(8, 0))
        self.iou_scale = ttk.Scale(model_box, from_=0.01, to=0.99, variable=self.iou_var, orient="horizontal")
        self.iou_scale.grid(row=8, column=0, sticky="ew")
        self.iou_label = ttk.Label(model_box, text="0.45")
        self.iou_label.grid(row=9, column=0, sticky="w")

        # Class filter
        filter_box = ttk.LabelFrame(left, text="Class Filter", padding=10)
        filter_box.grid(row=1, column=0, sticky="ew", pady=(0, 10))
        filter_box.columnconfigure(0, weight=1)

        ttk.Label(filter_box, text="classes (comma, empty=all):").grid(row=0, column=0, sticky="w")
        self.classes_var = tk.StringVar(value="")
        ttk.Entry(filter_box, textvariable=self.classes_var).grid(row=1, column=0, sticky="ew", pady=(4, 0))
        ttk.Label(filter_box, text="Ví dụ: 0,1 hoặc 2").grid(row=2, column=0, sticky="w", pady=(4, 0))

        # ---- Run modes (tabs)
        mode_box = ttk.LabelFrame(left, text="Run Mode", padding=10)
        mode_box.grid(row=2, column=0, sticky="ew", pady=(0, 10))
        mode_box.columnconfigure(0, weight=1)

        self.notebook = ttk.Notebook(mode_box)
        self.notebook.grid(row=0, column=0, sticky="ew")

        self.tab_image = ttk.Frame(self.notebook, padding=10)
        self.tab_video = ttk.Frame(self.notebook, padding=10)
        self.tab_camera = ttk.Frame(self.notebook, padding=10)

        self.notebook.add(self.tab_image, text="Image")
        self.notebook.add(self.tab_video, text="Video")
        self.notebook.add(self.tab_camera, text="Camera")

        self._build_tab_image()
        self._build_tab_video()
        self._build_tab_camera()

        # ---- Output settings
        out_box = ttk.LabelFrame(left, text="Output & Logging", padding=10)
        out_box.grid(row=3, column=0, sticky="ew")
        out_box.columnconfigure(0, weight=1)

        self.save_annotated_var = tk.BooleanVar(value=True)
        self.save_labels_var = tk.BooleanVar(value=False)  # YOLO txt labels
        self.record_video_var = tk.BooleanVar(value=False)

        ttk.Checkbutton(out_box, text="Save annotated image/video", variable=self.save_annotated_var).grid(row=0, column=0, sticky="w")
        ttk.Checkbutton(out_box, text="Save YOLO labels (.txt) per frame (video/cam)", variable=self.save_labels_var).grid(row=1, column=0, sticky="w")
        ttk.Checkbutton(out_box, text="Record video output (video/cam)", variable=self.record_video_var).grid(row=2, column=0, sticky="w")

        ttk.Label(out_box, text="Output folder:").grid(row=3, column=0, sticky="w", pady=(10, 0))
        self.out_dir_var = tk.StringVar(value=os.path.join(os.getcwd(), "outputs"))
        out_row = ttk.Frame(out_box)
        out_row.grid(row=4, column=0, sticky="ew", pady=(4, 0))
        out_row.columnconfigure(0, weight=1)
        ttk.Entry(out_row, textvariable=self.out_dir_var).grid(row=0, column=0, sticky="ew")
        ttk.Button(out_row, text="Browse", command=self.browse_output_dir).grid(row=0, column=1, padx=(6, 0))

        log_row = ttk.Frame(out_box)
        log_row.grid(row=5, column=0, sticky="ew", pady=(10, 0))
        log_row.columnconfigure(0, weight=1)
        ttk.Button(log_row, text="Export Logs CSV", command=self.export_logs).grid(row=0, column=0, sticky="ew")
        ttk.Button(log_row, text="Clear Logs", command=self.clear_logs).grid(row=0, column=1, padx=(6, 0), sticky="ew")

        # ---- Right display: image panel + status
        display_box = ttk.LabelFrame(right, text="Preview", padding=10)
        display_box.grid(row=0, column=0, sticky="nsew")
        display_box.columnconfigure(0, weight=1)
        display_box.rowconfigure(0, weight=1)

        self.preview_label = ttk.Label(display_box)
        self.preview_label.grid(row=0, column=0, sticky="nsew")

        status = ttk.Frame(right)
        status.grid(row=1, column=0, sticky="ew")
        status.columnconfigure(0, weight=1)

        self.status_var = tk.StringVar(value="Ready.")
        self.fps_var = tk.StringVar(value="FPS: -")
        ttk.Label(status, textvariable=self.status_var).grid(row=0, column=0, sticky="w")
        ttk.Label(status, textvariable=self.fps_var).grid(row=0, column=1, sticky="e")

        # Update conf/iou labels live
        def on_scale_change(_=None):
            self.conf_label.config(text=f"{self.conf_var.get():.2f}")
            self.iou_label.config(text=f"{self.iou_var.get():.2f}")
        self.conf_var.trace_add("write", on_scale_change)
        self.iou_var.trace_add("write", on_scale_change)
        on_scale_change()

    def _build_tab_image(self):
        self.tab_image.columnconfigure(0, weight=1)

        self.image_path_var = tk.StringVar(value="")
        ttk.Label(self.tab_image, text="Image path:").grid(row=0, column=0, sticky="w")
        row = ttk.Frame(self.tab_image)
        row.grid(row=1, column=0, sticky="ew", pady=(4, 8))
        row.columnconfigure(0, weight=1)
        ttk.Entry(row, textvariable=self.image_path_var).grid(row=0, column=0, sticky="ew")
        ttk.Button(row, text="Browse", command=self.browse_image).grid(row=0, column=1, padx=(6, 0))

        btn_row = ttk.Frame(self.tab_image)
        btn_row.grid(row=2, column=0, sticky="ew")
        btn_row.columnconfigure(0, weight=1)
        ttk.Button(btn_row, text="Run on Image", command=self.run_on_image).grid(row=0, column=0, sticky="ew")

    def _build_tab_video(self):
        self.tab_video.columnconfigure(0, weight=1)

        self.video_path_var = tk.StringVar(value="")
        ttk.Label(self.tab_video, text="Video path:").grid(row=0, column=0, sticky="w")
        row = ttk.Frame(self.tab_video)
        row.grid(row=1, column=0, sticky="ew", pady=(4, 8))
        row.columnconfigure(0, weight=1)
        ttk.Entry(row, textvariable=self.video_path_var).grid(row=0, column=0, sticky="ew")
        ttk.Button(row, text="Browse", command=self.browse_video).grid(row=0, column=1, padx=(6, 0))

        control = ttk.Frame(self.tab_video)
        control.grid(row=2, column=0, sticky="ew")
        control.columnconfigure(0, weight=1)
        control.columnconfigure(1, weight=1)
        ttk.Button(control, text="Start Video", command=self.start_video).grid(row=0, column=0, sticky="ew", padx=(0, 6))
        ttk.Button(control, text="Stop", command=self.stop_stream).grid(row=0, column=1, sticky="ew")

    def _build_tab_camera(self):
        self.tab_camera.columnconfigure(0, weight=1)

        cam_row = ttk.Frame(self.tab_camera)
        cam_row.grid(row=0, column=0, sticky="ew")
        ttk.Label(cam_row, text="Camera index:").grid(row=0, column=0, sticky="w")
        self.cam_index_var = tk.IntVar(value=0)
        ttk.Spinbox(cam_row, from_=0, to=10, textvariable=self.cam_index_var, width=6).grid(row=0, column=1, padx=(6, 0), sticky="w")

        control = ttk.Frame(self.tab_camera)
        control.grid(row=1, column=0, sticky="ew", pady=(10, 0))
        control.columnconfigure(0, weight=1)
        control.columnconfigure(1, weight=1)
        ttk.Button(control, text="Start Camera", command=self.start_camera).grid(row=0, column=0, sticky="ew", padx=(0, 6))
        ttk.Button(control, text="Stop", command=self.stop_stream).grid(row=0, column=1, sticky="ew")

        snap_row = ttk.Frame(self.tab_camera)
        snap_row.grid(row=2, column=0, sticky="ew", pady=(10, 0))
        snap_row.columnconfigure(0, weight=1)
        ttk.Button(snap_row, text="Snapshot (save current frame)", command=self.snapshot).grid(row=0, column=0, sticky="ew")

    # -------------------------
    # Browse / Load
    # -------------------------
    def browse_model(self):
        path = filedialog.askopenfilename(
            title="Select YOLO model (.pt)",
            filetypes=[("PyTorch model", "*.pt"), ("All files", "*.*")]
        )
        if path:
            self.model_path_var.set(path)

    def browse_output_dir(self):
        d = filedialog.askdirectory(title="Select output folder")
        if d:
            self.out_dir_var.set(d)

    def browse_image(self):
        path = filedialog.askopenfilename(
            title="Select image",
            filetypes=[("Images", "*.jpg *.jpeg *.png *.bmp *.webp"), ("All files", "*.*")]
        )
        if path:
            self.image_path_var.set(path)

    def browse_video(self):
        path = filedialog.askopenfilename(
            title="Select video",
            filetypes=[("Videos", "*.mp4 *.avi *.mov *.mkv *.webm"), ("All files", "*.*")]
        )
        if path:
            self.video_path_var.set(path)

    def load_model(self):
        path = self.model_path_var.get().strip()
        if not path or not os.path.isfile(path):
            messagebox.showerror("Error", "Model path không hợp lệ.")
            return

        try:
            self.status_var.set("Loading model...")
            self.update_idletasks()
            self.model = YOLO(path)
            self.model_path = path

            # Names can come from model
            try:
                self.class_names = self.model.names
            except Exception:
                self.class_names = None

            self.status_var.set(f"Model loaded: {os.path.basename(path)}")
        except Exception as e:
            self.model = None
            self.model_path = None
            messagebox.showerror("Error", f"Không load được model.\n\n{e}")
            self.status_var.set("Ready.")

    # -------------------------
    # Inference params
    # -------------------------
    def _parse_classes(self) -> Optional[List[int]]:
        raw = self.classes_var.get().strip()
        if not raw:
            return None
        try:
            items = [int(x.strip()) for x in raw.split(",") if x.strip() != ""]
            return items if items else None
        except Exception:
            messagebox.showwarning("Warning", "classes filter không hợp lệ. Bỏ qua filter.")
            return None

    def _ensure_model_ready(self) -> bool:
        if self.model is None:
            messagebox.showerror("Error", "Bạn chưa load model.")
            return False
        return True

    # -------------------------
    # Image run
    # -------------------------
    def run_on_image(self):
        if not self._ensure_model_ready():
            return

        img_path = self.image_path_var.get().strip()
        if not img_path or not os.path.isfile(img_path):
            messagebox.showerror("Error", "Image path không hợp lệ.")
            return

        bgr = cv2.imread(img_path)
        if bgr is None:
            messagebox.showerror("Error", "Không đọc được ảnh (cv2.imread failed).")
            return

        self.current_source_type = "image"
        self.current_source_path = img_path
        self.current_frame_index = 0

        try:
            self.status_var.set("Running inference on image...")
            self.update_idletasks()

            classes = self._parse_classes()
            conf = float(self.conf_var.get())
            iou = float(self.iou_var.get())
            device = self.device_var.get().strip()

            results = self.model.predict(
                source=bgr,
                conf=conf,
                iou=iou,
                device=device,
                classes=classes,
                verbose=False
            )

            res0 = results[0]
            plotted = res0.plot()  # BGR annotated
            self._update_preview(plotted)

            # Save annotated image
            if self.save_annotated_var.get():
                out_dir = self.out_dir_var.get().strip()
                ensure_dir(out_dir)
                base = os.path.splitext(os.path.basename(img_path))[0]
                out_path = os.path.join(out_dir, f"{base}_pred.jpg")
                cv2.imwrite(out_path, plotted)

            # Log detections
            self._append_logs_from_result(res0, source_type="image", source_path=img_path, frame_index=0)

            self.status_var.set("Done (image).")
            self.fps_var.set("FPS: -")
        except Exception as e:
            messagebox.showerror("Error", f"Inference lỗi.\n\n{e}")
            self.status_var.set("Ready.")

    # -------------------------
    # Video / Camera streaming
    # -------------------------
    def start_video(self):
        if not self._ensure_model_ready():
            return
        path = self.video_path_var.get().strip()
        if not path or not os.path.isfile(path):
            messagebox.showerror("Error", "Video path không hợp lệ.")
            return
        self._start_stream(source_type="video", source=path)

    def start_camera(self):
        if not self._ensure_model_ready():
            return
        idx = int(self.cam_index_var.get())
        self._start_stream(source_type="camera", source=idx)

    def stop_stream(self):
        self.stop_event.set()
        self.status_var.set("Stopping...")
        self._release_writer()

    def snapshot(self):
        # Save last shown frame (best effort)
        try:
            last = getattr(self, "_last_frame_bgr", None)
            if last is None:
                messagebox.showinfo("Info", "Chưa có frame để snapshot.")
                return
            out_dir = self.out_dir_var.get().strip()
            ensure_dir(out_dir)
            out_path = os.path.join(out_dir, f"snapshot_{time.strftime('%Y%m%d_%H%M%S')}.jpg")
            cv2.imwrite(out_path, last)
            self.status_var.set(f"Snapshot saved: {os.path.basename(out_path)}")
        except Exception as e:
            messagebox.showerror("Error", f"Snapshot lỗi.\n\n{e}")

    def _start_stream(self, source_type: str, source):
        # Stop old stream if any
        self.stop_stream()
        time.sleep(0.05)
        self.stop_event.clear()

        self.current_source_type = source_type
        self.current_source_path = str(source)
        self.current_frame_index = 0
        self.fps = 0.0
        self.fps_counter = 0
        self.last_fps_time = time.time()

        # Start worker thread
        self.worker_thread = threading.Thread(
            target=self._stream_worker,
            args=(source_type, source),
            daemon=True
        )
        self.worker_thread.start()
        self.status_var.set(f"Running {source_type}...")

    def _stream_worker(self, source_type: str, source):
        cap = cv2.VideoCapture(source)
        if not cap.isOpened():
            self.frame_queue_put(("error", f"Không mở được {source_type}: {source}"))
            return

        # For video, try read FPS & size
        src_fps = cap.get(cv2.CAP_PROP_FPS) or 30.0
        w = int(cap.get(cv2.CAP_PROP_FRAME_WIDTH) or 0)
        h = int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT) or 0)

        classes = self._parse_classes()
        conf = float(self.conf_var.get())
        iou = float(self.iou_var.get())
        device = self.device_var.get().strip()

        # Prepare output video writer if requested (video/cam)
        out_dir = self.out_dir_var.get().strip()
        ensure_dir(out_dir)

        writer_path = None
        if self.record_video_var.get():
            fourcc = cv2.VideoWriter_fourcc(*"mp4v")
            stamp = time.strftime("%Y%m%d_%H%M%S")
            writer_name = f"{source_type}_{stamp}_pred.mp4"
            writer_path = os.path.join(out_dir, writer_name)
            if w > 0 and h > 0:
                self.video_writer = cv2.VideoWriter(writer_path, fourcc, float(src_fps), (w, h))
            else:
                # fallback after first frame
                self.video_writer = None

        frame_index = 0

        while not self.stop_event.is_set():
            ok, frame = cap.read()
            if not ok:
                break

            # Inference
            try:
                results = self.model.predict(
                    source=frame,
                    conf=conf,
                    iou=iou,
                    device=device,
                    classes=classes,
                    verbose=False
                )
                res0 = results[0]
                plotted = res0.plot()

                # Log detections
                self._append_logs_from_result(res0, source_type=source_type, source_path=str(source), frame_index=frame_index)

                # Save labels (.txt) if requested
                if self.save_labels_var.get():
                    self._save_yolo_labels(res0, out_dir, source_type, frame_index, frame.shape[1], frame.shape[0])

                # Setup writer if needed and not initialized
                if self.record_video_var.get():
                    if self.video_writer is None:
                        h2, w2 = plotted.shape[:2]
                        fourcc = cv2.VideoWriter_fourcc(*"mp4v")
                        stamp = time.strftime("%Y%m%d_%H%M%S")
                        writer_name = f"{source_type}_{stamp}_pred.mp4"
                        writer_path = os.path.join(out_dir, writer_name)
                        self.video_writer = cv2.VideoWriter(writer_path, fourcc, float(src_fps), (w2, h2))
                    if self.video_writer is not None:
                        self.video_writer.write(plotted)

                # Update FPS
                self.fps_counter += 1
                tnow = time.time()
                if tnow - self.last_fps_time >= 1.0:
                    self.fps = self.fps_counter / (tnow - self.last_fps_time)
                    self.fps_counter = 0
                    self.last_fps_time = tnow

                # Send frame to UI
                self.frame_queue_put(("frame", plotted, self.fps, source_type, str(source), frame_index))
                frame_index += 1

            except Exception as e:
                self.frame_queue_put(("error", f"Inference lỗi: {e}"))
                break

        cap.release()
        self._release_writer()
        self.frame_queue_put(("done", f"Stopped {source_type}."))

    def _release_writer(self):
        try:
            if self.video_writer is not None:
                self.video_writer.release()
        except Exception:
            pass
        self.video_writer = None

    def frame_queue_put(self, item):
        # avoid UI blocking if queue full
        try:
            if self.frame_queue.full():
                _ = self.frame_queue.get_nowait()
            self.frame_queue.put_nowait(item)
        except Exception:
            pass

    def _poll_queue(self):
        try:
            while True:
                item = self.frame_queue.get_nowait()
                kind = item[0]
                if kind == "frame":
                    _, bgr, fps, stype, spath, fidx = item
                    self.current_source_type = stype
                    self.current_source_path = spath
                    self.current_frame_index = fidx
                    self._last_frame_bgr = bgr.copy()

                    self._update_preview(bgr)
                    self.fps_var.set(f"FPS: {fps:.1f}")
                    self.status_var.set(f"Running {stype} | frame={fidx}")

                elif kind == "error":
                    _, msg = item
                    messagebox.showerror("Error", msg)
                    self.status_var.set("Ready.")
                    self.fps_var.set("FPS: -")
                    self.stop_event.set()
                    self._release_writer()

                elif kind == "done":
                    _, msg = item
                    self.status_var.set(msg)
                    self.fps_var.set("FPS: -")
        except queue.Empty:
            pass
        finally:
            self.after(30, self._poll_queue)

    def _update_preview(self, bgr):
        # Fit to right panel size
        # Approx preview area: window width minus left panel
        max_w = 880
        max_h = 620
        try:
            # dynamic sizing from widget
            w = self.preview_label.winfo_width()
            h = self.preview_label.winfo_height()
            if w > 50 and h > 50:
                max_w, max_h = w, h
        except Exception:
            pass

        tk_img = bgr_to_tk(bgr, max_w=max_w, max_h=max_h)
        if tk_img is not None:
            self.preview_label.configure(image=tk_img)
            self.preview_label.image = tk_img  # keep ref

    # -------------------------
    # Logging / Export
    # -------------------------
    def _append_logs_from_result(self, res0, source_type: str, source_path: str, frame_index: int):
        try:
            boxes = res0.boxes
            if boxes is None:
                return

            names = getattr(res0, "names", None)
            if names is None and self.class_names is not None:
                names = self.class_names

            xyxy = boxes.xyxy.cpu().numpy() if hasattr(boxes.xyxy, "cpu") else np.array(boxes.xyxy)
            confs = boxes.conf.cpu().numpy() if hasattr(boxes.conf, "cpu") else np.array(boxes.conf)
            clss = boxes.cls.cpu().numpy().astype(int) if hasattr(boxes.cls, "cpu") else np.array(boxes.cls).astype(int)

            for (x1, y1, x2, y2), c, cid in zip(xyxy, confs, clss):
                cname = str(cid)
                if isinstance(names, dict) and cid in names:
                    cname = names[cid]
                log = DetectionLog(
                    ts=now_str(),
                    source_type=source_type,
                    source_path=source_path,
                    frame_index=int(frame_index),
                    class_id=int(cid),
                    class_name=str(cname),
                    conf=float(c),
                    x1=float(x1), y1=float(y1), x2=float(x2), y2=float(y2)
                )
                self.logs.append(log)
        except Exception:
            # do not crash the app for logging errors
            pass

    def export_logs(self):
        if not self.logs:
            messagebox.showinfo("Info", "Chưa có log để export.")
            return

        out_dir = self.out_dir_var.get().strip()
        ensure_dir(out_dir)
        out_path = os.path.join(out_dir, f"detections_{time.strftime('%Y%m%d_%H%M%S')}.csv")

        try:
            with open(out_path, "w", newline="", encoding="utf-8") as f:
                writer = csv.DictWriter(f, fieldnames=list(asdict(self.logs[0]).keys()))
                writer.writeheader()
                for log in self.logs:
                    writer.writerow(asdict(log))
            self.status_var.set(f"Exported logs: {os.path.basename(out_path)}")
        except Exception as e:
            messagebox.showerror("Error", f"Export CSV lỗi.\n\n{e}")

    def clear_logs(self):
        self.logs.clear()
        self.status_var.set("Logs cleared.")

    # -------------------------
    # Save YOLO labels (txt)
    # -------------------------
    def _save_yolo_labels(self, res0, out_dir: str, source_type: str, frame_index: int, img_w: int, img_h: int):
        """
        Save labels in YOLO format:
        class x_center y_center width height (normalized 0..1)
        """
        try:
            boxes = res0.boxes
            if boxes is None or len(boxes) == 0:
                return

            xyxy = boxes.xyxy.cpu().numpy()
            clss = boxes.cls.cpu().numpy().astype(int)

            label_dir = os.path.join(out_dir, "labels")
            ensure_dir(label_dir)
            label_path = os.path.join(label_dir, f"{source_type}_frame_{frame_index:06d}.txt")

            with open(label_path, "w", encoding="utf-8") as f:
                for (x1, y1, x2, y2), cid in zip(xyxy, clss):
                    x1 = max(0.0, min(float(x1), img_w - 1))
                    y1 = max(0.0, min(float(y1), img_h - 1))
                    x2 = max(0.0, min(float(x2), img_w - 1))
                    y2 = max(0.0, min(float(y2), img_h - 1))

                    bw = max(0.0, x2 - x1)
                    bh = max(0.0, y2 - y1)
                    xc = x1 + bw / 2.0
                    yc = y1 + bh / 2.0

                    # normalize
                    xc /= img_w
                    yc /= img_h
                    bw /= img_w
                    bh /= img_h

                    f.write(f"{int(cid)} {xc:.6f} {yc:.6f} {bw:.6f} {bh:.6f}\n")
        except Exception:
            pass

    # -------------------------
    # Close
    # -------------------------
    def on_close(self):
        self.stop_stream()
        self.destroy()


if __name__ == "__main__":
    # Better DPI scaling on Windows (optional)
    try:
        from ctypes import windll
        windll.shcore.SetProcessDpiAwareness(1)
    except Exception:
        pass

    app = YoloRunnerUI()
    app.mainloop()
