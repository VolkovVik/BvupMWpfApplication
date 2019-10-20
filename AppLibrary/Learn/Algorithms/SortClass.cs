using System.Collections.Generic;
using System.Linq;

namespace AppLibrary.Learn.Algorithms {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://habr.com/ru/post/335920/
    /// https://programm.top/c-sharp/algorithm/array-sort/
    /// 
    /// Пузырьковая сортировка       https://habr.com/ru/post/204600/
    /// Сортировка вставками         https://habr.com/ru/post/204968/    
    ///                              https://habr.com/ru/post/415935/
    /// Сортировки выбором           https://habr.com/ru/post/422085/
    /// Сортировка слиянием          https://habr.com/ru/company/edison/blog/431964/
    /// Сортировки обменами          https://habr.com/ru/users/valemak/posts/
    /// 
    /// Сортировка вставками
    /// Библиотечная сортировка      https://habr.com/ru/post/416653/
    /// Пасьянсная сортировка        https://habr.com/ru/company/edison/blog/431094/
    /// Сортировка «Ханойская башня» https://habr.com/ru/company/edison/blog/431694/
    ///
    /// Пирамидальная сортировка     https://habr.com/ru/users/valemak/posts/page2/
    /// </remarks>
    internal static class SortClass {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static string GetString( IEnumerable< int > items ) {
            return items.Aggregate( string.Empty, ( current, item ) => current + item.ToString( "D3" ) + "  " );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private static void Swap( IList< int > items, int left, int right ) {
            if ( items == null || items.Count == 0 || left == right ) return;
            var temp = items[ left ];
            items[ left ]  = items[ right ];
            items[ right ] = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        private static void Flip( IList< int > items, int begin, int end ) {
            while ( begin < end ) {
                var swap = items[ begin ];
                items[ begin ] = items[ end ];
                items[ end ]   = swap;
                begin++;
                end--;
            }
        }

#region Сортировка пузырьком

        /// <summary>
        /// Сортировка пузырьком
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void BubbleSort( int[] items ) {
            ////App.MyWindows.TextLine = $"Сортировка пузырьком\r\n{GetString( items )}\r\n";
            bool swapped;
            do {
                swapped = false;
                for ( var i = 0; i < items.Length - 1; i++ ) {
                    if ( items[ i ] <= items[ i + 1 ] ) continue;
                    Swap( items, i, i + 1 );
                    swapped = true;
                }
                ////App.MyWindows.TextLine += GetString( items );
            } while ( swapped );
        }

        /// <summary>
        /// Шейкерная сортировка 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void ShakerSort( int[] items ) {
            //App.MyWindows.TextLine = $"Шейкерная сортировка\r\n{GetString( items )}\r\n";

            var  begin = 0;
            var  end   =  items.Length - 1;
            bool swapped;
            do {
                // Движение от большего к меньшему
                swapped = false;
                for ( var i = begin; i < end; i++ ) {
                    if ( items[ i ] <= items[ i + 1 ] ) continue;
                    Swap( items, i, i + 1 );
                    swapped = true;
                }
                end--;

                //App.MyWindows.TextLine += GetString( items );
                if ( !swapped ) break;

                // Движение от меньшего к большего к меньшему
                swapped = false;
                for ( var i = end; i > begin; i-- ) {
                    if ( items[ i ] >= items[ i - 1 ] ) continue;
                    Swap( items, i, i - 1 );
                    swapped = true;
                }
                begin++;

                //App.MyWindows.TextLine += GetString( items );
            } while ( swapped );
        }

        /// <summary>
        /// Сортировка расческой
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void CombSort( int[] items ) {
            //App.MyWindows.TextLine = $"Сортировка расческой\r\n{GetString( items )}\r\n";

            // Фактор уменьшения
            const double k    = 1.247330950103979;
            var          step = items.Length - 1;
            // Причесываем расческой
            while ( step >= 1 ) {
                for ( var i = 0; i < items.Length - step; i++ ) {
                    if ( items[ i ] > items[ i + step ] ) {
                        Swap( items, i, i + step );
                    }
                }
                step                   =  ( int ) ( step / k );
                //App.MyWindows.TextLine += GetString( items );
            }
            //App.MyWindows.TextLine += GetString( items );
            // Сортировка пузырьком
            bool swapped;
            do {
                swapped = false;
                for ( var i = 0; i < items.Length - 1; i++ ) {
                    if ( items[ i ] <= items[ i + 1 ] ) continue;
                    Swap( items, i, i + 1 );
                    swapped = true;
                }
                //App.MyWindows.TextLine += GetString( items );
            } while ( swapped );
        }

#endregion Сортировка пузырьком

#region Сортировка вставками

        /// <summary>
        /// Гномья cортировка
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void GnomeSort( int[] items ) {
            //App.MyWindows.TextLine = $"Гномья сортировка\r\n{GetString( items )}\r\n";
            for ( var i = 1; i < items.Length; i++ ) {
                if ( items[ i ] >= items[ i - 1 ] ) continue;
                Swap( items, i, i - 1 );
                // Проверка ранних элементов
                var j = i - 1;
                while ( j > 0 && items[ j ] < items[ j - 1 ] ) {
                    Swap( items, j, j - 1 );
                    j--;
                }
            }
            //App.MyWindows.TextLine += GetString( items );
        }

