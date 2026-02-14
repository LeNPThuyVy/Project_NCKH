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

