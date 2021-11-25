/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System.Security.Cryptography;
using System.Text;

namespace Agrirouter.Common.Extensions
{
    public static class StringExtensions
    {
        public static string GetMd5Hash(this string value)
        {
            using (var md5 = MD5.Create())
                return GenerateMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        private static string GenerateMd5Hash(byte[] data)
        {
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}