        /// <summary>
        /// Сортировка вставками
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void InsertionSort( int[] items ) {
            //App.MyWindows.TextLine = $"Сортировка вставками\r\n{GetString( items )}\r\n";

            // Цикл элементов массива
            for ( var i = 1; i < items.Length; i++ ) {
                if ( items[ i ] >= items[ i - 1 ] ) continue;
                // Цикл вставки
                var j    = i - 1;
                var temp = items[ i ];
                while ( j >= 0 && temp < items[ j ] ) {
                    items[ j + 1 ] = items[ j ];
                    j--;
                }
                if ( i != j + 1 ) items[ j + 1 ] = temp;
                //App.MyWindows.TextLine += GetString( items );
            }
        }

        /// <summary>
        /// Сортировка Шелла
        /// </summary>
        /// <param name="items"></param>
        public static void ShellSort( int[] items ) {
            //App.MyWindows.TextLine = $"Сортировка Шелла\r\n{GetString( items )}\r\n";

            var step = items.Length / 2;
            // Причесываем расческой
            while ( step >= 1 ) {
                for ( var i = 0; i < step; i++ ) {
                    // Цикл элементов массива
                    for ( var j = i + step; j < items.Length; j += step ) {
                        if ( items[ j ] >= items[ i ] ) continue;
                        // Цикл вставки
                        var index = j - step;
                        var temp  = items[ j ];
                        while ( index >= 0 && temp < items[ index ] ) {
                            items[ index + step ] =  items[ index ];
                            index                 -= step;
                        }
                        if ( j != index + step ) items[ index + step ] = temp;
                    }
                }
                step                   =  step / 2;
                //App.MyWindows.TextLine += GetString( items );
            }
        }

#endregion Сортировка вставками

#region Сортировка выбором

        /// <summary>
        /// Сортировка выбором
        /// </summary>
        /// <param name="items"></param>
        public static void SelectionSort( int[] items ) {
            //App.MyWindows.TextLine = $"Сортировка выбором\r\n{GetString( items )}\r\n";

            // Цикл элементов массива
            for ( var i = 0; i < items.Length - 1; i++ ) {
                // Поиск минимального значения
                var i_min = i;
                for ( var j = i + 1; j < items.Length; j++ ) {
                    if ( items[ i_min ] <= items[ j ] ) continue;
                    i_min = j;
                }
                if ( i != i_min ) Swap( items, i, i_min );
                //App.MyWindows.TextLine += GetString( items );
            }
        }

        /// <summary>
        /// Двухсторонняя сортировка выбором
        /// </summary>
        /// <param name="items"></param>
        public static void DoubleSelectionSort( int[] items ) {
            //App.MyWindows.TextLine = $"Двухсторонняя сортировка выбором\r\n{GetString( items )}\r\n";

            // Цикл элементов массива
            for ( var i = 0; i < items.Length >> 1; i++ ) {
                // Поиск минимального значения
                var i_min = i;
                for ( var j = i + 1; j < items.Length - i; j++ ) {
                    if ( items[ i_min ] <= items[ j ] ) continue;
                    i_min = j;
                }
                if ( i_min != i ) Swap( items, i, i_min );

                // Поиск максимального значения
                var i_max = items.Length - i - 1;
                for ( var j = i + 1; j < items.Length - i - 1; j++ ) {
                    if ( items[ i_max ] >= items[ j ] ) continue;
                    i_max = j;
                }
                if ( i_max != items.Length - 1 - i ) Swap( items, items.Length - 1 - i, i_max );

                //App.MyWindows.TextLine += GetString( items );
            }
        }

        /// <summary>
        /// Бинго сортировка
        /// </summary>
        /// <param name="items"></param>
        public static void BingoSort( int[] items ) {
            //App.MyWindows.TextLine = $"Бинго сортировка\r\n{GetString( items )}\r\n";

            // Поиск максимального значения
            var max_value = items[ 0 ];
            for ( var i = 1; i < items.Length; i++ ) {
                if ( max_value < items[ i ] ) max_value = items[ i ];
            }
            // Сортировка
            var i_max =  items.Length - 1;
            while ( i_max > 0 ) {
                // Пропуск элементов стоящих на своих местах
                if ( items[ i_max ] == max_value ) {
                    i_max--;
                    continue;
                }
                var next_value = 0;
                for ( var i = i_max - 1; i >= 0; i-- ) {
                    if ( items[ i ] == max_value ) {
                        Swap( items, i, i_max );
                        i_max--;
                        continue;
                    }
                    // Поиск следующего максимума
                    if ( next_value < items[ i ] ) next_value = items[ i ];
                }
                max_value              =  next_value;
                //App.MyWindows.TextLine += GetString( items );
            }
        }

