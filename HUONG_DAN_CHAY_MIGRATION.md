# Hướng dẫn chạy Migration để tạo bảng Position

## Vấn đề
Bảng `Positions` chưa được tạo trong database vì migration chưa được apply.

## Giải pháp

### Cách 1: Chạy migration bằng lệnh (Khuyến nghị)

1. **Mở Terminal/Command Prompt** tại thư mục project:
   ```
   cd c:\Users\RedNV\Desktop\Project\EmployeeManagement
   ```

2. **Chạy lệnh apply migration:**
   ```
   dotnet ef database update --project EmployeeManagement.Infrastructure --startup-project EmployeeManagement.Web
   ```

3. **Kiểm tra kết quả:**
   - Nếu thành công, bạn sẽ thấy thông báo migration đã được apply
   - Bảng `Positions` sẽ được tạo trong database PostgreSQL

### Cách 2: Migration tự động khi chạy ứng dụng

Nếu bạn đã cấu hình auto-migration trong `Program.cs` (như trong API project), migration sẽ tự động chạy khi ứng dụng khởi động.

### Cách 3: Chạy SQL trực tiếp (Nếu migration không chạy được)

Nếu gặp vấn đề với migration, bạn có thể chạy SQL trực tiếp trong PostgreSQL:

```sql
CREATE TABLE "Positions" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(500) NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL
);
```

## Kiểm tra sau khi chạy migration

1. **Kiểm tra trong database:**
   ```sql
   SELECT * FROM "Positions";
   ```

2. **Hoặc kiểm tra trong ứng dụng:**
   - Chạy ứng dụng
   - Truy cập `/Positions` 
   - Nếu không lỗi, bảng đã được tạo thành công

## Lưu ý

- Đảm bảo PostgreSQL đang chạy
- Đảm bảo connection string trong `appsettings.json` đúng
- Nếu có lỗi, kiểm tra log để xem chi tiết
