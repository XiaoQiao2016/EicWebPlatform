﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lm.Eic.App.DomainModel.Bpm.Hrm.Archives;
using System;
using Lm.Eic.App.Business.Bmp.Hrm.Archives;
using System.Globalization;
using Lm.Eic.App.Business.BmpTests.Hrm.Archives;

namespace Lm.Eic.App.Business.Bmp.Hrm.Archives.Tests
{
    [TestClass()]
    public class ArchivesManagerTests
    {
        [TestMethod()]
        public void StoreTest()
        {
            string dt = "2011年12月27日--2031年12月27日";
            int pos = dt.LastIndexOf("--");
            if (pos > 0)
            {
                string et = dt.Substring(pos + 2, dt.Length - pos - 2);
                DateTime b = DateTime.Parse(et);
            }
            Assert.Fail();
        }
          [TestMethod()]
        public void testLeaveOffManager()
        {
            var model = new ArLeaveOfficeModel
            {
                ID="42092319811109247X",
                WorkerId="001359",
                WorkerName ="万晓桥",
                Department="Eic",
                LeaveDate=DateTime.Now,
                LeaveReason="漫漫流量监测",
                OpPerson ="万晓桥",
                Post ="操作工",
                Memo ="测试",
                OpSign ="add"

            };
         var Result=   ArchiveService.ArchivesManager.LeaveOffManager.StoreLeaveOffInfo(model);
        }

        public void testWorkerIdChange()
        {

            var model = new WorkerChangedModel {
                OldWorkerId = "881359",
                NewWorkerId ="001359",
                WorkerName = "万晓桥",
                OpSign = "add",
                OpPerson ="万晓桥"
                
              
                
            };
            var resulst = ArchiveService.ArchivesManager.WorkerIdChangeManager.StoreWorkerIdChangeInfo(model);
        }


        public void test()
        {
           
            var ddd = GetChineseDateTime(DateTime.Now.Date);
            var mm = ArchiveService.ArCalendarManger.GetDateDictionary(2017, 2);
            if (mm==null || mm.Count < 0)
            {
                Assert.Fail();
            }
          
        }


        ///<summary>
        /// 实例化一个 ChineseLunisolarCalendar
        ///</summary>
        private static ChineseLunisolarCalendar ChineseCalendar = new ChineseLunisolarCalendar();

        ///<summary>
        /// 十天干
        ///</summary>
        private static string[] tg = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

        ///<summary>
        /// 十二地支
        ///</summary>
        private static string[] dz = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        ///<summary>
        /// 十二生肖
        ///</summary>
        private static string[] sx = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

        ///<summary>
        /// 返回农历天干地支年
        ///</summary>
        ///<param name="year">农历年</param>
        ///<return s></return s>
        public static string GetLunisolarYear(int year)
        {
            if (year > 3)
            {
                int tgIndex = (year - 4) % 10;
                int dzIndex = (year - 4) % 12;

                return string.Concat(tg[tgIndex], dz[dzIndex], "[", sx[dzIndex], "]");
            }

            throw new ArgumentOutOfRangeException("无效的年份!");
        }

        ///<summary>
        /// 农历月
        ///</summary>

        ///<return s></return s>
        private static string[] months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二(腊)" };

        ///<summary>
        /// 农历日
        ///</summary>
        private static string[] days1 = { "初", "十", "廿", "三" };
        ///<summary>
        /// 农历日
        ///</summary>
        private static string[] days = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };


        ///<summary>
        /// 返回农历月
        ///</summary>
        ///<param name="month">月份</param>
        ///<return s></return s>
        public static string GetLunisolarMonth(int month)
        {
            if (month < 13 && month > 0)
            {
                return months[month - 1];
            }

            throw new ArgumentOutOfRangeException("无效的月份!");
        }

        ///<summary>
        /// 返回农历日
        ///</summary>
        ///<param name="day">天</param>
        ///<return s></return s>
        public static string GetLunisolarDay(int day)
        {
            if (day > 0 && day < 32)
            {
                if (day != 20 && day != 30)
                {
                    return string.Concat(days1[(day - 1) / 10], days[(day - 1) % 10]);
                }
                else
                {
                    return string.Concat(days[(day - 1) / 10], days1[1]);
                }
            }

            throw new ArgumentOutOfRangeException("无效的日!");
        }



        ///<summary>
        /// 根据公历获取农历日期
        ///</summary>
        ///<param name="datetime">公历日期</param>
        ///<return s></return s>
        public static string GetChineseDateTime(DateTime datetime)
        {
            int year = ChineseCalendar.GetYear(datetime);
            int month = ChineseCalendar.GetMonth(datetime);
            int day = ChineseCalendar.GetDayOfMonth(datetime);
            //获取闰月， 0 则表示没有闰月
            int leapMonth = ChineseCalendar.GetLeapMonth(year);

            bool isleap = false;

            if (leapMonth > 0)
            {
                if (leapMonth == month)
                {
                    //闰月
                    isleap = true;
                    month--;
                }
                else if (month > leapMonth)
                {
                    month--;
                }
            }

            return string.Concat(GetLunisolarYear(year), "年", isleap ? "闰" : string.Empty, GetLunisolarMonth(month), "月", GetLunisolarDay(day));
        }
    }
 }
