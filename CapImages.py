import cv2, os

video_name="Qui_Normal"
video_path = "Video\\"+video_name+".mp4"
out_dir = "Video"
os.makedirs(out_dir, exist_ok=True)
cap = cv2.VideoCapture(video_path)
fps = cap.get(cv2.CAP_PROP_FPS)

interval = 0.5  # đổi thành 1.0 nếu muốn 1 ảnh / giây
frame_interval = int(fps * interval)

count = 0
saved = 0

while True:
    ret, frame = cap.read()
    if not ret:
        break

    if count % frame_interval == 0:
        cv2.imwrite(f"{out_dir}/{video_name}_{saved:05d}.jpg", frame)
        saved += 1

    count += 1

cap.release()
print(f"Saved {saved} frames")