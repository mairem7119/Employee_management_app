# Hướng dẫn sắp xếp – Danh sách chức vụ (Positions/Index)

## Cách hoạt động

1. **Controller** (`PositionsController.Index`)
   - Nhận tham số: `sortBy` (cột sắp xếp), `sortOrder` (asc/desc).
   - Mặc định: `sortBy = "Name"`, `sortOrder = "asc"`.
   - Gửi qua ViewBag: `ViewBag.SortBy`, `ViewBag.SortOrder` để View tạo link và hiển thị mũi tên.

2. **Service** (`IPositionService` / `PositionService`)
   - Overload: `GetAllPositionsAsync(string? sortBy, string? sortOrder)`.
   - Cột hợp lệ: `Id`, `Name`, `CreatedAt`, `UpdatedAt`.
   - Sắp xếp trong memory sau khi lấy danh sách từ repository (không đổi repository).

3. **View** (`Index.cshtml`)
   - Đầu cột có sắp xếp là link: `asp-action="Index" asp-route-sortBy="..." asp-route-sortOrder="..."`.
   - Click cùng cột: đổi chiều (asc ↔ desc).
   - Hiển thị ↑ (asc) hoặc ↓ (desc) bên cạnh cột đang sắp xếp.

## Thêm cột sắp xếp mới

- Trong **PositionService.GetAllPositionsAsync(sortBy, sortOrder)** thêm nhánh trong `switch (by)` cho tên thuộc tính mới.
- Trong **Index.cshtml** thêm `<a asp-action="Index" asp-route-sortBy="TênThuộcTính" ...>` cho header cột tương ứng.

## Áp dụng cho Employee hoặc Department

- Controller: thêm `sortBy`, `sortOrder` vào `Index`, gán ViewBag.
- Service/Repository: thêm method (hoặc overload) nhận `sortBy`/`sortOrder`, áp dụng `OrderBy`/`OrderByDescending` theo từng cột.
- View: header cột dùng link với `asp-route-sortBy` và `asp-route-sortOrder`, dùng helper tương tự `NextSortOrder` và `SortIcon`.
