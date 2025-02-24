using IOITCore.Models.ViewModels;

namespace IOITCore.Services.Common
{
    public class CheckRoleService
    {
        public static bool CheckRoleByCode(string access_key, string key, int type)
        {
            var check = false;

            var functionRole = access_key.Split('-');
            for (var i = 0; i < functionRole.Count(); i++)
            {
                var code = functionRole[i].Split(':')[0];
                var role = functionRole[i].Split(':')[1];
                if (code == key)
                {
                    check = role.Substring(type, 1) == "1" ? true : false;
                    break;
                }
            }

            return check;
        }

        public static string PlusActiveKey(string key1, string key2)
        {
            string str = "";
            char[] str1 = key1.ToCharArray();
            char[] str2 = key2.ToCharArray();
            for (int i = 0; i < str1.Length; i++)
            {
                int k = int.Parse(str1[i].ToString()) + int.Parse(str2[i].ToString());
                if (k > 1) k = 1;
                str += k;
            }
            return str;
        }

        //create menu
        public static List<MenuDTO> CreateMenu(List<MenuDTO> list, int k)
        {
            var listMenu = list.Where(e => e.MenuParent == k).ToList();
            if (listMenu.Count > 0)
            {
                List<MenuDTO> menus = new List<MenuDTO>();
                foreach (var item in listMenu)
                {
                    char[] str = item.ActiveKey.ToCharArray();
                    if (int.Parse(str[8].ToString()) == 1)
                    {
                        MenuDTO menu = new MenuDTO();
                        menu.MenuId = item.MenuId;
                        menu.Code = item.Code;
                        menu.Name = item.Name;
                        menu.Url = item.Url;
                        menu.Icon = item.Icon;
                        menu.IsParamRoute = item.IsParamRoute;
                        menu.MenuParent = item.MenuParent;
                        menu.ActiveKey = item.ActiveKey;
                        menu.listMenus = CreateMenu(list, item.MenuId);
                        menus.Add(menu);
                    }
                }
                return menus;
            }
            return new List<MenuDTO>();
        }



    }
}
