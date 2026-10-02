# Hướng dẫn chi tiết triển khai PRN232 - Assignment 01: FUNewsManagementSystem

Tài liệu này cung cấp lộ trình từng bước để hoàn thành bài Assignment 01 theo đúng yêu cầu kiến trúc 3-layer, Repository pattern, Singleton DAO và OData trên nền tảng .NET 8.

---

## Phần 1: Khởi tạo Project và Cấu trúc Solution

1. **Tạo Solution mới:**
   * Mở Visual Studio, tạo **Blank Solution** (hoặc tạo cùng lúc với project đầu tiên) với tên: `No_Fullname_Assignment01`.
2. **Tạo Project BackEnd (Web API):**
   * Add New Project -> `ASP.NET Core Web API` -> Tên: `No_Fullname_Assignment01_BackEnd`.
   * Chọn framework **.NET 8.0**, bỏ chọn *Use controllers (uncheck to use minimal APIs)* (nghĩa là chúng ta **sử dụng** Controllers).
3. **Tạo Project FrontEnd (Web App):**
   * Add New Project -> `ASP.NET Core Web App (Model-View-Controller)` hoặc `Razor Pages` -> Tên: `No_Fullname_Assignment01_FrontEnd`.
   * Chọn framework **.NET 8.0**.
4. **Cấu hình chạy song song:**
   * Click chuột phải vào Solution -> `Properties` -> `Startup Project` -> Chọn `Multiple startup projects`.
   * Chuyển Action của cả BackEnd và FrontEnd thành **Start**.

---

## Phần 2: Cấu hình Database & Entity Framework Core

1. **Khởi tạo Database:**
   * Chạy script `FUNewsManagement.sql` trong SQL Server Management Studio (SSMS) để tạo database và dữ liệu mẫu.
2. **Cài đặt NuGet Packages (Tại project BackEnd):**
   * Mở *Package Manager Console*, trỏ Default project vào BackEnd.
   * Chạy các lệnh sau (chú ý dùng phiên bản 8.0.x để tương thích .NET 8):
     ```powershell
     Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.10
     Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.10
     Install-Package Microsoft.AspNetCore.OData
     ```
3. **Cấu hình appsettings.json:**
   * Bổ sung chuỗi kết nối và tài khoản Admin mặc định:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "AdminAccount": {
       "Email": "admin@FUNewsManagementSystem.org",
       "Password": "@@abc123@@"
     }
     ```
4. **Scaffold Database (Sinh Model):**
   * Chạy lệnh sau trong *Package Manager Console*:
     ```powershell
     Scaffold-DbContext Name=ConnectionStrings:DefaultConnection Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
     ```

---

## Phần 3: Xây dựng BackEnd (Data Access & API Layer)

Yêu cầu cốt lõi: **Không được gọi trực tiếp DbContext từ Controller.**

### 3.1. Data Access Objects (DAO - Singleton Pattern)
* Tạo thư mục `DataAccessObjects`.
* Xây dựng các class DAO cho từng thực thể (ví dụ: `SystemAccountDAO`, `CategoryDAO`, `NewsArticleDAO`).
* Triển khai Singleton cho mỗi class để quản lý instance duy nhất. Đảm bảo mỗi phương thức CRUD khởi tạo một `using var context = new FunewsManagementContext();` riêng biệt biệt để tránh lỗi tracking.

### 3.2. Repository Layer
* Tạo thư mục `Repositories`.
* Tạo các Interface (ví dụ: `ISystemAccountRepository`) và Class triển khai gọi tới các DAO tương ứng (ví dụ: `SystemAccountDAO.Instance.Get()`).

### 3.3. Cấu hình OData & Dependency Injection (Program.cs)
* Đăng ký Repository:
  ```csharp
  builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
  // Thêm các repository khác...
  ```
* Cấu hình OData:
  ```csharp
  builder.Services.AddControllers()
      .AddOData(options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100));
  ```

### 3.4. Xây dựng Controllers
* **AuthenticationController**: 
  * POST request: Kiểm tra thông tin đăng nhập. Ưu tiên check cấu hình Admin trong `appsettings.json` trước, nếu không khớp mới gọi Repository để check trong CSDL.
* **Các OData Controllers** (`SystemAccountsController`, `CategoriesController`, `NewsArticlesController`):
  * Cung cấp các endpoint GET, POST, PUT, DELETE.
  * Thêm attribute `[EnableQuery]` cho các hàm GET để hỗ trợ Filter/Search/Sort.
  * **Xử lý ràng buộc xóa (Delete):**
    * *Account*: Không cho xóa nếu `CreatedByID` đã tồn tại trong `NewsArticle`.
    * *Category*: Không cho xóa nếu `CategoryID` đang được dùng trong `NewsArticle`.

---

## Phần 4: Xây dựng FrontEnd (Client Application)

### 4.1. Thiết lập HttpClient & Routing
* Cấu hình `HttpClient` trong `Program.cs` để gọi tới Base Address của BackEnd API.
* Thiết kế routing phân chia rõ ràng các khu vực: `/Admin`, `/Staff`, và trang public.

### 4.2. Xử lý Đăng nhập & Session
* Xây dựng form Login. Khi gọi API Authentication thành công, lưu `AccountID` và `Role` vào **Session** (hoặc Cookie).
* Sử dụng custom filters hoặc middlewares để kiểm tra quyền truy cập (Authorization) dựa trên Role ở mỗi trang.

### 4.3. Nghiệp vụ theo Role
* **Khách (Không đăng nhập):**
  * Chỉ được xem danh sách bài viết có trạng thái Active (`NewsStatus == 1`).
* **Admin (Role = 0/Admin):**
  * Quản lý Account (CRUD).
  * Chức năng Report: Dùng OData filter `CreatedDate` (từ StartDate đến EndDate) và OrderByDescending.
* **Staff/Lecturer (Role = 1 & 2):**
  * Quản lý Category (CRUD).
  * Quản lý News Article & Tags (CRUD, liên kết bảng nhiều-nhiều).
  * Xem/Cập nhật thông tin Profile cá nhân.
  * Lịch sử bài viết (Hiển thị list bài viết có `CreatedByID == Current User ID`).

### 4.4. Yêu cầu UI/UX bắt buộc
* **Create / Update:** Phải sử dụng **Popup Dialog (Modal)**. Có thể dùng Bootstrap Modal kết hợp với AJAX/Fetch API hoặc jQuery để gửi dữ liệu form mà không cần load lại toàn bộ trang.
* **Delete:** Luôn có hộp thoại **Confirmation** ("Bạn có chắc chắn muốn xóa không?") trước khi thực thi lệnh gọi API DELETE.
* **Validation:** Bắt buộc có Data Validation ở phía FrontEnd (HTML5, jQuery Validation) kết hợp hiển thị lỗi từ API trả về nếu có (ví dụ nhập thiếu title, sai định dạng email).

---

## Phần 5: Kiểm tra và Bàn giao (Checklist)
- [ ] Tên Solution và 2 Projects đúng chuẩn `No_Fullname_...`
- [ ] Cấu trúc 3-layer (Controller -> Repository -> DAO) được đảm bảo ở BackEnd.
- [ ] Mẫu thiết kế Singleton áp dụng cho toàn bộ DAO.
- [ ] Không hardcode ConnectionString và Admin credential trong mã C#.
- [ ] FrontEnd chỉ giao tiếp với Database thông qua HttpClient gọi tới API.
- [ ] Host source code lên hệ thống Git theo yêu cầu giảng viên.