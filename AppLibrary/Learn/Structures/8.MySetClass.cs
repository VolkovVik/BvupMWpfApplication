using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <inheritdoc />
    /// <summary>
    /// Множество
    /// </summary>
    /// <remarks>
    /// пересечения (intersection)
    /// объединение (union)
    /// разность (difference)
    /// симметрическая разность (symmetric difference)
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class MySetClass< T > : IEnumerable< T >
        where T : IComparable {
        /// <summary>
        /// Количество элементов
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Признак заполнения очереди
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Коллекция хранимых данных.
        /// </summary>
        private readonly List< T > _items = new List< T >();

        /// <summary>
        /// 
        /// </summary>
        public MySetClass() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public MySetClass( IEnumerable< T > items ) {
            Add( items );
        }

        /// <summary>
        /// Подпрограмма добавления элемента в множество
        /// </summary>
        /// <param name="item"></param>
        public void Add( T item ) {
            // Проверяем входные данные
            if ( item == null ) throw new ArgumentNullException( nameof( item ) );
            if ( _items.Contains( item ) ) throw new InvalidOperationException( "Item already exists in Set" );
            _items.Add( item );
        }

        /// <summary>
        /// Подпрограмма добавления элементов в множество
        /// </summary>
        /// <param name="list"></param>
        public void Add( IEnumerable< T > list ) {
            foreach ( var item in list ) {
                _items.Add( item );
            }
        }

        /// <summary>
        /// Подпрограмма удаления элементов из множества
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove( T item ) {
            // Проверяем входные данные
            if ( item == null ) throw new ArgumentNullException( nameof( item ) );
            return _items.Remove( item );
        }

        /// <summary>
        /// Подпрограмма проверки наличия элемента в множестве
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains( T item ) {
            // Проверяем входные данные
            if ( item == null ) throw new ArgumentNullException( nameof( item ) );
            return _items.Contains( item );
        }

        /// <summary>
        /// Алгоритм, возвращающий множество, полученное операцией объединения его с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public MySetClass< T > Union( MySetClass< T > other ) {
            // Проверяем входные данные
            if ( other == null ) throw new ArgumentNullException( nameof( other ) );
            var result = new MySetClass< T >( _items );
            foreach ( var item in other._items ) {
                if ( !Contains( item ) ) {
                    result.Add( item );
                }
            }
            return result;
        }

        /// <summary>
        /// Алгоритм, возвращающий множество, полученное операцией пересечения его с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public MySetClass< T > Intersection( MySetClass< T > other ) {
            // Проверяем входные данные
            if ( other == null ) throw new ArgumentNullException( nameof( other ) );
            var result = new MySetClass< T >();
            foreach ( var item in other._items ) {
                if ( Contains( item ) ) {
                    result.Add( item );
                }
            }
            return result;
        }

        /// <summary>
        /// Алгоритм, возвращающий множество, являющееся разностью текущего с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public MySetClass< T > Difference( MySetClass< T > other ) {
            // Проверяем входные данные
            if ( other == null ) throw new ArgumentNullException( nameof( other ) );
            var result = new MySetClass< T >( _items );
            foreach ( var item in other._items ) {
                result.Remove( item );
            }
            return result;
        }

        /// <summary>
        /// Алгоритм, возвращающий множество, полученное операцией объединения его с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public MySetClass< T > SymmetricDifference( MySetClass< T > other ) {
            // Проверяем входные данные
            if ( other == null ) throw new ArgumentNullException( nameof( other ) );
            var union        = Union( other );
            var intersection = Intersection( other );
            return union.Difference( intersection );
        }

        /// <summary>
        /// Подпрограмма возврата строки
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var str = $"Set {Count:D3} items : ";
            foreach ( var item in _items ) {
                str += item + "  ";
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator< T > GetEnumerator() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}