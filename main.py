from check_model_ui import YoloRunnerUI;

if __name__ == "__main__":
    # Better DPI scaling on Windows (optional)
    try:
        from ctypes import windll
        windll.shcore.SetProcessDpiAwareness(1)
    except Exception:
        pass

    app = YoloRunnerUI()
    app.mainloop()
