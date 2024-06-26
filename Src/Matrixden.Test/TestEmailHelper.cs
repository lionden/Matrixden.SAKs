﻿using Matrixden.SwissArmyKnives;
using Matrixden.SwissArmyKnives.Models;
using Matrixden.Utils.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace Matrixden.Test
{
    [TestClass]
    public class TestEmailHelper
    {
        [TestMethod]
        public void TestValidateMethod()
        {
            HandleEmailAddr(a =>
            {
                return EmailHelper.Validate(a);
            });
        }

        [TestMethod]
        public void TestSend()
        {
            HandleEmailAddr(a =>
            {
                return new EmailHelper(a,"Matrix Bot").Send();
            });
        }

        private void HandleEmailAddr(Func<string, OperationResult> func)
        {
            //文件路径
            string filePath = @"D:\Users\Lionden\Downloads\Emails.txt";
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine("文件不存在");
                    return;
                }

                foreach (var l in File.ReadLines(filePath))
                {
                    if (l.IsNullOrEmptyOrWhiteSpace())
                        continue;

                    var or = func(l);
                    Debug.WriteLine($"{l}: {(or.Result ? "OK" : or.Message)}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
