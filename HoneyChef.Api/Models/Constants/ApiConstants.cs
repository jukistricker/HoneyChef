using System;
using System.Collections.Generic;
using System.Text;

namespace IOITCore.Constants
{
    public class ApiConstants
    {
        public static class StatusCode
        {
            public const int Success200 = 200;
            public const int Success201 = 201;
            public const int Valid210 = 210;
            public const int Valid211 = 211;
            public const int Valid213 = 213;
            public const int NoPermision = 222;
            public const int Error400 = 400;
            public const int Error401 = 401;
            public const int Error404 = 404;
            public const int Error422 = 422;
            public const int Error500 = 500;
            public const int Error600 = 600;
        }

        public static class MessageWarning
        {
            public const string WARNING_2111_EXIT_TOWER = "Tòa nhà không tồn tại!";
            public const string WARNING_2112_EXIT_PROJECT = "Khu đô thị không tồn tại!";
            public const string WARNING_2113_EXIT_FLOOR = "Tầng không tồn tại!";
            public const string WARNING_2114_EXIT_ZONE = "Vị trí không tồn tại!";
            public const string WARNING_2115_EXIT_COUNTRY = "Quốc gia không tồn tại!";
            public const string WARNING_2116_EXIT_PROVINCE = "Tỉnh TP không tồn tại!";
            public const string WARNING_2117_EXIT_DISTRICT = "Quận huyện không tồn tại!";
            public const string WARNING_2118_EXIT_WARD = "Phường xã không tồn tại!";
            public const string WARNING_2119_EXIT_APARTMENT = "Cư dân không tồn tại!";
            public const string WARNING_21110_EXIT_DEPARTMENT = "Phòng ban không tồn tại!";
            public const string WARNING_21111_EXIT_HOTLINE = "Hotline không tồn tại!";
            public const string WARNING_21112_EXIT_NEW = "Tin tức không tồn tại!";

            public const string WARNING_21115_DUPLICATE_CODE = "Trường mã đã tồn tại!";
            public const string WARNING_21113_EXIT_CODE = "Trường mã không được để trống!";
            public const string WARNING_21114_EXIT_NAME = "Trường tên không được để trống!";
            public const string WARNING_21116_EXIT_USERNAME = "Trường tài khoản không được để trống!";
            public const string WARNING_21117_EXIT_PASSWORD = "Trường mật khẩu không được để trống!";
            public const string WARNING_2122_DUPLICATE_USERNAME = "Tài khoản đã tồn tại!";


        }    
        public static class MessageResource
        {
            #region common
            public const string ACCTION_SUCCESS = "Thành công!";
            public const string EXCEPTION_UNKNOWN = "Lỗi ngoại lệ chưa xác định!";
            public const string ADD_SUCCESS = "Thêm mới thành công!";
            public const string UPDATE_SUCCESS = "Sửa thông tin thành công!";
            public const string UNAUTHORIZE = "Lỗi xác thực!";
            public const string DELETE_SUCCESS = "Xóa thành công!";

            public static string MISS_DATA_MESSAGE = "Thông tin không đủ!";  //error_code = 210
            public static string BAD_REQUEST_MESSAGE = "Lỗi sai dữ liệu!";  //error_code = 400
            public static string NOT_FOUND_VIEW_MESSAGE = "Không tìm thấy mục cần xem!"; //error_code = 404
            public static string NOT_FOUND_UPDATE_MESSAGE = "Không tìm thấy mục cần sửa!"; //error_code = 404
            public static string NOT_FOUND_DELETE_MESSAGE = "Không tìm thấy mục cần xóa!"; //error_code = 404
            public static string ERROR_EXIST_MESSAGE = "Mục này đã tồn tại!";   //error_code = 211

            public static string ERROR_400_MESSAGE = "Có lỗi xảy ra. Xin vui lòng thử lại sau!";    //error_code = 400
            public static string ERROR_500_MESSAGE = "Hệ thống xảy ra lỗi. Xin vui lòng thử lại sau!";

            public const string NOT_FOUND = "Không tìm thấy!";
            public const string INVALID = "Không hợp lệ!";




            #endregion

            #region check role
            public const string NOPERMISION_VIEW_MESSAGE = "Bạn không có quyền xem dữ liệu tới mục này!";    //error_code = 222
            public const string NOPERMISION_UPDATE_MESSAGE = "Bạn không có quyền cập nhật mục này!";   //error_code = 222
            public const string NOPERMISION_CREATE_MESSAGE = "Bạn không có quyền thêm mới mục này!";   //error_code = 222
            public const string NOPERMISION_DELETE_MESSAGE = "Bạn không có quyền xoá mục này!";   //error_code = 222
            public const string NOPERMISION_ACCEPT_MESSAGE = "Bạn không có quyền duyệt đơn đăng ký!";
            public const string NOPERMISION_ACTION_MESSAGE = "Bạn không có quyền thực hiện thao tác này!";
            #endregion

            #region "user"
            public const string USER_NOT_FOUND = "Người dùng không tồn tại!";
            #endregion
        }

        public static class ErrorCode
        {
            #region common
            public const string ERROR_SERVER = "ERROR_SERVER";
            public const string ERROR_AUTHORIZED = "ERROR_AUTHORIZED";
            public const string ERROR_VALIDATION = "ERROR_VALIDATION";
            public const string ERROR_BADREQUEST = "ERROR_BADREQUEST";
            public const string ERROR_BADREQUEST_SAP = "ERROR_BADREQUEST_SAP";
            public const string ERROR_NOTFOUND = "ERROR_NOTFOUND";
            public const string ERROR_UNKNOWN = "ERROR_UNKNOWN";
            public const string ERROR_UNPERMISSION = "ERROR_UNPERMISSION";
            public const string ERROR_CONNECTION = "ERROR_CONNECTION";
            public const string ERROR_TIMEOUT = "ERROR_TIMEOUT";
            public const string ERROR_PERMISSSION = "ERROR_PERMISSSION";
            #endregion

            #region user
            public const string USERNAME_INVALID = "USERNAME_INVALID";
            public const string USER_LOCKED = "USER_LOCKED";
            public const string USER_PASSWORD_INCORRECT = "USER_PASSWORD_INCORRECT";
            #endregion


        }
    }
}