        /// <summary>
        /// Цикличная сортировка
        /// </summary>
        /// <param name="items"></param>
        public static void CycleSort( int[] items ) {
            //App.MyWindows.TextLine = $"Цикличная сортировка\r\n{GetString( items )}\r\n";

            // Проходим по массиву в поиске циклических круговоротов
            for ( var i = 0; i < items.Length - 1; i++ ) {
                var value = items[ i ];
                // Поиск индекса элемента
                int i_min;
                do {
                    // Поиск количества элементов больше заданного
                    i_min = i;
                    for ( var j = i + 1; j < items.Length; j++ ) {
                        if ( items[ j ] < value ) i_min++;
                    }
                    // Если элемент занят равным, то сдвигаем его
                    if ( i_min != i ) {
                        while ( value == items[ i_min ] ) i_min++;
                    }
                    if ( items[ i_min ] == value ) continue;
                    // Меняем элемент
                    var temp = value;
                    value          = items[ i_min ];
                    items[ i_min ] = temp;
                } while ( i_min != i );
            }
            //App.MyWindows.TextLine += GetString( items );
        }

        /// <summary>
        /// Блинная сортировка
        /// </summary>
        /// <param name="items"></param>
        public static void PancakeSort( int[] items ) {
            //App.MyWindows.TextLine = $"Блинная сортировка\r\n{GetString( items )}\r\n";

            var i = items.Length - 1;
            while ( i > 0 ) {
                // Поиск максимального значения
                var max_value = 0;
                for ( var j = i; j >= 0; j-- ) {
                    if ( items[ j ] > max_value ) max_value = items[ j ];
                }
                // Поиск элементов равных максимальному значению
                for ( var j = i; j >= 0; j-- ) {
                    if ( items[ j ] != max_value ) continue;
                    if ( i != j ) {
                        Flip( items, 0, j );
                        Flip( items, 0, i );
                    }
                    i--;
                }
            }
            //App.MyWindows.TextLine += GetString( items );
        }

#endregion Сортировка выбором

#region Сортировка слиянием

        /// <summary>
        /// Cортировка слиянием
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void MergeSort( int[] items ) {
            //App.MyWindows.TextLine = $"Cортировка слиянием\r\n{GetString( items )}\r\n";

            var sort_items = MergeSort( items, 0, items.Length - 1 );
            //App.MyWindows.TextLine += GetString( sort_items );
        }

        /// <summary>
        /// Cортировка слиянием
        /// </summary>
        /// <param name="items"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int[] MergeSort( int[] items, int min, int max ) {
            if ( min >= max ) return items;
            var middle_index = ( min + max ) >> 1;
            MergeSort( items, min, middle_index );
            MergeSort( items, middle_index + 1, max );
            Merge( items, min, middle_index, max );
            return items;
        }

        /// <summary>
        /// метод для слияния массивов
        /// </summary>
        /// <param name="items"></param>
        /// <param name="min"></param>
        /// <param name="middle"></param>
        /// <param name="max"></param>
        private static void Merge( IList< int > items, int min, int middle, int max ) {
            var left       = min;
            var right      = middle + 1;
            var temp_array = new int[ max - min + 1 ];
            var index      = 0;

            while ( left <= middle && right <= max ) {
                if ( items[ left ] < items[ right ] ) {
                    temp_array[ index ] = items[ left ];
                    left++;
                } else {
                    temp_array[ index ] = items[ right ];
                    right++;
                }
                index++;
            }

            for ( var i = left; i <= middle; i++ ) {
                temp_array[ index ] = items[ i ];
                index++;
            }

            for ( var i = right; i <= max; i++ ) {
                temp_array[ index ] = items[ i ];
                index++;
            }

            for ( var i = 0; i < temp_array.Length; i++ ) {
                items[ min + i ] = temp_array[ i ];
            }
        }

#endregion Сортировка слиянием

#region Быстрая сортировка

        /// <summary>
        /// Быстрая сортировка
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void QuickSort( int[] items ) {
            //App.MyWindows.TextLine = $"Быстрая сортировка\r\n{GetString( items )}\r\n";

            var sort_items = QuickSort( items, 0, items.Length - 1 );
            //App.MyWindows.TextLine += GetString( sort_items );
        }

        /// <summary>
        /// Быстрая сортировка
        /// </summary>
        /// <param name="items"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int[] QuickSort( int[] items, int min, int max ) {
            if ( min >= max ) return items;

            var i = Partition( items, min, max );
            QuickSort( items, min, i - 1 );
            QuickSort( items, i      + 1, max );
            return items;
        }

        /// <summary>
        /// метод возвращающий индекс опорного элемента
        /// </summary>
        /// <param name="items"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int Partition( IList< int > items, int min, int max ) {
            var i = min;
            for ( var j = min; j < max; j++ ) {
                if ( items[ j ] > items[ max ] ) continue;
                Swap( items, i, j );
                i++;
            }
            Swap( items, i, max );
            return i;
        }

#endregion Быстрая сортировка
    }
}