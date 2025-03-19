using System;
using System.ComponentModel.DataAnnotations;

namespace DALTW.Models
{
    public class TrafficLog
    {
        [Key]
        public int Id { get; set; } // ID tự tăng

        public string IpAddress { get; set; } // Địa chỉ IP người dùng

        public string Url { get; set; } // URL mà người dùng truy cập

        public DateTime AccessTime { get; set; } = DateTime.Now; // Thời gian truy cập
    }
}
