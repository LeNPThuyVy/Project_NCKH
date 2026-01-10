from pathlib import Path

def is_text_file_empty(txt_path: Path) -> bool:
    """
    Trả về True nếu file không có dữ liệu (rỗng hoặc chỉ toàn whitespace).
    """
    try:
        content = txt_path.read_text(encoding="utf-8", errors="ignore")
    except Exception:
        # Nếu đọc lỗi, coi như có dữ liệu để tránh xóa nhầm
        return False
    return len(content.strip()) == 0


def delete_empty_txt_and_matching_image(
    txt_dir: str,
    img_dir: str,
    img_exts=(".jpg", ".jpeg", ".png", ".webp"),
    recursive: bool = False,
    dry_run: bool = False,
):
    txt_dir = Path(txt_dir)
    img_dir = Path(img_dir)

    if not txt_dir.exists():
        raise FileNotFoundError(f"Không thấy thư mục txt: {txt_dir}")
    if not img_dir.exists():
        raise FileNotFoundError(f"Không thấy thư mục ảnh: {img_dir}")

    # Chọn cách duyệt file txt
    txt_files = txt_dir.rglob("*.txt") if recursive else txt_dir.glob("*.txt")

    deleted_txt = 0
    deleted_img = 0
    deleted_pairs = 0

    for txt_path in txt_files:
        if not txt_path.is_file():
            continue

        if is_text_file_empty(txt_path):
            stem = txt_path.stem  # tên file không có .txt

            # Xóa txt
            if dry_run:
                print(f"[DRY-RUN] Sẽ xóa TXT: {txt_path}")
            else:
                txt_path.unlink(missing_ok=True)
            deleted_txt += 1

            # Tìm và xóa ảnh cùng tên ở img_dir (ưu tiên các đuôi phổ biến)
            img_deleted_this = 0
            for ext in img_exts:
                img_path = img_dir / f"{stem}{ext}"
                if img_path.exists() and img_path.is_file():
                    if dry_run:
                        print(f"[DRY-RUN] Sẽ xóa IMG: {img_path}")
                    else:
                        img_path.unlink(missing_ok=True)
                    deleted_img += 1
                    img_deleted_this += 1

            deleted_pairs += 1
            if img_deleted_this == 0:
                print(f"⚠ Không tìm thấy ảnh cùng tên cho: {stem}")

    print("\n===== TỔNG KẾT =====")
    print(f"Số TXT rỗng đã xóa: {deleted_txt}")
    print(f"Số ảnh đã xóa:      {deleted_img}")
    print(f"Số trường hợp xử lý (txt rỗng): {deleted_pairs}")
    print("====================\n")


# ======================
# CÁCH DÙNG
# ======================
if __name__ == "__main__":
    TXT_FOLDER = r"D:\NCKH_AI\dataset\dataset_yolo\labels\train"     # thư mục chứa file .txt
    IMG_FOLDER = r"D:\NCKH_AI\dataset\dataset_yolo\images\train"         # thư mục chứa ảnh cùng tên

    delete_empty_txt_and_matching_image(
        txt_dir=TXT_FOLDER,
        img_dir=IMG_FOLDER,
        img_exts=(".jpg", ".jpeg", ".png"),
        recursive=False,   # True nếu muốn quét cả thư mục con
        dry_run=False      # True để chạy thử (không xóa thật)
    )
