using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <inheritdoc />
    /// <summary>
    /// Кольцевой двусвязный список
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyDoubleCycleLinkedListClass< T > : IEnumerable< T > {
        /// <summary>
        /// Количество элементов
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public int Count { get; private set; }

        /// <summary>
        /// Признак заполнения очереди
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Первый (головной) элемент списка.
        /// </summary>
        private Item< T > _head;

        /// <summary>
        /// 
        /// </summary>
        public MyDoubleCycleLinkedListClass() {
            Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public MyDoubleCycleLinkedListClass( T data ) {
            SetHeadItem( data );
        }

        /// <summary>
        /// Задание первого элемента
        /// </summary>
        /// <param name="data"></param>
        private void SetHeadItem( T data ) {
            var item = new Item< T >( data );
            _head          = item;
            _head.Next     = item;
            _head.Previous = item;
            Count          = 1;
        }

        /// <summary>
        /// Очистить список.
        /// </summary>
        public void Clear() {
            _head = null;
            Count = 0;
        }

        /// <summary>
        /// Добавить данные в связный список.
        /// </summary>
        /// <param name="data"></param>
        public void Add( T data ) {
            // Не забываем проверять входные аргументы на null.
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // Если связный список пуст, то добавляем созданный элемент в начало,
            // иначе добавляем этот элемент как следующий за крайним элементом.
            if ( IsEmpty ) {
                SetHeadItem( data );
                return;
            }
            // Создаем новый элемент связного списка.
            var item = new Item< T >( data ) {
                Previous = _head.Previous,
                Next     = _head
            };
            _head.Previous.Next = item;
            _head.Previous      = item;
            Count++;
        }

        /// <summary>
        /// Добавить данные в связный список.
        /// </summary>
        /// <param name="data"></param>
        public void AddFirst( T data ) {
            // Не забываем проверять входные аргументы на null.
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // Если связный список пуст, то добавляем созданный элемент в начало,
            // иначе добавляем этот элемент как следующий за крайним элементом.
            if ( IsEmpty ) {
                SetHeadItem( data );
                return;
            }
            // Создаем новый элемент связного списка.
            var item = new Item< T >( data ) {
                Previous = _head.Previous,
                Next     = _head
            };
            _head.Previous.Next = item;
            _head.Previous      = item;
            _head               = item;
            Count++;
        }

        /// <summary>
        /// Удалить данные из связного списка.
        /// Выполняется удаление первого вхождения данных.
        /// </summary>
        /// <param name="data"> Данные, которые будут удалены </param>
        /// <param name="all"></param>
        public void Delete( T data, bool all = false ) {
            // Не забываем проверять входные аргументы на null.
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // Текущий обозреваемый элемент списка.
            var current = _head;
            // Цикл по всех элементам списка
            var count = Count;
            while ( count > 0 ) {
                count--;
                if ( current == null ) break;
                // Поиск заданного элемента
                if ( !current.Data.Equals( data ) ) {
                    // Элемент не равен заданному
                    // Переходим к следующему элементу списка.
                    current = current.Next;
                    continue;
                }
                // Элемент равен заданному
                // Удаляем текущий элемент
                Count--;
                if ( Count == 0 ) {
                    Clear();
                    break;
                }
                if ( current == _head ) {
                    // Удаление головного элемента
                    _head = current.Next;
                }
                var item = current;
                current.Previous.Next = item.Next;
                current.Next.Previous = item.Previous;
                // Выбор следующего элемента
                current = current.Previous.Next;
                if ( !all ) break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var count   = Count;
            var current = _head;
            // Цикл по всех элементам списка
            var str = $"DoubleLinkedList {Count:D3} items : ";
            while ( count > 0 ) {
                count--;
                if ( current == null ) break;
                str     += current.Data + "  ";
                current =  current.Next;
            }
            return str;
        }

        /// <inheritdoc />
        /// <summary>
        /// Вернуть перечислитель, выполняющий перебор всех элементов
        /// </summary>
        /// <returns></returns>
        public IEnumerator< T > GetEnumerator() {
            var current = _head;
            var count   = Count;
            while ( count > 0 ) {
                count--;
                if ( current == null ) break;
                yield return current.Data;
                current = current.Next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable< T > BackEnumerator() {
            var current = _head.Previous;
            var count   = Count;
            while ( count > 0 ) {
                count--;
                if ( current == null ) break;
                yield return current.Data;
                current = current.Previous;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Вернуть перечислитель, который осуществляет итерационный переход
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}