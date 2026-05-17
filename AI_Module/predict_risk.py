# -*- coding: utf-8 -*-
import mysql.connector
import pandas as pd
import joblib
import sys
from sklearn.ensemble import RandomForestClassifier
from sklearn.preprocessing import LabelEncoder
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report, confusion_matrix, roc_auc_score
from sqlalchemy import create_engine

# Đảm bảo hiển thị tiếng Việt trên Terminal và tránh lỗi font
sys.stdout.reconfigure(encoding='utf-8')

def run_ai_prediction_final():
    try:
        print("🔗 Đang kết nối tới Database ntu_learninganalytics...")
        # 1. KẾT NỐI DATABASE
        engine = create_engine("mysql+mysqlconnector://root:@localhost/ntu_learninganalytics")
        
        # 2. LẤY DỮ LIỆU CÓ NHÃN (Dữ liệu lịch sử để AI học)
        query_history = """
            SELECT ma_lhp, diem_chuyen_can, diem_giua_ky, trang_thai_thuc_te 
            FROM KetQuaHocTap 
            WHERE trang_thai_thuc_te IS NOT NULL
        """
        df_history = pd.read_sql(query_history, engine)
        
        if df_history.empty:
            print("❌ Không có dữ liệu lịch sử để huấn luyện mô hình!")
            return

        # Tiền xử lý dữ liệu: Chuyển mã môn học và nhãn Đậu/Rớt sang số
        le = LabelEncoder()
        df_history['ma_lhp_encoded'] = le.fit_transform(df_history['ma_lhp'])
        df_history['label'] = df_history['trang_thai_thuc_te'].apply(lambda x: 1 if x == 'Rớt' else 0)

        X = df_history[['ma_lhp_encoded', 'diem_chuyen_can', 'diem_giua_ky']]
        y = df_history['label']

        # --- GIAI ĐOẠN HỌC THUẬT: CHIA DỮ LIỆU TRAIN/TEST (80/20) ---
        # Giúp đánh giá mô hình khách quan, tránh học vẹt (Overfitting)
        X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42, stratify=y)

        # 3. HUẤN LUYỆN MÔ HÌNH RANDOM FOREST
        print("🚀 AI đang học quy luật từ 80% dữ liệu lịch sử...")
        model = RandomForestClassifier(n_estimators=100, random_state=42)
        model.fit(X_train, y_train)

        # --- XUẤT CÁC CHỈ SỐ ĐỂ ĐƯA VÀO BÁO CÁO ---
        y_pred = model.predict(X_test)
        y_prob = model.predict_proba(X_test)[:, 1]

        print("\n" + "="*50)
        print("📊 KẾT QUẢ ĐÁNH GIÁ MÔ HÌNH (Ghi vào báo cáo ngay)")
        print("="*50)
        print("1. Ma trận nhầm lẫn (Confusion Matrix):")
        print(confusion_matrix(y_test, y_pred))
        print("\n2. Báo cáo chi tiết (Precision, Recall, F1-score):")
        print(classification_report(y_test, y_pred))
        print(f"3. Chỉ số AUC (Khả năng phân loại): {roc_auc_score(y_test, y_prob):.4f}")
        print("="*50 + "\n")

        # 4. DỰ BÁO CHO TOÀN BỘ SINH VIÊN HIỆN TẠI (115k dòng)
        print("🔮 Đang tính toán xác suất rủi ro cho toàn bộ sinh viên...")
        query_all = "SELECT id_student, ma_lhp, diem_chuyen_can, diem_giua_ky FROM KetQuaHocTap"
        df_all = pd.read_sql(query_all, engine)
        
        # Xử lý mã lớp cho dữ liệu mới (gán -1 nếu là môn học mới hoàn toàn)
        df_all['ma_lhp_encoded'] = df_all['ma_lhp'].apply(lambda x: le.transform([x])[0] if x in le.classes_ else -1)
        
        X_predict = df_all[['ma_lhp_encoded', 'diem_chuyen_can', 'diem_giua_ky']]
        probs = model.predict_proba(X_predict)[:, 1]
        df_all['probability'] = probs

        # 5. CẬP NHẬT KẾT QUẢ VÀO DATABASE
        raw_conn = mysql.connector.connect(host="localhost", user="root", password="", database="ntu_learninganalytics")
        cursor = raw_conn.cursor()
        
        cursor.execute("SET FOREIGN_KEY_CHECKS = 0;")
        cursor.execute("TRUNCATE TABLE dubaoruiro;") # Làm sạch bảng để nạp kết quả mới
        
        insert_sql = """
            INSERT INTO dubaoruiro (id_student, ma_lhp, xac_suat_truot, nhan_du_bao, ly_do_chi_tiet, ngay_cap_nhat) 
            VALUES (%s, %s, %s, %s, %s, NOW())
        """
        
        batch_data = []
        for _, row in df_all.iterrows():
            p = float(row['probability'] * 100)
            cc = row['diem_chuyen_can']
            gk = row['diem_giua_ky']
            
            # --- LOGIC KẾT HỢP: AI + LUẬT NGHIỆP VỤ ---
            if cc < 5.0 or gk < 5.0:
                final_p = max(p, 80.0) # Ưu tiên cảnh báo rủi ro cao nếu điểm liệt
                label = "Nguy co"
                reason = f"Cảnh báo: Điểm thấp (CC: {cc}, GK: {gk}). Cần hỗ trợ thi cuối kỳ."
            else:
                final_p = p
                label = "Nguy co" if final_p >= 45 else "An toan"
                reason = f"Dựa trên CC: {cc}, GK: {gk}. Ngưỡng rủi ro AI: {final_p:.1f}%"
            
            batch_data.append((row['id_student'], row['ma_lhp'], final_p, label, reason))

        # Nạp dữ liệu theo đợt (Batch Processing) để tối ưu tốc độ cho 115k dòng
        print("💾 Đang đồng bộ kết quả dự báo sang Dashboard...")
        batch_size = 5000
        for i in range(0, len(batch_data), batch_size):
            cursor.executemany(insert_sql, batch_data[i:i+batch_size])
            raw_conn.commit()

        cursor.execute("SET FOREIGN_KEY_CHECKS = 1;")
        print(f"✅ THÀNH CÔNG! Đã cập nhật rủi ro cho {len(batch_data)} sinh viên.")

    except Exception as e:
        print(f"❌ Lỗi thực thi: {e}")
    finally:
        if 'raw_conn' in locals() and raw_conn.is_connected():
            cursor.close()
            raw_conn.close()

if __name__ == "__main__":
    run_ai_prediction_final()