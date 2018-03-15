using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace baidumap.DAL
{
    class User
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ErrorTimes { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime? LoginTime { get; set; } //LoginTime有可能为空值，所以定义为 DateTime? 类型
    }
}
