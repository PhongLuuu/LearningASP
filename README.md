Endpoint Filter: kiểm tra các điều kiện như xác thực, quyền truy cập, hoặc kiểm tra dữ liệu đầu vào trước khi yêu cầu được xử lý bởi các middleware hoặc controller.
Middleware: xác thực người dùng, kiểm tra dữ liệu đầu vào, xử lý session....
  Middleware nhận các request, thi hành các mệnh lệnh tương ứng trên request đó, sau khi hoàn thành nó response (trả về) hoặc chuyển kết quả ủy thác cho một Middleware khác trong hàng đợi.
  Pipeline: ![image](https://github.com/PhongLuuu/LearningASP/assets/158790293/39540157-2852-42e5-a96c-b412ddd71e9a)
            ![image](https://github.com/PhongLuuu/LearningASP/assets/158790293/b7735c58-cd17-4756-8919-eea908617931)
            ![image](https://github.com/PhongLuuu/LearningASP/assets/158790293/a2cfa705-a547-4d39-ad04-bbcb1d54ed5a)

Read more about Middleware: https://viblo.asia/p/tong-quan-ve-middleware-ung-dung-middleware-trong-aspnet-core-RnB5pOz2lPG
