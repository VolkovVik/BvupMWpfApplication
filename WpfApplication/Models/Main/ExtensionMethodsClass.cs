using System;
using System.Collections.Generic;
using System.Text;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// Класс методов расширения
    /// </summary>
    internal static class ExtensionMethodsClass {
        /// <summary>
        /// Подпрограмма выдачи сообщения об ошибке
        /// </summary>
        private static readonly Action< Exception, string > ShowErrorMessage =
            App.MyWindows.ShowFormErrorCommand.Execute;

        /// <summary>
        /// Подпрограма конвертации строки в число
        /// </summary>
        /// <param name="str"></param>
        /// <param name="frbase"></param>
        /// <returns></returns>
        public static int? ToIntExt( this string str, int frbase = 10 ) {
            if ( string.IsNullOrWhiteSpace( str ) ) return null;
            int? result = null;
            try {
                result = Convert.ToInt32( str, frbase );
            }
            catch ( FormatException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( OverflowException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            return result;
        }

        /// <summary>
        /// Подпрограма конвертации строки в число
        /// </summary>
        /// <param name="str"></param>
        /// <param name="frbase"></param>
        /// <returns></returns>
        public static uint? ToUintExt( this string str, int frbase = 10 ) {
            if ( string.IsNullOrWhiteSpace( str ) ) return null;
            uint? result = null;
            try {
                result = Convert.ToUInt32( str, frbase );
            }
            catch ( FormatException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( OverflowException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static uint? ToUintWithoutBaseExt( this string str ) {
            // Проверка получения данных
            if ( string.IsNullOrWhiteSpace( str ) || str.Length < 3 ) {
                return null;
            }
            // Анализ строки
            var i = str.IndexOf( "0x", StringComparison.OrdinalIgnoreCase );
            return i == 0 && str.Length > 2 ? str.ToUintExt( 16 ) : str.ToUintExt();
        }

        /// <summary>
        /// Подпрограма конвертации строки в число
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double? ToDoubleExt( this string str ) {
            if ( string.IsNullOrWhiteSpace( str ) ) return null;
            double? result = null;
            try {
                result = Convert.ToDouble( str.Replace( ".", "," ) );
            }
            catch ( FormatException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( OverflowException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации строки в число" );
            }
            return result;
        }

        /// <summary>
        /// Подпрограмма преобразования байтового массива с строку
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="codepage"></param>
        /// <returns></returns>
        public static string ToStringExt( this byte[] data, int index, int count, int codepage = 1251 ) {
            var result = string.Empty;
            if ( index + count > data.Length || count == 0 ) return result;
            try {
                // Gets the encoding for the specified code page.
                // Decode bytes back to string.
                result = Encoding.GetEncoding( codepage ).GetString( data, index, count );
            }
            catch ( DecoderFallbackException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации байтового массива в строку" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации байтового массива в строку" );
            }
            catch ( ArgumentNullException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации байтового массива в строку" );
            }
            catch ( ArgumentException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации байтового массива в строку" );
            }
            catch ( NotSupportedException exc ) {
                ShowErrorMessage( exc, "Ошибка конвертации байтового массива в строку" );
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToRigthExt( this string sourse, int? length = null ) {
            var sb = new StringBuilder();
            try {
                if ( length == null || length < 0 ) return sourse;
                for ( var i = 0; i < length; i++ ) {
                    sb.Append( " " );
                }
                sb.Append( sourse );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка сдвига строки" );
            }
            catch ( OutOfMemoryException exc ) {
                ShowErrorMessage( exc, "Ошибка сдвига строки" );
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToLengthExt( this string sourse, string str, int? length = null ) {
            var sb = new StringBuilder();
            try {
                if ( length == null || length < 0 || length - str.Length - sourse.Length < 0 ) return sourse;
                sb.Append( sourse );
                var count = length - str.Length - sourse.Length;
                for ( var i = 0; i < count; i++ ) {
                    sb.Append( " " );
                }
                sb.Append( str );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка сдвига строки" );
            }
            catch ( OutOfMemoryException exc ) {
                ShowErrorMessage( exc, "Ошибка сдвига строки" );
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string ToSubstringExt( this string source, string begin, string end = " " ) {
            string value = null;
            try {
                // Поиск искомой строки за которой следуют данные
                var i = source.IndexOf( begin, StringComparison.OrdinalIgnoreCase );
                if ( i == -1 ) return null;
                // Начало строки
                i += begin.Length;
                // Конец строки
                var j = source.IndexOf( end, i, StringComparison.OrdinalIgnoreCase );
                // Получение выбранной части строки
                value = source.Substring( i, j == -1 ? source.Length - i : j - i );
            }
            catch ( ArgumentNullException exc ) {
                ShowErrorMessage( exc, "Ошибка поиска заданного значения" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка поиска заданного значения" );
            }
            catch ( ArgumentException exc ) {
                ShowErrorMessage( exc, "Ошибка поиска заданного значения" );
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ToFindStringExt( this string source, string str ) {
            try {
                return source.IndexOf( str, StringComparison.OrdinalIgnoreCase ) != -1;
            }
            catch ( ArgumentNullException exc ) {
                ShowErrorMessage( exc, "Ошибка поиска заданного значения" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                ShowErrorMessage( exc, "Ошибка поиска заданного значения" );
            }
            catch ( ArgumentException exc ) {
                ShowErrorMessage( exc, "Ошибка поиска заданного значения" );
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int ToArrayExt( this List< byte > bytes, byte[] data, int offset, int count ) {
            if ( bytes  == null || bytes.Count == 0 || bytes.Count < count          ||
                 data   == null || data.Length == 0 || data.Length < offset + count ||
                 offset < 0     || count       == 0 ) return 0;
            // Цикл преобразования
            for ( var i = 0; i < bytes.Count; i++ ) {
                data[ offset + i ] = bytes[ i ];
            }
            return bytes.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int ToArrayExt( this List< byte > bytes, ushort[] data, int offset, int count,
            Rs232ProtocolClass.TypeCpu type ) {
            if ( bytes  == null || bytes.Count == 0 || bytes.Count < count          ||
                 data   == null || data.Length == 0 || data.Length < offset + count ||
                 offset < 0     || count       == 0 ) return 0;
            // Цикл преобразования
            for ( var i = 0; i < bytes.Count; i += 2 ) {
                // Различный порядок передачи байт big-endian и little-endian
                if ( type == Rs232ProtocolClass.TypeCpu.BigEndian ) {
                    data[ offset + ( i >> 1 ) ] = ( ushort ) ( ( bytes[ i ] << 8 ) | bytes[ i + 1 ] );
                } else {
                    data[ offset + ( i >> 1 ) ] = ( ushort ) ( ( bytes[ i + 1 ] << 8 ) | bytes[ i ] );
                }
            }
            return bytes.Count >> 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int ToArrayExt( this List< byte > bytes, uint[] data, int offset, int count,
            Rs232ProtocolClass.TypeCpu type ) {
            if ( bytes  == null || bytes.Count == 0 || bytes.Count < count          ||
                 data   == null || data.Length == 0 || data.Length < offset + count ||
                 offset < 0     || count       == 0 ) return 0;
            // Цикл преобразования
            for ( var i = 0; i < bytes.Count; i += 4 ) {
                if ( type == Rs232ProtocolClass.TypeCpu.BigEndian ) {
                    // Различный порядок передачи байт big-endian и little-endian
                    data[ offset + ( i >> 2 ) ] = ( data[ i + 0 ] << 24 ) | ( data[ i + 1 ] << 16 ) |
                                                  ( data[ i + 2 ] << 8 )  | ( data[ i + 3 ] << 0 );
                } else {
                    data[ offset + ( i >> 2 ) ] = ( data[ i + 3 ] << 24 ) | ( data[ i + 2 ] << 16 ) |
                                                  ( data[ i + 1 ] << 8 )  | ( data[ i + 0 ] << 0 );
                }
            }
            return bytes.Count >> 2;
        }
    }
}