import os

def fix_numbers_in_folder(folder_path, target_number):
    for filename in os.listdir(folder_path):
        if not filename.endswith(".txt"):
            continue

        file_path = os.path.join(folder_path, filename)

        with open(file_path, "r", encoding="utf-8") as f:
            lines = f.readlines()

        new_lines = []

        for line in lines:
            line = line.strip()

            if not line:
                new_lines.append("\n")
                continue

            parts = line.split(maxsplit=1)
            content = parts[1] if len(parts) > 1 else ""

            new_lines.append(f"{target_number} {content}\n")

        # Ghi đè lại file
        with open(file_path, "w", encoding="utf-8") as f:
            f.writelines(new_lines)
        
        print(f"✔ Đã xử lý: {filename}")


# ======================
# CÁCH DÙNG
# ======================
folder_path = "D:\\NCKH_AI\\Test\\Check_class"   # đường dẫn tới folder chứa file txt
target_number = 1          # số cần xét

fix_numbers_in_folder(folder_path, target_number)
