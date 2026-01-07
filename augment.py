import cv2
import os
import numpy as np
import random
import shutil

INPUT_DIR = "dataset"
OUTPUT_DIR = "newdataset"

os.makedirs(OUTPUT_DIR, exist_ok=True)

def rotate_image_no_black_border(image, angle):
    h, w = image.shape[:2]
    center = (w // 2, h // 2)
    M = cv2.getRotationMatrix2D(center, angle, 1.0)

    rotated = cv2.warpAffine(
        image,
        M,
        (w, h),
        flags=cv2.INTER_LINEAR,
        borderMode=cv2.BORDER_REFLECT
    )
    return rotated

def augment_image(image):
    augmented = image.copy()

    angle = random.uniform(-5, 5)
    augmented = rotate_image_no_black_border(augmented, angle)

    alpha = random.uniform(0.9, 1.1)
    beta = random.randint(-15, 15)
    augmented = cv2.convertScaleAbs(augmented, alpha=alpha, beta=beta)

    return augmented

# copy ảnh gốc
for file_name in os.listdir(INPUT_DIR):
    src = os.path.join(INPUT_DIR, file_name)
    dst = os.path.join(OUTPUT_DIR, file_name)
    if os.path.isfile(src):
        shutil.copy(src, dst)

# tạo 3 ảnh mới / ảnh gốc
for file_name in os.listdir(INPUT_DIR):
    if not file_name.lower().endswith((".jpg", ".jpeg", ".png")):
        continue

    img_path = os.path.join(INPUT_DIR, file_name)
    image = cv2.imread(img_path)
    if image is None:
        continue

    name, ext = os.path.splitext(file_name)

    for i in range(3):
        aug_img = augment_image(image)
        new_name = f"{name}_aug{i+1}{ext}"
        cv2.imwrite(os.path.join(OUTPUT_DIR, new_name), aug_img)

print("Hoàn thành quá trình tạo tập dữ liệu mới (newdataset).")
