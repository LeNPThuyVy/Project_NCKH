from PIL import Image
import os
import cv2, os
import numpy as np

#Chuẩn hóa kênh màu và tên ảnh (chuyển về tên có đuôi .jpg) 
src_dir = "Dataset/Images"
out_dir = "Dataset/Images_clean"
os.makedirs(out_dir, exist_ok=True)

for f in os.listdir(src_dir):
    if f.lower().endswith((".png", ".jpeg", ".jpg")):
        img = Image.open(os.path.join(src_dir, f)).convert("RGB")
        img.save(os.path.join(out_dir, f.split('.')[0] + ".jpg"), quality=95)

#Chuẩn hóa size ảnh về  640x640 với padding giữ tỉ lệ ảnh
def letterbox(img, new_size=640):
    h, w = img.shape[:2]
    scale = new_size / max(h, w)
    nh, nw = int(h * scale), int(w * scale)
    img_resized = cv2.resize(img, (nw, nh))
    canvas = np.full((new_size, new_size, 3), 128, dtype=np.uint8)
    canvas[:nh, :nw] = img_resized
    return canvas

src_dir = "Dataset/Images_clean"
out_dir = "Dataset/Images_clean"
os.makedirs(out_dir, exist_ok=True)

for f in os.listdir(src_dir):
    img = cv2.imread(os.path.join(src_dir, f))
    if img is None:
        continue
    img = letterbox(img, 640)
    cv2.imwrite(os.path.join(out_dir, f), img)

#Loại bỏ ảnh có kích thước nhỏ hơn 100x100
min_size = 100

for f in os.listdir("Dataset/Images_clean"):
    img = cv2.imread(os.path.join("Images_clean", f))
    if img is None or img.shape[0] < min_size or img.shape[1] < min_size:
        os.remove(os.path.join("Dataset/Images_clean", f))

